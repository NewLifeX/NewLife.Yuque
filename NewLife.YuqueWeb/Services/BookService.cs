using NewLife.Log;
using NewLife.YuQue;
using NewLife.YuQueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Services;

/// <summary>
/// 知识库服务
/// </summary>
public class BookService
{
    private readonly ITracer _tracer;

    /// <summary>
    /// 实例化知识库服务
    /// </summary>
    /// <param name="tracer"></param>
    public BookService(ITracer tracer)
    {
        _tracer = tracer;
    }

    /// <summary>
    /// 扫描发现所有知识库
    /// </summary>
    /// <returns></returns>
    public async Task<Int32> ScanAll()
    {
        var count = 0;
        var token = GetToken();

        var client = new YuqueClient { Token = token, Log = XTrace.Log, Tracer = _tracer };
        var user = await client.GetUser();

        var list = Book.FindAll();
        var offset = 0;
        while (true)
        {
            var repos = await client.GetRepos(user.Id, "all", offset);
            if (repos.Length == 0) break;

            foreach (var item in repos)
            {
                var book = list.FirstOrDefault(e => e.Id == item.Id || e.Slug == item.Slug);
                if (book == null)
                {
                    book = new Book
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Slug = item.Slug,
                        Code = item.Slug,
                        Enable = true,
                        Sync = item.Public > 0,
                    };
                    book.Insert();

                    list.Add(book);
                }

                book.Name = item.Name;
                book.Type = item.Type;
                book.UserName = item.User?.Name;
                book.Docs = item.Items;
                book.Likes = item.Likes;
                book.Watches = item.Watches;
                book.Namespace = item.Namespace;
                book.Remark = item.Description;
                book.SyncTime = DateTime.Now;
                book.CreateTime = item.CreateTime;
                book.UpdateTime = item.UpdateTime;

                book.Update();
            }

            count += repos.Length;
            offset += repos.Length;
        }

        return count;
    }

    String GetToken()
    {
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

        return p.Value;
    }

    /// <summary>
    /// 同步指定知识库之下的文章列表
    /// </summary>
    /// <param name="bookId"></param>
    /// <returns></returns>
    public async Task<Int32> Sync(Int32 bookId)
    {
        var book = Book.FindById(bookId);
        if (book == null || !book.Sync) return 0;

        var token = GetToken();
        var client = new YuqueClient { Token = token, Log = XTrace.Log, Tracer = _tracer };

        var count = 0;
        var offset = 0;
        while (true)
        {
            var list = await client.GetDocuments(book.Id, offset);
            if (list.Length == 0) break;

            foreach (var item in list)
            {
                var doc = Document.FindByCode(item.Slug);
                if (doc == null)
                {
                    doc = new Document
                    {
                        Id = item.Id,
                        Code = item.Slug,
                        Title = item.Title,
                        BookId = bookId,
                        Slug = item.Slug,
                        Enable = true,
                        Sync = item.Public > 0,
                    };
                    doc.Insert();
                }

                doc.UserName = item.LastEditor?.Name;
                doc.Format = item.Format;
                //doc.Hits = item.Hits;
                doc.Likes = item.Likes;
                doc.Comments = item.Comments;
                doc.WordCount = item.WordCount;
                doc.Cover = item.Cover;
                doc.Remark = item.Description;

                doc.SyncTime = DateTime.Now;
                doc.PublishTime = item.PublishTime;
                doc.PublishTime = item.FirstPublishTime;
                doc.CreateTime = item.CreateTime;
                doc.UpdateTime = item.UpdateTime;

                doc.Update();
            }

            count += list.Length;
            offset += list.Length;
        }

        return count;
    }
}