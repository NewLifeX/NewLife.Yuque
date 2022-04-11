using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Web;
using NewLife.YuQue;
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
        public async Task<ActionResult> SyncRepo()
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

        /// <summary>扫描全部知识库</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Insert)]
        public async Task<ActionResult> ScanAll()
        {
            var count = 0;

            // 获取令牌
            var p = Parameter.GetOrAdd(0, "Yuque", "Token");
            if (p.Value.IsNullOrEmpty())
            {
                if (p.Remark.IsNullOrEmpty())
                {
                    p.Remark = "访问语雀的令牌，账户设置/Token，https://www.yuque.com/settings/tokens";
                    p.Update();
                }

                throw new Exception("未设置令牌！[系统管理/字典参数/Yuque/Token]");
            }

            var client = new YuqueClient { Token = p.Value };
            var user = await client.GetUser();

            var list = Book.FindAll();
            var offset = 0;
            while (true)
            {
                var repos = await client.GetRepos(user.Id, "all", offset);
                if (repos.Length == 0) break;

                foreach (var repo in repos)
                {
                    var book = list.FirstOrDefault(e => e.Id == repo.Id || e.Slug == repo.Slug);
                    if (book == null)
                    {
                        book = new Book
                        {
                            Id = repo.Id,
                            Name = repo.Name,
                            Slug = repo.Slug,
                            Code = repo.Slug,
                            Enable = true
                        };
                        book.Insert();

                        list.Add(book);
                    }

                    book.Name = repo.Name;
                    book.Type = repo.Type;
                    book.UserName = repo.User?.Name;
                    book.Docs = repo.Items;
                    book.Likes = repo.Likes;
                    book.Watches = repo.Watches;
                    book.Namespace = repo.Namespace;
                    book.Remark = repo.Description;
                    book.SyncTime = DateTime.Now;

                    book.Update();
                }

                count += repos.Length;
                offset += repos.Length;
            }

            return JsonRefresh($"共扫描[{count}]个知识库");
        }
    }
}