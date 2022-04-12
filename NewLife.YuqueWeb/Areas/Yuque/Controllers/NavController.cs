using NewLife.Cube;
using NewLife.Web;
using NewLife.YuQueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    /// <summary>
    /// 知识库管理
    /// </summary>
    [YuqueArea]
    [Menu(50, true, Icon = "fa-tachometer")]
    public class NavController : EntityTreeController<Nav>
    {
        static NavController()
        {
            LogOnChange = true;

            ListFields.RemoveCreateField();
            ListFields.RemoveUpdateField();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override IEnumerable<Nav> Search(Pager p)
        {
            var parentId = p["parentId"].ToInt(-1);

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return Nav.Search(null, parentId, start, end, p["Q"], p);
        }
    }
}