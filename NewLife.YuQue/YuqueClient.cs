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
                _client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                _client.DefaultRequestHeaders.Add("X-Auth-Token", Token);
            }
        }

        /// <summary>Get调用</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual async Task<TResult> GetAsync<TResult>(String action)
        {
            Init();

            var rs = await _client.GetStringAsync(_prefix + action);
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
        /// <returns></returns>
        public virtual async Task<TResult> DeleteAsync<TResult>(String action)
        {
            Init();

            var response = await _client.DeleteAsync(_prefix + action);
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
        public virtual async Task<UserDetail> GetUser(Int64 id)
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
        public virtual async Task<Group[]> GetUserGroups(Int64 id)
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
        public virtual async Task<GroupDetail> GetGroup(Int64 id)
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
        /// <param name="name"></param>
        /// <param name="login"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<GroupDetail> UpdateGroup(String name, String login, String description)
        {
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));
            if (login.IsNullOrEmpty()) throw new ArgumentNullException(nameof(login));

            return await PostAsync<GroupDetail>($"/groups", new { name, login, description });
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
        public virtual async Task<GroupUser[]> GetGroupUsers(Int64 id)
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