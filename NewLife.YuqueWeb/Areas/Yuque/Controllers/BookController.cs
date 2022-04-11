using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Web;
using NewLife.YuqueWeb.Services;
using NewLife.YuQueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    [YuqueArea]
    [Menu(90, true, Icon = "fa-tachometer")]
    public class BookController : EntityController<Book>
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService) => _bookService = bookService;

        protected override IEnumerable<Book> Search(Pager p)
        {
            var enable = p["enable"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return Book.Search(null, null, enable, start, end, p["Q"], p);
        }

        /// <summary>同步知识库</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        public async Task<ActionResult> SyncRepo()
        {
            var count = 0;
            var ids = GetRequest("keys").SplitAsInt();
            foreach (var id in ids.OrderBy(e => e))
            {
                count += await _bookService.Sync(id);
            }

            return JsonRefresh($"共刷新[{count}]个知识库");
        }

        /// <summary>扫描全部知识库</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Insert)]
        public async Task<ActionResult> ScanAll()
        {
            var count = await _bookService.ScanAll();

            return JsonRefresh($"共扫描[{count}]个知识库");
        }
    }
}