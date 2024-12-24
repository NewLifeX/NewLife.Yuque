using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife.Caching;
using NewLife.Common;
using NewLife.Cube.Entity;
using NewLife.Log;
using NewLife.Serialization;
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
    private readonly DocumentService _documentService;
    private readonly ITracer _tracer;

    public YuqueController(BookService bookService, DocumentService documentService, ITracer tracer)
    {
        _bookService = bookService;
        _documentService = documentService;
        _tracer = tracer;
    }

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
            OrderBy = "Sort desc, Id desc",
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
            await _bookService.SyncDocument(inf, false);
        }

        // 增加浏览数
        inf.LocalHits++;
        (inf as IEntity).SaveAsync(15_000);

        ViewBag.Title = inf.Title;

        var model = new InfoView
        {
            Document = inf,
            Navs = _documentService.BuildNavs(inf.Html),
        };

        return View("Info", model);
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

    #region WebHook
    /// <summary>语雀WebHook接口。语雀文章新增或更新时调用</summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public ActionResult Notify([FromBody] Object body)
    {
        var json = body.ToString();
        using var span = _tracer.NewSpan(nameof(Notify), json);

        var dic = JsonParser.Decode(json);
        if (dic != null && dic.TryGetValue("data", out var data))
        {
            var detail = JsonHelper.Convert<WebHookModel>(data);
            if (detail != null && !detail.Slug.IsNullOrEmpty())
            {
                var doc = Document.GetOrAdd(detail.Id);

                // 如果是更新，则特殊处理发布，默认公开文档
                if (detail.ActionType == "publish")
                {
                    var dic2 = data as IDictionary<String, Object>;
                    if (dic2 != null && !dic2.ContainsKey("public"))
                    {
                        detail.Public = 1;
                        detail.Status = 1;
                    }
                }

                doc.Fill(detail);

                _bookService.ProcessHtml(doc);

                // 处理封面
                if (!detail.Cover.IsNullOrEmpty() && detail.Cover.StartsWithIgnoreCase("http://", "https://"))
                {
                    doc.Cover = _bookService.SaveImage(doc, detail.Cover, null);
                }

                doc.Sync = true;
                doc.SyncTime = DateTime.Now;

                // 简化埋点数据
                detail.Body = null;
                detail.BodyHtml = null;

                span.AppendTag(detail);

                doc.Save();
            }
        }
        else
        {
            XTrace.WriteLine(json);
        }

        return new EmptyResult();
    }
    #endregion
}