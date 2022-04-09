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
            if (rs[0] != '{' || rs[^1] != '}') return default;

            var dic = JsonParser.Decode(rs);
            if (dic == null || dic.Count == 0) return default;

            if (!dic.TryGetValue("data", out var data)) return default;

            return JsonHelper.Convert<TResult>(data);
        }

        /// <summary>Post调用</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<TResult> PostAsync<TResult>(String action, Object args)
        {
            Init();

            return await _client.InvokeAsync<TResult>(HttpMethod.Post, action, args, null, "data");
        }
        #endregion

        #region 用户
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<UserDetail> GetUser(String userName)
        {
            if (userName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userName));

            return await GetAsync<UserDetail>($"/users/{userName}");
        }

        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<UserDetail> GetUser(Int64 userId)
        {
            if (userId <= 0) throw new ArgumentNullException(nameof(userId));

            return await GetAsync<UserDetail>($"/users/{userId}");
        }

        /// <summary>
        /// 获取当前认证用户
        /// </summary>
        /// <returns></returns>
        public virtual async Task<UserDetail> GetCurrentUser() => await GetAsync<UserDetail>($"/user");
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