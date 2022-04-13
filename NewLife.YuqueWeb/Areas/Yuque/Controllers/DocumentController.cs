using NewLife.Cube;
using NewLife.Cube.ViewModels;
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
            var allows = new[] { "BookName", "Title", "Code", "Enable", "UserName", "Hits", "Likes", "Reads", "Comments", "Public", "Status", "PublishTime", "WordCount", "Sync", "SyncTime" };
            foreach (var item in allows)
            {
                list.AddListField(item);
            }

            {
                var df = list.GetField("Code") as ListField;
                df.Url = "/{BookCode}/{Code}";
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

            return Document.Search(null, null, bookId, start, end, p["Q"], p);
        }
    }
}