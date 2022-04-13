using NewLife.Cube;
using NewLife.Web;
using NewLife.YuqueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    /// <summary>
    /// 知识库管理
    /// </summary>
    [YuqueArea]
    [Menu(30, true, Icon = "fa-tachometer")]
    public class HtmlRuleController : EntityController<HtmlRule>
    {
        static HtmlRuleController()
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
        protected override IEnumerable<HtmlRule> Search(Pager p)
        {
            var kind = p["kind"].ToInt(-1);

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return HtmlRule.Search(kind, start, end, p["Q"], p);
        }
    }
}