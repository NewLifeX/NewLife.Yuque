using NewLife.Cube;
using NewLife.Web;
using NewLife.YuQueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    [YuqueArea]
    [Menu(80)]
    public class DocumentController : EntityController<Document>
    {
        protected override IEnumerable<Document> Search(Pager p)
        {
            var bookId = p["bookId"].ToInt(-1);

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return Document.Search(null, null, bookId, start, end, p["Q"], p);
        }
    }
}