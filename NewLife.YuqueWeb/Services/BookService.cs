using NewLife.Log;
using NewLife.Yuque;
using NewLife.YuQueWeb.Entity;
using XCode;
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
                        Enable = true,
                        Sync = repo.Public > 0,
                    };
                    book.Insert();

                    list.Add(book);
                }

                if (book.Name.IsNullOrEmpty()) book.Name = repo.Name;
                book.Slug = repo.Slug;
                book.Public = repo.Public > 0;
                book.Type = repo.Type;
                book.UserName = repo.User?.Name;
                book.Docs = repo.Items;
                book.Likes = repo.Likes;
                book.Watches = repo.Watches;
                book.Namespace = repo.Namespace;
                book.ContentUpdateTime = repo.ContentUpdateTime;
                book.Remark = repo.Description;
                book.CreateTime = repo.CreateTime;
                book.UpdateTime = repo.UpdateTime;

                book.SyncTime = DateTime.Now;

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

        // 同步知识库详细
        var repo = await client.GetRepo(bookId);
        if (repo != null)
        {
            if (book.Name.IsNullOrEmpty()) book.Name = repo.Name;
            book.Slug = repo.Slug;
            book.Public = repo.Public > 0;
            book.Type = repo.Type;
            book.UserName = repo.User?.Name;
            book.Docs = repo.Items;
            book.Likes = repo.Likes;
            book.Watches = repo.Watches;
            book.Namespace = repo.Namespace;
            book.ToC = repo.Toc;
            //book.ContentUpdateTime = repo.ContentUpdateTime;
            book.Remark = repo.Description;
            book.CreateTime = repo.CreateTime;
            book.UpdateTime = repo.UpdateTime;

            book.SyncTime = DateTime.Now;

            book.Update();
        }

        var count = 0;
        var offset = 0;
        while (true)
        {
            // 分批拉取
            var list = await client.GetDocuments(book.Id, offset);
            if (list.Length == 0) break;

            foreach (var item in list)
            {
                var doc = Document.FindById(item.Id);
                if (doc == null) doc = Document.FindByBookAndSlug(book.Id, item.Slug);
                if (doc == null)
                {
                    doc = new Document
                    {
                        Id = item.Id,
                        Code = item.Slug,
                        Slug = item.Slug,
                        Title = item.Title,
                        Enable = true,
                        Sync = item.Public > 0,
                    };
                    doc.Insert();
                }

                doc.Id = item.Id;
                doc.Title = item.Title;
                doc.Slug = item.Slug;
                doc.BookId = bookId;
                //doc.Sync = item.Public > 0;

                doc.UserName = item.LastEditor?.Name;
                doc.Format = item.Format;
                //doc.Hits = item.Hits;
                doc.Likes = item.Likes;
                doc.Comments = item.Comments;
                doc.WordCount = item.WordCount;
                if (!item.Cover.IsNullOrEmpty()) doc.Cover = item.Cover;
                doc.Remark = item.Description;

                //doc.SyncTime = DateTime.Now;
                doc.PublishTime = item.PublishTime;
                doc.FirstPublishTime = item.FirstPublishTime;
                doc.CreateTime = item.CreateTime;
                doc.UpdateTime = item.UpdateTime;

                doc.Update();
            }

            count += list.Length;
            offset += list.Length;
        }

        return count;
    }

    public async Task<Int32> Sync(Document doc)
    {
        var book = doc.Book;
        if (doc == null || !doc.Sync || book == null || !book.Sync) return 0;

        var token = GetToken();
        var client = new YuqueClient { Token = token, Log = XTrace.Log, Tracer = _tracer };

        var detail = await client.GetDocument(book.Namespace, doc.Slug);
        if (detail == null) return 0;

        doc.Id = detail.Id;
        doc.Title = detail.Title;
        doc.BookId = detail.BookId;
        doc.Slug = detail.Slug;

        doc.UserName = detail.Creator?.Name;
        doc.Format = detail.Format;

        doc.Body = detail.Body;
        doc.BodyHtml = detail.BodyHtml;
        doc.ContentUpdateTime = detail.ContentUpdateTime;

        doc.Hits = detail.Hits;
        doc.Likes = detail.Likes;
        doc.Comments = detail.Comments;
        doc.WordCount = detail.WordCount;

        if (!detail.Cover.IsNullOrEmpty()) doc.Cover = detail.Cover;
        doc.Remark = detail.Description;

        doc.PublishTime = detail.PublishTime;
        doc.FirstPublishTime = detail.FirstPublishTime;
        doc.CreateTime = detail.CreateTime;
        doc.UpdateTime = detail.UpdateTime;

        if (!(doc as IEntity).HasDirty) return 0;

        doc.SyncTime = DateTime.Now;

        return doc.Update();
    }
}