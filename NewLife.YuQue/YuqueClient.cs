using System.Text;
using NewLife.Http;
using NewLife.Log;
using NewLife.Remoting;
using NewLife.Serialization;
using NewLife.YuQue.Models;

namespace NewLife.YuQue
{
    /// <summary>语雀客户端</summary>
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
                    sb.AppendJoin('&', item.Key, "=", item.Value);
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
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<UserDetail> GetUser(String login)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await GetAsync<UserDetail>($"/users/{login}");
        }

        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id"></param>
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
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Group[]> GetUserGroups(String login)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await GetAsync<Group[]>($"/users/{login}/groups");
        }

        /// <summary>
        /// 根据Id获取用户加入的组织
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Group[]> GetUserGroups(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<Group[]>($"/users/{id}/groups");
        }

        /// <summary>
        /// 获取公开的组织
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Group[]> GetGroups() => await GetAsync<Group[]>($"/groups");

        /// <summary>
        /// 根据用户名获取组织信息
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> GetGroup(String login)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await GetAsync<GroupDetail>($"/groups/{login}");
        }

        /// <summary>
        /// 根据Id获取组织信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> GetGroup(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<GroupDetail>($"/groups/{id}");
        }

        /// <summary>
        /// 创建组织
        /// </summary>
        /// <param name="name"></param>
        /// <param name="login"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> CreateGroup(String name, String login, String description)
        {
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await PostAsync<GroupDetail>($"/groups", new { name, login, description });
        }

        /// <summary>
        /// 更新组织
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="login"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroup(String @namespace, String login, String description)
        {
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException(nameof(@namespace));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await PostAsync<GroupDetail>($"/groups", new { @namespace, login, description });
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroup(String login)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await DeleteAsync<GroupDetail>($"/groups/{login}");
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroup(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await DeleteAsync<GroupDetail>($"/groups/{id}");
        }

        /// <summary>
        /// 获取组织成员信息
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupUser[]> GetGroupUsers(String login)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await GetAsync<GroupUser[]>($"/groups/{login}/users");
        }

        /// <summary>
        /// 获取组织成员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupUser[]> GetGroupUsers(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<GroupUser[]>($"/groups/{id}/users");
        }

        /// <summary>
        /// 增加或更新组织成员
        /// </summary>
        /// <param name="group_login"></param>
        /// <param name="login"></param>
        /// <param name="role">0 - 管理员, 1 - 普通成员</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroupUser(String group_login, String login, Int32 role = 1)
        {
            if (group_login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(group_login));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await PutAsync<GroupDetail>($"/groups/{group_login}/{login}", new { role });
        }

        /// <summary>
        /// 增加或更新组织成员
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="login"></param>
        /// <param name="role">0 - 管理员, 1 - 普通成员</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroupUser(Int32 group_id, String login, Int32 role = 1)
        {
            if (group_id <= 0) throw new ArgumentNullException(nameof(group_id));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await PutAsync<GroupDetail>($"/groups/{group_id}/{login}", new { role });
        }

        /// <summary>
        /// 删除组织成员
        /// </summary>
        /// <param name="group_login"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroupUser(String group_login, String login)
        {
            if (group_login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(group_login));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await DeleteAsync<GroupDetail>($"/groups/{group_login}");
        }

        /// <summary>
        /// 删除组织成员
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> DeleteGroupUser(Int32 group_id, String login)
        {
            if (group_id <= 0) throw new ArgumentNullException(nameof(group_id));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await DeleteAsync<GroupDetail>($"/groups/{group_id}");
        }
        #endregion

        #region 知识库
        /// <summary>
        /// 获取某个用户的知识库列表
        /// </summary>
        /// <param name="login"></param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetRepos(String login, String type = "all", Int32 offset = 20)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await GetAsync<Book[]>($"/users/{login}/repos", new { type, offset });
        }

        /// <summary>
        /// 获取某个用户的知识库列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetRepos(Int32 id, String type = "all", Int32 offset = 20)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<Book[]>($"/users/{id}/repos", new { type, offset });
        }

        /// <summary>
        /// 获取某个团队的知识库列表
        /// </summary>
        /// <param name="login"></param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetGroupRepos(String login, String type = "all", Int32 offset = 20)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await GetAsync<Book[]>($"/groups/{login}/repos", new { type, offset });
        }

        /// <summary>
        /// 获取某个团队的知识库列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">Book, Design, all - 所有类型</param>
        /// <param name="offset">用于分页，效果类似 MySQL 的 limit offset，一页 20 条</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Book[]> GetGroupRepos(Int32 id, String type = "all", Int32 offset = 20)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<Book[]>($"/groups/{id}/repos", new { type, offset });
        }

        /// <summary>
        /// 创建知识库
        /// </summary>
        /// <param name="login">用户</param>
        /// <param name="model">仓库</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateRepo(String login, BookModel model)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/users/{login}/repos", model);
        }

        /// <summary>
        /// 创建知识库
        /// </summary>
        /// <param name="id">用户</param>
        /// <param name="model">仓库</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateRepo(Int32 id, BookModel model)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/users/{id}/repos", model);
        }

        /// <summary>
        /// 创建团队知识库
        /// </summary>
        /// <param name="login">团队</param>
        /// <param name="model">仓库</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateGroupRepo(String login, BookModel model)
        {
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/groups/{login}/repos", model);
        }

        /// <summary>
        /// 创建团队知识库
        /// </summary>
        /// <param name="id">团队</param>
        /// <param name="model">仓库</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> CreateGroupRepo(Int32 id, BookModel model)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PostAsync<BookDetail>($"/groups/{id}/repos", model);
        }

        /// <summary>
        /// 获取知识库详情
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> GetRepo(String @namespace)
        {
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException(nameof(@namespace));

            return await GetAsync<BookDetail>($"/repos/{@namespace}");
        }

        /// <summary>
        /// 获取知识库详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> GetRepo(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<BookDetail>($"/repos/{id}");
        }

        /// <summary>
        /// 更新知识库信息
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> UpdateRepo(String @namespace, BookModel model)
        {
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException(nameof(@namespace));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PutAsync<BookDetail>($"/repos/{@namespace}", model);
        }

        /// <summary>
        /// 更新知识库信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> UpdateRepo(Int32 id, BookModel model)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));
            if (model == null) throw new ArgumentNullException(nameof(model));

            return await PutAsync<BookDetail>($"/repos/{id}", model);
        }

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> DeleteRepo(String @namespace)
        {
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException(nameof(@namespace));

            return await DeleteAsync<BookDetail>($"/groups/{@namespace}");
        }

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<BookDetail> DeleteRepo(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await DeleteAsync<BookDetail>($"/groups/{id}");
        }
        #endregion

        #region 文档
        /// <summary>
        /// 获取一个仓库的文档列表
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Document[]> GetDocuments(String @namespace)
        {
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException(nameof(@namespace));

            return await GetAsync<Document[]>($"/repos/{@namespace}/docs");
        }

        /// <summary>
        /// 获取一个仓库的文档列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<Document[]> GetDocuments(Int32 id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            return await GetAsync<Document[]>($"/repos/{id}/docs");
        }

        /// <summary>
        /// 获取单篇文档的详细信息
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<DocumentDetail> GetDocument(String @namespace, String slug)
        {
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException(nameof(@namespace));
            if (slug.IsNullOrEmpty()) throw new ArgumentNullException(nameof(slug));

            return await GetAsync<DocumentDetail>($"/repos/{@namespace}/docs/{slug}");
        }
        #endregion

        #region 属性
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