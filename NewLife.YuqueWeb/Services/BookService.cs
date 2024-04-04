using System.Text.RegularExpressions;
using NewLife.Cube.Entity;
using NewLife.Http;
using NewLife.Log;
using NewLife.Reflection;
using NewLife.Yuque;
using NewLife.YuqueWeb.Entity;
using Group = NewLife.YuqueWeb.Entity.Group;

namespace NewLife.YuqueWeb.Services;

/// <summary>
/// 知识库服务
/// </summary>
public partial class BookService
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
        using var span = _tracer?.NewSpan(nameof(ScanAll));

        var count = 0;

        var list = Book.FindAll();
        foreach (var item in Group.FindAllWithCache())
        {
            if (!item.Enable && item.Token.IsNullOrEmpty()) continue;

            using var span2 = _tracer?.NewSpan($"Scan-{item.Name}", item);

            var client = new YuqueClient { Token = item.Token, Log = XTrace.Log, Tracer = _tracer };
            var user = await client.GetUser();

            // 遍历用户的所有知识库
            var offset = 0;
            while (true)
            {
                var repos = await client.GetRepos(user.Id, "all", offset);
                if (repos.Length == 0) break;

                foreach (var repo in repos)
                {
                    var book = list.FirstOrDefault(e => e.Id == repo.Id || e.Slug == repo.Slug);
                    book ??= new Book { Id = repo.Id, Enable = true, };

                    book.Fill(repo);
                    book.SyncTime = DateTime.Now;

                    book.Save();
                }

                count += repos.Length;
                offset += repos.Length;

                if (span2 != null) span2.Value += repos.Length;
            }
        }

        return count;
    }

    private String GetToken(Book book)
    {
        var token = book?.Group?.Token;

        if (token.IsNullOrEmpty()) throw new Exception("未设置令牌！");

        return token;
    }

    /// <summary>
    /// 同步指定知识库之下的文章列表，不含文章内容
    /// </summary>
    /// <param name="bookId"></param>
    /// <returns></returns>
    public async Task<Int32> SyncBook(Int32 bookId)
    {
        using var span = _tracer?.NewSpan(nameof(SyncBook), bookId + "");

        var book = Book.FindById(bookId);
        if (book == null || !book.Sync) return 0;

        var token = GetToken(book);
        var client = new YuqueClient { Token = token, Log = XTrace.Log, Tracer = _tracer };

        // 同步知识库详细
        var repo = await client.GetRepo(bookId);
        if (repo != null)
        {
            book.Fill(repo);

            book.SyncTime = DateTime.Now;
            //book.TraceId = DefaultSpan.Current?.TraceId;

            book.Update();
        }

        // 同步知识库之下的文档列表
        var count = 0;
        var offset = 0;
        while (true)
        {
            // 分批拉取
            var list = await client.GetDocuments(book.Id, offset);
            if (list.Length == 0) break;

            DefaultSpan.Current?.AppendTag($"GetDocuments({book.Id}, {offset})=[{list.Length}]");
            foreach (var detail in list)
            {
                // 跳过未发布未公开文档，等它写好了发布后再同步
                if (detail.Status == 0 || detail.Public == 0) continue;

                var doc = Document.FindById(detail.Id);
                doc ??= Document.FindByBookAndSlug(book.Id, detail.Slug);
                doc ??= new Document { Id = detail.Id, Enable = true, Sync = detail.Public > 0, };

                doc.Fill(detail);
                //doc.TraceId = DefaultSpan.Current?.TraceId;

                doc.Save();
            }

            count += list.Length;
            offset += list.Length;
            if (span != null) span.Value += list.Length;
        }

        return count;
    }

    /// <summary>同步文章，核心内容处理。跳过没有同步标记的文档</summary>
    /// <param name="doc"></param>
    /// <param name="noPublic"></param>
    /// <returns></returns>
    public async Task<Int32> SyncDocument(Document doc, Boolean noPublic)
    {
        var book = doc?.Book;
        if (doc == null || !doc.Sync || book == null || !book.Sync) return 0;

        var token = GetToken(book);
        var client = new YuqueClient { Token = token, Log = XTrace.Log, Tracer = _tracer };

        var detail = await client.GetDocument(book.Namespace, doc.Slug);
        if (detail == null) return 0;

        // 如果目标文档处于草稿状态，则不要同步，否则将会导致文档在前台消失不可见
        if (!noPublic && detail.Status == 0) return 0;

        doc.Fill(detail);

        // 处理HTML
        //if (ProcessHtml(doc) > 0) _ = Task.Run(() => FetchAttachment(doc));
        ProcessHtml(doc);

        //if (!(doc as IEntity).HasDirty) return 0;

        doc.Sync = true;
        doc.SyncTime = DateTime.Now;
        //doc.TraceId = DefaultSpan.Current?.TraceId;

        return doc.Update();
    }

    /// <summary>处理文档HTML内存，主要是抓取图片附件、替换超链接和文本</summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public Int32 ProcessHtml(Document doc)
    {
        var html = doc?.BodyHtml;
        if (html.IsNullOrEmpty()) return 0;

        var rules = HtmlRule.GetValids();
        foreach (var rule in rules)
        {
            html = rule.Kind switch
            {
                RuleKinds.图片 => ProcessImage(doc, rule, html),
                RuleKinds.超链接 => ProcessLink(doc, rule, html),
                RuleKinds.文本 => ProcessText(doc, rule, html),
                _ => html,
            };
        }

        doc.Html = html;

        return 1;
    }

    [GeneratedRegex("<img.*?src=\"(.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline, "zh-CN")]
    private static partial Regex MyImageRegex();

    private static readonly Regex _regexImage = MyImageRegex();
    /// <summary>抓取图片附件到本地</summary>
    /// <param name="doc"></param>
    /// <param name="rule"></param>
    /// <param name="html"></param>
    /// <returns></returns>
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
                att ??= new Attachment { Enable = true };

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
                if (!File.Exists(filePath))
                    _ = Task.Run(() => FetchAttachment(att));
                else if (!filePath.IsNullOrEmpty())
                {
                    var fi = filePath.AsFile();
                    if (!fi.Exists || fi.Length < 1024) _ = Task.Run(() => FetchAttachment(att));
                }

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

    [GeneratedRegex("<a.*?href=\"(.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline, "zh-CN")]
    private static partial Regex MyLinkRegex();

    private static readonly Regex _regexLink = MyLinkRegex();
    /// <summary>按规则替换超链接地址</summary>
    /// <param name="doc"></param>
    /// <param name="rule"></param>
    /// <param name="html"></param>
    /// <returns></returns>
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
                // 特殊处理卡片跳转，把id链接替换为真实链接
                var flag = false;
                var p = url.ToLower().IndexOf("/go/doc/");
                if (p > 0)
                {
                    p += "/go/doc/".Length;
                    var id = url[p..].ToInt();
                    var doc = Document.FindById(id);
                    if (doc != null)
                    {
                        url2 = $"/{doc.BookCode}/{doc.Code}";
                        flag = true;
                    }
                }

                if (!flag)
                {
                    var str = url.Replace(rule.Rule.Trim('*'), null);
                    url2 = url2.Replace("$1", str);
                }
            }

            return full.Replace(url, url2);
        });

        return rs;
    }

    /// <summary>按规则替换文本内容</summary>
    /// <param name="doc"></param>
    /// <param name="rule"></param>
    /// <param name="html"></param>
    /// <returns></returns>
    public String ProcessText(Document doc, HtmlRule rule, String html)
    {
        if (rule.Rule.IsNullOrEmpty()) return html;

        return html.Replace(rule.Rule, rule.Target);
    }

    /// <summary>抓取远程附件</summary>
    /// <param name="att"></param>
    /// <returns></returns>
    public async Task<Int32> FetchAttachment(Attachment att)
    {
        var url = att.Source;
        if (url.IsNullOrEmpty()) return 0;

        //!! 强行设置HttpClient，设置UserAgent，避免被列入黑名单而无法抓取图片
        var client = att.GetType().GetValue("_client") as HttpClient;
        if (client == null)
        {
            client = new HttpClient();
            client.SetUserAgent();
            att.GetType().SetValue("_client", client);
        }

        using var span = _tracer.NewSpan(nameof(FetchAttachment), url);
        try
        {
            await att.Fetch(url);
        }
        catch (Exception ex)
        {
            span.SetError(ex, att);
            XTrace.WriteException(ex);

            return 0;
        }

        return 1;
    }
}