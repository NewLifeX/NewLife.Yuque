using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife.Caching;
using NewLife.Common;
using NewLife.Cube.Entity;
using NewLife.Web;
using NewLife.YuqueWeb.Entity;
using NewLife.YuqueWeb.Models;
using NewLife.YuqueWeb.Services;
using XCode;

namespace NewLife.YuqueWeb.Controllers;

/// <summary>
/// 语雀控制器
/// </summary>
[AllowAnonymous]
public class YuqueController : Controller
{
    /// <summary>分页大小</summary>
    public static Int32 PageSize => 20;

    private readonly BookService _bookService;

    public YuqueController(BookService bookService) => _bookService = bookService;

    private static Boolean ViewExists(String vpath) => System.IO.File.Exists(vpath.GetFullPath());

    private static readonly ICache _cache = MemoryCache.Default;

    private static String GetView(String name, String category)
    {
        var viewName = $"../{category}/{name}";
        var view = _cache.Get<String>(viewName);
        if (view != null) return view;

        // 如果频道模版不存在，则采用模型模版

        // 模型目录的模版
        view = viewName;
        var vpath = $"Views/{category}/{name}.cshtml";
        if (ViewExists(vpath))
        {
            _cache.Set(viewName, view);
            return view;
        }

        // 内容目录的模板
        view = $"../Content/{name}";
        vpath = $"Views/Content/{name}.cshtml";
        if (ViewExists(vpath))
        {
            _cache.Set(viewName, view);
            return view;
        }

        // 共享目录的模板
        view = $"../Shared/{name}";
        vpath = $"Views/Shared/{name}.cshtml";
        if (ViewExists(vpath))
        {
            _cache.Set(viewName, view);
            return view;
        }

        return null;
    }

    public ActionResult Index()
    {
        var list = Book.GetValids();

        var sys = SysConfig.Current;
        ViewBag.Title = sys.DisplayName;

        return View("Index", list);
    }

    #region 分类列表页
    /// <summary>
    /// 分类列表页
    /// </summary>
    /// <param name="categoryCode"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    public ActionResult List(String categoryCode, Int32? pageIndex)
    {
        var book = Book.FindByCode(categoryCode);
        if (book == null) return NotFound();

        //// 选择模版
        //var tmp = cat.GetCategoryTemplate();
        //if (tmp.IsNullOrEmpty() || !ViewExists(tmp)) tmp = GetView("Book", cat.Model);

        var page = new Pager
        {
            PageIndex = pageIndex ?? 1,
            PageSize = PageSize,
            RetrieveTotalCount = true,
        };

        var list = Document.Search(null, book.Id, true, true, true, DateTime.MinValue, DateTime.MinValue, null, page);

        var model = new BookIndexModel
        {
            Book = book,
            Documents = list,
            Page = page,
        };

        ViewBag.Title = book.Name;

        return View(model);
    }
    #endregion

    #region 信息详细页
    /// <summary>
    /// 信息详细页
    /// </summary>
    /// <param name="categoryCode"></param>
    /// <param name="infoCode"></param>
    /// <returns></returns>
    public async Task<ActionResult> Detail(String categoryCode, String infoCode)
    {
        var book = Book.FindByCode(categoryCode);
        if (book == null) return NotFound();

        var inf = Document.FindByBookAndCode(book.Id, infoCode);
        if (inf == null) return NotFound();

        // 正文不存在时，去同步一份
        if (inf.Html.IsNullOrEmpty())
        {
            await _bookService.Sync(inf, false);
        }

        // 增加浏览数
        inf.LocalHits++;
        (inf as IEntity).SaveAsync(15_000);

        ViewBag.Title = inf.Title;

        return View("Info", inf);
    }
    #endregion

    #region 搜索页
    /// <summary>
    /// 搜索页
    /// </summary>
    /// <param name="key"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    public ActionResult Search(String key, Int32? pageIndex)
    {
        //var name = RouteData.Values["modelName"] + "";

        var code = RouteData.Values["categoryCode"] + "";
        var book = Book.FindByCode(code);

        var pager = new Pager { PageIndex = pageIndex ?? 1, PageSize = PageSize };
        var list = Document.Search(null, book.Id, true, true, true, DateTime.MinValue, DateTime.MinValue, key, pager);

        ViewData["Title"] = $"搜索[{key}]";

        return View(list);
    }
    #endregion

    #region 附件
    public async Task<ActionResult> Image(String id)
    {
        if (id.IsNullOrEmpty()) return NotFound();

        // 去掉仅用于装饰的后缀名
        var p = id.IndexOf('.');
        if (p > 0) id = id[..p];

        var att = Attachment.FindById(id.ToLong());
        if (att == null) return NotFound();

        // 如果附件不存在，则抓取
        var filePath = att.GetFilePath();
        if (filePath.IsNullOrEmpty() || !System.IO.File.Exists(filePath))
        {
            var rs = await _bookService.FetchAttachment(att);
            if (rs == 0) return NotFound();

            filePath = att.GetFilePath();
        }

        if (!att.ContentType.IsNullOrEmpty())
            return PhysicalFile(filePath, att.ContentType);
        else
            return PhysicalFile(filePath, "image/png");
    }
    #endregion

    #region 跳转
    public ActionResult Go(String type, String id)
    {
        if (id.IsNullOrEmpty()) return NotFound();

        if (type.EqualIgnoreCase("doc"))
        {
            var doc = Document.FindById(id.ToInt());
            if (doc != null) return Redirect($"/{doc.BookCode}/{doc.Code}");
        }

        return NotFound();
    }
    #endregion
}