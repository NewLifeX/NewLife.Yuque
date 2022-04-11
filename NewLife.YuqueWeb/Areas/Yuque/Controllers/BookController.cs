using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Web;
using NewLife.YuQueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    [YuqueArea]
    [Menu(90, true, Icon = "fa-tachometer")]
    public class BookController : EntityController<Book>
    {
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
        public ActionResult SyncRepo()
        {
            var count = 0;
            var ids = GetRequest("keys").SplitAsInt();
            if (ids.Length > 0)
                foreach (var id in ids)
                {
                    var team = Book.FindById(id);
                    if (team != null)
                    {
                    }
                }

            return JsonRefresh($"共刷新[{count}]个知识库");
        }
    }
}