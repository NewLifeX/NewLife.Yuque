using System.Text.RegularExpressions;
using NewLife.Cube.Entity;
using NewLife.Log;
using NewLife.Yuque;
using NewLife.YuqueWeb.Entity;
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
        ProcessHtml(doc);

        if (!(doc as IEntity).HasDirty) return 0;

        doc.SyncTime = DateTime.Now;

        return doc.Update();
    }

    public Int32 ProcessHtml(Document doc)
    {
        var html = doc?.BodyHtml;
        if (html.IsNullOrEmpty()) return 0;

        var rules = HtmlRule.GetValids();
        foreach (var rule in rules)
        {
            switch (rule.Kind)
            {
                case RuleKinds.图片:
                    html = ProcessImage(doc, rule, html);
                    break;
                case RuleKinds.超链接:
                    html = ProcessLink(doc, rule, html);
                    break;
                case RuleKinds.文本:
                    html = ProcessText(doc, rule, html);
                    break;
            }
        }

        doc.Html = html;

        return 1;
    }

    private static readonly Regex _regexImage = new("<img.*?src=\"(.*?)\".*?>");
    public String ProcessImage(Document doc, HtmlRule rule, String html)
    {
        var ms = _regexImage.Matches(html);
        if (ms.Count == 0) return html;

        // 所有附件
        var list = Attachment.FindAllByCategoryAndKey("Yuque", doc.Id + "");

        var rs = _regexImage.Replace(html, match =>
        {
            var full = match.Groups[0].Value;
            var url = match.Groups[1].Value;
            if (url.IsNullOrEmpty()) return full;

            // 判断域名
            if (!rule.Rule.IsNullOrEmpty() && rule.Rule != "*")
            {
                var uri = new Uri(url);
                if (uri.Host != rule.Rule) return full;
            }

            try
            {
                var fileName = Path.GetFileName(url);
                var p = fileName.IndexOf('?');
                if (p > 0) fileName = fileName[..p];

                // 生成附件
                var att = list.FirstOrDefault(e => e.Source == url);
                if (att == null) att = new Attachment { Enable = true };

                att.Category = "Yuque";
                att.Key = doc.Id + "";
                att.Title = doc.Title;
                att.FileName = fileName;
                att.Source = url;
                att.Url = $"/{doc.BookCode}/{doc.Code}";

                // 尝试从文件名中解析出来原始上传时间
                if (att.UploadTime.Year < 2000)
                {
                    // 第一段是毫秒
                    var ss = fileName.Split('-');
                    if (ss.Length > 0)
                    {
                        var time = ss[0].ToLong().ToDateTime().ToLocalTime();
                        if (time.Year > 2000) att.UploadTime = time;
                    }
                }

                att.Save();

                // 异步抓取
                var filePath = att.GetFilePath().GetBasePath();
                if (!File.Exists(filePath)) _ = Task.Run(() => FetchAttachment(att));

                var ext = Path.GetExtension(url);
                var url2 = $"/images/{att.Id}{ext}";

                return full.Replace(url, url2);
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
                return full;
            }
        });

        return rs;
    }

    private static readonly Regex _regexLink = new("<a.*?href=\"(.*?)\".*?>");
    public String ProcessLink(Document doc, HtmlRule rule, String html)
    {
        var ms = _regexLink.Matches(html);
        if (ms.Count == 0) return html;

        var rs = _regexLink.Replace(html, match =>
        {
            var full = match.Groups[0].Value;
            var url = match.Groups[1].Value;
            if (url.IsNullOrEmpty()) return full;

            // 判断规则
            if (!rule.Rule.IsMatch(url)) return full;

            var url2 = rule.Target;
            if (!url2.IsNullOrEmpty() && url2.Contains("$1"))
            {
                var str = url.Replace(rule.Rule.Trim('*'), null);
                url2 = url2.Replace("$1", str);
            }

            return full.Replace(url, url2);
        });

        return rs;
    }

    public String ProcessText(Document doc, HtmlRule rule, String html)
    {
        if (rule.Rule.IsNullOrEmpty()) return html;

        return html.Replace(rule.Rule, rule.Target);
    }

    /// <summary>
    /// 处理附件
    /// </summary>
    /// <param name="att"></param>
    /// <returns></returns>
    public async Task<Int32> FetchAttachment(Attachment att)
    {
        var url = att.Source;
        if (url.IsNullOrEmpty()) return 0;

        try
        {
            await att.Fetch(url);
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);

            return 0;
        }

        return 1;
    }
}