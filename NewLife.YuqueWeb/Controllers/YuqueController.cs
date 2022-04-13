using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife.Collections;
using NewLife.Common;
using NewLife.Web;
using NewLife.YuqueWeb.Models;
using NewLife.YuQueWeb.Entity;
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

        static Boolean ViewExists(String vpath) => System.IO.File.Exists(vpath.GetFullPath());

        static DictionaryCache<String, String> _cache = new(StringComparer.OrdinalIgnoreCase);
        static String GetView(String name, String category)
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
    }
}