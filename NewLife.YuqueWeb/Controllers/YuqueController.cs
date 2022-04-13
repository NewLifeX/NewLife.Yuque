using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife.Collections;
using NewLife.Common;
using NewLife.Cube.Entity;
using NewLife.Web;
using NewLife.YuqueWeb.Entity;
using NewLife.YuqueWeb.Models;
using NewLife.YuqueWeb.Services;
using XCode;

namespace NewLife.YuqueWeb.Controllers
{
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

        private static readonly DictionaryCache<String, String> _cache = new(StringComparer.OrdinalIgnoreCase);

        private static String GetView(String name, String category)
        {
            var viewName = $"../{category}/{name}";

            // 如果频道模版不存在，则采用模型模版
            return _cache.GetItem(viewName, k =>
            {
                // 模型目录的模版
                var view = k;
                var vpath = $"Views/{category}/{name}.cshtml";
                if (ViewExists(vpath)) return view;

                // 内容目录的模板
                view = $"../Content/{name}";
                vpath = $"Views/Content/{name}.cshtml";
                if (ViewExists(vpath)) return view;

                // 共享目录的模板
                view = $"../Shared/{name}";
                vpath = $"Views/Shared/{name}.cshtml";
                if (ViewExists(vpath)) return view;

                return null;
            });
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

            var list = Document.Search(null, null, book.Id, DateTime.MinValue, DateTime.MinValue, null, page);

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
        public ActionResult Detail(String categoryCode, String infoCode)
        {
            var book = Book.FindByCode(categoryCode);
            if (book == null) return NotFound();

            var inf = Document.FindByBookAndCode(book.Id, infoCode);
            if (inf == null) return NotFound();

            //// 选择模版
            //var tmp = cat.GetInfoTemplate();
            //if (tmp.IsNullOrEmpty() || !ViewExists(tmp)) tmp = GetView("Info", inf.Model);

            // 增加浏览数
            inf.LocalHits++;
            //inf.Statistics.Increment(null);
            (inf as IEntity).SaveAsync(15);

            ViewBag.Title = inf.Title;

            return View("Info", inf);
            //return Content(inf.BodyHtml, "text/html", Encoding.UTF8);
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
            var list = Document.Search(null, null, book.Id, DateTime.MinValue, DateTime.MinValue, key, pager);

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

            var set = NewLife.Cube.Setting.Current;

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
    }
}