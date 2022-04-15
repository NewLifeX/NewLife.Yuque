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
    /// 文档管理
    /// </summary>
    [YuqueArea]
    [Menu(80)]
    public class DocumentController : EntityController<Document>
    {
        private readonly BookService _bookService;

        /// <summary>
        /// 实例化知识库管理
        /// </summary>
        /// <param name="bookService"></param>
        public DocumentController(BookService bookService) => _bookService = bookService;

        static DocumentController()
        {
            LogOnChange = true;

            //ListFields.RemoveCreateField();
            //ListFields.RemoveUpdateField();

            //ListFields.RemoveField("Code", "BookName", "ContentUpdateTime");

            var list = ListFields;
            list.Clear();
            var allows = new[] { "BookName", "Title", "Code", "Enable", "UserName", "Hits", "TotalHits", "Likes", "Reads", "Comments", "Public", "Status", "PublishTime", "WordCount", "Sync", "SyncTime" };
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

            p.RetrieveState = true;

            return Document.Search(null, null, bookId, start, end, p["Q"], p);
        }

        /// <summary>同步知识库</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        public async Task<ActionResult> SyncAll()
        {
            var count = 0;
            var ids = GetRequest("keys").SplitAsInt();
            foreach (var id in ids.OrderBy(e => e))
            {
                var doc = Document.FindById(id);
                if (doc != null) count += await _bookService.Sync(doc);
            }

            return JsonRefresh($"共刷新[{count}]篇文章");
        }
    }
}