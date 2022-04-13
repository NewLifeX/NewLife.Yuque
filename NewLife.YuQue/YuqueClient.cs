using System.Text;
using NewLife.Http;
using NewLife.Log;
using NewLife.Remoting;
using NewLife.Serialization;
using NewLife.Yuque.Models;

namespace NewLife.Yuque
{
    /// <summary>语雀客户端</summary>
    /// <remarks>
    /// 文档 https://www.yuque.com/yuque/developer/api
    /// </remarks>
    public class YuqueClient
    {
        #region 属性
        /// <summary>服务端地址</summary>
        public String Server { get; set; } = "https://www.yuque.com/api/v2";

        /// <summary>访问令牌</summary>
        /// <remarks>https://www.yuque.com/settings/tokens</remarks>
        public String Token { get; set; }

        private String _prefix;
        private HttpClient _client;
        #endregion

        #region 基础方法
        private void Init()
        {
            if (_client == null)
            {
                if (Server.IsNullOrEmpty()) throw new ArgumentNullException(nameof(Server));
                if (Token.IsNullOrEmpty()) throw new ArgumentNullException(nameof(Token));

                var uri = new Uri(Server);
                _prefix = uri.AbsolutePath;

                _client = Tracer.CreateHttpClient();
                _client.SetUserAgent();
                _client.BaseAddress = uri;
                //_client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                _client.DefaultRequestHeaders.Add("X-Auth-Token", Token);
            }
        }

        /// <summary>Get调用</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<TResult> GetAsync<TResult>(String action, Object args = null)
        {
            Init();

            var url = _prefix + action;
            if (args != null)
            {
                var sb = new StringBuilder();
                foreach (var item in args.ToDictionary())
                {
                    if (sb.Length > 0) sb.Append('&');
                    sb.AppendFormat("{0}={1}", item.Key, item.Value);
                }
                if (sb.Length > 0) url += "?" + sb;
            }

            var rs = await _client.GetStringAsync(url);
            if (rs.IsNullOrEmpty()) return default;

            return ConvertResponse<TResult>(rs);
        }

        /// <summary>Post调用</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<TResult> PostAsync<TResult>(String action, Object args)
        {
            Init();

            var rs = await _client.PostJsonAsync(_prefix + action, args);
            if (rs.IsNullOrEmpty()) return default;
            if (rs[0] != '{' || rs[^1] != '}') return default;

            return ConvertResponse<TResult>(rs);
        }

        /// <summary>Put调用</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<TResult> PutAsync<TResult>(String action, Object args)
        {
            Init();

            HttpContent content = null;
            if (args != null)
            {
                content = args is String str
                    ? new StringContent(str, Encoding.UTF8, "application/json")
                    : new StringContent(args.ToJson(), Encoding.UTF8, "application/json");
            }
            var request = new HttpRequestMessage(HttpMethod.Put, _prefix + action)
            {
                Content = content
            };

            var response = await _client.SendAsync(request);
            var rs = await response.Content.ReadAsStringAsync();

            return ConvertResponse<TResult>(rs);
        }

        /// <summary>Delete调用</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<TResult> DeleteAsync<TResult>(String action, Object args = null)
        {
            Init();

            var url = _prefix + action;
            if (args != null)
            {
                var sb = new StringBuilder();
                foreach (var item in args.ToDictionary())
                {
                    sb.AppendJoin('&', item.Key, "=", item.Value);
                }
                if (sb.Length > 0) url += "?" + sb;
            }

            var response = await _client.DeleteAsync(url);
            var rs = await response.Content.ReadAsStringAsync();

            return ConvertResponse<TResult>(rs);
        }

        TResult ConvertResponse<TResult>(String rs)
        {
            if (rs.IsNullOrEmpty()) return default;
            if (rs[0] != '{' || rs[^1] != '}') return default;

            var dic = JsonParser.Decode(rs);
            if (dic == null || dic.Count == 0) return default;

            // 异常处理
            if (dic.TryGetValue("message", out var message)) throw new ApiException(500, message + "");

            if (!dic.TryGetValue("data", out var data)) return default;

            return JsonHelper.Convert<TResult>(data);
        }
        #endregion

        #region 用户
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="name">登录名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<UserDetail> GetUser(String name)
        {
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));

            return await GetAsync<UserDetail>($"/users/{name}");
        }

        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<UserDetail> GetUser(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<UserDetail>($"/users/{id}");
        }

        /// <summary>
        /// 获取当前认证用户
        /// </summary>
        /// <returns></returns>
        public virtual async Task<UserDetail> GetUser() => await GetAsync<UserDetail>($"/user");
        #endregion

        #region 组织
        /// <summary>
        /// 根据用户名获取用户加入的组织
        /// </summary>
        /// <param name="userName">登录名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Group[]> GetUserGroups(String userName)
        {
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await GetAsync<Group[]>($"/users/{userName}/groups");
        }

        /// <summary>
        /// 根据Id获取用户加入的组织
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Group[]> GetUserGroups(Int32 userId)
        {
            if (userId <= 0) throw new ArgumentNullException(nameof(userId));

            return await GetAsync<Group[]>($"/users/{userId}/groups");
        }

        /// <summary>
        /// 获取公开的组织
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Group[]> GetGroups() => await GetAsync<Group[]>($"/groups");

        /// <summary>
        /// 创建组织
        /// </summary>
        /// <param name="nickName">昵称</param>
        /// <param name="name">登录名</param>
        /// <param name="description">说明</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> CreateGroup(String nickName, String name, String description)
        {
            if (nickName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(nickName));
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));

            return await PostAsync<GroupDetail>($"/groups", new { name = nickName, login = name, description });
        }

        /// <summary>
        /// 根据名称获取组织信息
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> GetGroup(String groupName)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));

            return await GetAsync<GroupDetail>($"/groups/{groupName}");
        }

        /// <summary>
        /// 根据Id获取组织信息
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> GetGroup(Int32 groupId)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));

            return await GetAsync<GroupDetail>($"/groups/{groupId}");
        }

        /// <summary>
        /// 更新组织
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <param name="nickName">昵称</param>
        /// <param name="name">新组织名</param>
        /// <param name="description">说明</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroup(String groupName, String nickName, String name, String description)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));
            if (nickName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(nickName));
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));

            return await PutAsync<GroupDetail>($"/groups/{groupName}", new { name = nickName, login = name, description });
        }

        /// <summary>
        /// 更新组织
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <param name="nickName">昵称</param>
        /// <param name="name">新组织名</param>
        /// <param name="description">说明</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroup(Int32 groupId, String nickName, String name, String description)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));
            if (nickName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(nickName));
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));

            return await PutAsync<GroupDetail>($"/groups/{groupId}", new { name = nickName, login = name, description });
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroup(String groupName)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));

            return await DeleteAsync<GroupDetail>($"/groups/{groupName}");
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroup(Int32 groupId)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));

            return await DeleteAsync<GroupDetail>($"/groups/{groupId}");
        }

        /// <summary>
        /// 获取组织成员信息
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupUser[]> GetGroupUsers(String groupName)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));

            return await GetAsync<GroupUser[]>($"/groups/{groupName}/users");
        }

        /// <summary>
        /// 获取组织成员信息
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupUser[]> GetGroupUsers(Int32 groupId)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));

            return await GetAsync<GroupUser[]>($"/groups/{groupId}/users");
        }

        /// <summary>
        /// 增加或更新组织成员
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <param name="userName">用户名</param>
        /// <param name="role">0 - 管理员, 1 - 普通成员</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroupUser(String groupName, String userName, Int32 role = 1)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await PutAsync<GroupDetail>($"/groups/{groupName}/users/{userName}", new { role });
        }

        /// <summary>
        /// 增加或更新组织成员
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <param name="userName">用户名</param>
        /// <param name="role">0 - 管理员, 1 - 普通成员</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroupUser(Int32 groupId, String userName, Int32 role = 1)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await PutAsync<GroupDetail>($"/groups/{groupId}/users/{userName}", new { role });
        }

        /// <summary>
        /// 删除组织成员
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroupUser(String groupName, String userName)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await DeleteAsync<GroupDetail>($"/groups/{groupName}/users/{userName}");
        }

        /// <summary>
        /// 删除组织成员
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroupUser(Int32 groupId, String userName)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await DeleteAsync<GroupDetail>($"/groups/{groupId}/users/{userName}");
        }
        #endregion

        #region 知识库
        /// <summary>
        /// 获取某个用户的知识库列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetRepos(String userName, String type = "all", Int32 offset = 0)
        {
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await GetAsync<Book[]>($"/users/{userName}/repos", new { type, offset });
        }

        /// <summary>
        /// 获取某个用户的知识库列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetRepos(Int32 userId, String type = "all", Int32 offset = 0)
        {
            if (userId <= 0) throw new ArgumentNullException(nameof(userId));

            return await GetAsync<Book[]>($"/users/{userId}/repos", new { type, offset });
        }

        /// <summary>
        /// 获取某个团队的知识库列表
        /// </summary>
        /// <param name="groupName">登录名</param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetGroupRepos(String groupName, String type = "all", Int32 offset = 0)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));

            return await GetAsync<Book[]>($"/groups/{groupName}/repos", new { type, offset });
        }

        /// <summary>
        /// 获取某个团队的知识库列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetGroupRepos(Int32 groupId, String type = "all", Int32 offset = 0)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));

            return await GetAsync<Book[]>($"/groups/{groupId}/repos", new { type, offset });
        }

        /// <summary>
        /// 创建知识库
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="model">仓库模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateRepo(String userName, BookModel model)
        {
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/users/{userName}/repos", model);
        }

        /// <summary>
        /// 创建知识库
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="model">仓库模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateRepo(Int32 userId, BookModel model)
        {
            if (userId <= 0) throw new ArgumentNullException(nameof(userId));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/users/{userId}/repos", model);
        }

        /// <summary>
        /// 创建团队知识库
        /// </summary>
        /// <param name="groupName">组织名</param>
        /// <param name="model">仓库模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateGroupRepo(String groupName, BookModel model)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(groupName));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/groups/{groupName}/repos", model);
        }

        /// <summary>
        /// 创建团队知识库
        /// </summary>
        /// <param name="groupId">组织编号</param>
        /// <param name="model">仓库</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateGroupRepo(Int32 groupId, BookModel model)
        {
            if (groupId <= 0) throw new ArgumentNullException(nameof(groupId));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/groups/{groupId}/repos", model);
        }

        /// <summary>
        /// 获取知识库详情
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> GetRepo(String bookName)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));

            return await GetAsync<BookDetail>($"/repos/{bookName}");
        }

        /// <summary>
        /// 获取知识库详情
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> GetRepo(Int32 bookId)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));

            return await GetAsync<BookDetail>($"/repos/{bookId}");
        }

        /// <summary>
        /// 更新知识库信息
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <param name="model">仓库模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> UpdateRepo(String bookName, BookModel2 model)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PutAsync<BookDetail>($"/repos/{bookName}", model);
        }

        /// <summary>
        /// 更新知识库信息
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <param name="model">仓库模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> UpdateRepo(Int32 bookId, BookModel2 model)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PutAsync<BookDetail>($"/repos/{bookId}", model);
        }

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> DeleteRepo(String bookName)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));

            return await DeleteAsync<BookDetail>($"/groups/{bookName}");
        }

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> DeleteRepo(Int32 bookId)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));

            return await DeleteAsync<BookDetail>($"/groups/{bookId}");
        }
        #endregion

        #region 文档
        /// <summary>
        /// 获取一个仓库的文档列表
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Document[]> GetDocuments(String bookName, Int32 offset = 0)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));

            return await GetAsync<Document[]>($"/repos/{bookName}/docs", new { offset });
        }

        /// <summary>
        /// 获取一个仓库的文档列表
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Document[]> GetDocuments(Int32 bookId, Int32 offset = 0)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));

            return await GetAsync<Document[]>($"/repos/{bookId}/docs", new { offset });
        }

        /// <summary>
        /// 获取单篇文档的详细信息
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <param name="docName">文档路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> GetDocument(String bookName, String docName)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));
            if (docName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(docName));

            return await GetAsync<DocumentDetail>($"/repos/{bookName}/docs/{docName}");
        }

        /// <summary>
        /// 创建文档
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <param name="model">文档模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> CreateDocument(String bookName, DocumentModel model)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<DocumentDetail>($"/repos/{bookName}/docs", model);
        }

        /// <summary>
        /// 创建文档
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <param name="model">文档模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> CreateDocument(Int32 bookId, DocumentModel model)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<DocumentDetail>($"/repos/{bookId}/docs", model);
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <param name="docId">文档编号</param>
        /// <param name="model">文档模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> UpdateDocument(String bookName, Int32 docId, DocumentModel2 model)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));
            if (docId <= 0) throw new ArgumentNullException(nameof(docId));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PutAsync<DocumentDetail>($"/repos/{bookName}/docs/{docId}", model);
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <param name="docId">文档编号</param>
        /// <param name="model">文档模型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> UpdateDocument(Int32 bookId, Int32 docId, DocumentModel2 model)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));
            if (docId <= 0) throw new ArgumentNullException(nameof(docId));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PutAsync<DocumentDetail>($"/repos/{bookId}/docs/{docId}", model);
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="bookName">仓库路径</param>
        /// <param name="docId">文档编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> DeleteDocument(String bookName, Int32 docId)
        {
            if (bookName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(bookName));
            if (docId <= 0) throw new ArgumentNullException(nameof(docId));

            return await DeleteAsync<DocumentDetail>($"/repos/{bookName}/docs/{docId}");
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="bookId">仓库编号</param>
        /// <param name="docId">文档编号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> DeleteDocument(Int32 bookId, Int32 docId)
        {
            if (bookId <= 0) throw new ArgumentNullException(nameof(bookId));
            if (docId <= 0) throw new ArgumentNullException(nameof(docId));

            return await DeleteAsync<DocumentDetail>($"/repos/{bookId}/docs/{docId}");
        }
        #endregion

        #region 辅助
        /// <summary>性能追踪</summary>
        public ITracer Tracer { get; set; }

        /// <summary>日志</summary>
        public ILog Log { get; set; }

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
        #endregion
    }
}