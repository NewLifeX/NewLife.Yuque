using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Cube.Extensions;
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

            ListFields.RemoveField("Token");
            //ListFields.RemoveCreateField();
            //ListFields.RemoveUpdateField();
            ListFields.TraceUrl();

            {
                var df = ListFields.GetField("Books") as ListField;
                df.Url = "book?groupId={Id}";
            }
        }

        private readonly GroupService _groupService;

        /// <summary>
        /// 实例化知识组管理
        /// </summary>
        /// <param name="groupService"></param>
        public GroupController(GroupService groupService) => _groupService = groupService;

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
                await _groupService.Sync(id);
                count++;
            }

            return JsonRefresh($"共刷新[{count}]个知识组");
        }
    }
}