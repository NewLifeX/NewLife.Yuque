using System.Text.RegularExpressions;
using NewLife.Http;
using NewLife.Cube.Entity;
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
    public BookService(ITracer tracer) => _tracer = tracer;

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

    private String GetToken()
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

            foreach (var detail in list)
            {
                var doc = Document.FindById(detail.Id);
                if (doc == null) doc = Document.FindByBookAndSlug(book.Id, detail.Slug);
                if (doc == null)
                {
                    doc = new Document
                    {
                        Id = detail.Id,
                        Code = detail.Slug,
                        Slug = detail.Slug,
                        Title = detail.Title,
                        Enable = true,
                        Sync = detail.Public > 0,
                    };
                    doc.Insert();
                }

                doc.Id = detail.Id;
                doc.Title = detail.Title;
                doc.Slug = detail.Slug;
                doc.BookId = detail.BookId;
                doc.Public = detail.Public > 0;
                doc.Status = detail.Status > 0;

                doc.UserName = detail.LastEditor?.Name;
                doc.Format = detail.Format;
                //doc.Hits = item.Hits;
                doc.Likes = detail.Likes;
                doc.Reads = detail.Reads;
                doc.Comments = detail.Comments;
                doc.WordCount = detail.WordCount;
                if (!detail.Cover.IsNullOrEmpty()) doc.Cover = detail.Cover;
                doc.Remark = detail.Description;

                doc.DraftVersion = detail.DraftVersion;
                doc.ContentUpdateTime = detail.ContentUpdateTime;
                //doc.SyncTime = DateTime.Now;
                doc.PublishTime = detail.PublishTime;
                doc.FirstPublishTime = detail.FirstPublishTime;
                doc.CreateTime = detail.CreateTime;
                doc.UpdateTime = detail.UpdateTime;

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

        // 未正式公开时，允许修改Code
        if (detail.Public == 0) doc.Code = detail.Slug;

        doc.UserName = detail.Creator?.Name;
        doc.Format = detail.Format;
        doc.Public = detail.Public > 0;
        doc.Status = detail.Status > 0;

        doc.Body = detail.Body;
        doc.BodyHtml = detail.BodyHtml;
        doc.ContentUpdateTime = detail.ContentUpdateTime;

        doc.Hits = detail.Hits;
        //doc.Reads = detail.Reads;
        doc.Likes = detail.Likes;
        doc.Comments = detail.Comments;
        doc.WordCount = detail.WordCount;

        if (!detail.Cover.IsNullOrEmpty()) doc.Cover = detail.Cover;
        doc.Remark = detail.Description;

        doc.PublishTime = detail.PublishTime;
        doc.FirstPublishTime = detail.FirstPublishTime;
        doc.CreateTime = detail.CreateTime;
        doc.UpdateTime = detail.UpdateTime;

        if (detail.DeleteTime.Year > 2000) doc.Enable = false;

        // 处理HTML
        //if (ProcessHtml(doc) > 0) _ = Task.Run(() => FetchAttachment(doc));
        if (ProcessHtml(doc) == 0) doc.Html = doc.BodyHtml;

        if (!(doc as IEntity).HasDirty) return 0;

        doc.SyncTime = DateTime.Now;

        return doc.Update();
    }

    static Regex _regex = new("<img.*?src=\"(.*?)\".*?>");
    public Int32 ProcessHtml(Document doc)
    {
        var html = doc?.BodyHtml;
        if (html.IsNullOrEmpty()) return 0;

        var ms = _regex.Matches(html);
        if (ms.Count == 0) return 0;

        //foreach (Match item in ms)
        //{
        //    var url = item.Groups[1].Value;
        //    XTrace.WriteLine(url);
        //}

        //if (!_regex.IsMatch(html)) return 0;

        // 所有附件
        var list = FindAllAttachment(doc.Id);

        var rs = _regex.Replace(html, match =>
        {
            var url = match.Groups[1].Value;
            if (url.IsNullOrEmpty()) return url;

            var fileName = Path.GetFileName(url);
            var p = fileName.IndexOf('?');
            if (p > 0) fileName = fileName[..p];

            var att = list.FirstOrDefault(e => e.Remark == url);
            if (att == null) att = new Attachment();

            att.Category = "Yuque";
            att.Key = doc.Id + "";
            att.Title = doc.Title;
            att.FileName = fileName;
            att.Remark = url;

            att.Save();

            var ext = Path.GetExtension(url);
            var url2 = $"/images/{att.ID}{ext}";

            return match.Groups[0].Value.Replace(url, url2);
        });

        doc.Html = rs;

        return ms.Count;
    }

    static IList<Attachment> FindAllAttachment(Int32 docId) => Attachment.FindAll(Attachment._.Category == "" & Attachment._.Key == docId + "");

    /// <summary>
    /// 处理附件
    /// </summary>
    /// <param name="att"></param>
    /// <returns></returns>
    public async Task<Int32> FetchAttachment(Attachment att)
    {
        var url = att.Remark;
        if (url.IsNullOrEmpty()) return 0;

        var ext = Path.GetExtension(att.FileName);
        var set = NewLife.Cube.Setting.Current;
        var fileName = set.UploadPath.CombinePath(att.Category, DateTime.Today.Year + "", att.ID + ext);
        var fileName2 = fileName.GetFullPath();

        XTrace.WriteLine("抓取附件 {0}，保存到 {1}", url, fileName);

        fileName.EnsureDirectory(true);
        if (File.Exists(fileName2)) File.Delete(fileName2);

        var client = new HttpClient();
        //await client.DownloadFileAsync(url, fileName.GetFullPath());
        var rs = await client.GetAsync(url);
        att.ContentType = rs.Content.Headers.ContentType + "";

        {
            using var fs = new FileStream(fileName2, FileMode.Create);
            await rs.Content.CopyToAsync(fs);
        }

        var fi = fileName.AsFile();
        att.Size = fi.Length;
        att.Hash = fi.MD5().ToHex();
        att.Path = fileName;

        if (att.ContentType.IsNullOrEmpty() && ext.EqualIgnoreCase(".png")) att.ContentType = "image/png";

        att.Update();

        return 1;
    }
}