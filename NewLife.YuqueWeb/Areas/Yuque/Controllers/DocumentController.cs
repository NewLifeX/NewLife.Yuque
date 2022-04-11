using NewLife.Cube;
using NewLife.Web;
using NewLife.YuQueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers
{
    /// <summary>
    /// 文档管理
    /// </summary>
    [YuqueArea]
    [Menu(80)]
    public class DocumentController : EntityController<Document>
    {
        static DocumentController()
        {
            LogOnChange = true;

            //ListFields.RemoveCreateField();
            //ListFields.RemoveUpdateField();

            //ListFields.RemoveField("Code", "BookName", "ContentUpdateTime");

            var list = ListFields;
            list.Clear();
            var allows = new[] { "BookName", "Title", "Enable", "UserName", "Hits", "Likes", "Comments", "PublishTime", "WordCount", "Sync", "SyncTime" };
            foreach (var item in allows)
            {
                list.AddListField(item);
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
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