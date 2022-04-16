using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Cube.ViewModels;
using NewLife.Web;
using NewLife.YuqueWeb.Entity;
using NewLife.YuqueWeb.Services;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    /// <summary>
    /// 知识小组管理
    /// </summary>
    [YuqueArea]
    [Menu(100, true, Icon = "fa-tachometer")]
    public class GroupController : EntityController<Group>
    {
        static GroupController()
        {
            LogOnChange = true;

            //ListFields.RemoveCreateField();
            //ListFields.RemoveUpdateField();

            {
                var df = ListFields.GetField("book") as ListField;
                df.Url = "book?Id={GroupId}";
            }
        }

        private readonly GroupService _bookService;

        /// <summary>
        /// 实例化知识库管理
        /// </summary>
        /// <param name="bookService"></param>
        public GroupController(GroupService bookService) => _bookService = bookService;

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override IEnumerable<Group> Search(Pager p)
        {
            var id = p["id"].ToInt(-1);
            if (id > 0)
            {
                var entity = Group.FindById(id);
                if (entity != null) return new[] { entity };
            }

            //var enable = p["enable"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return Group.Search(null, null, start, end, p["Q"], p);
        }

        /// <summary>同步知识组</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        public async Task<ActionResult> SyncGroup()
        {
            var count = 0;
            var ids = GetRequest("keys").SplitAsInt();
            foreach (var id in ids.OrderBy(e => e))
            {
                count += await _bookService.Sync(id);
            }

            return JsonRefresh($"共刷新[{count}]个知识组");
        }
    }
}