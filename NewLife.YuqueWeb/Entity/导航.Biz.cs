using System.ComponentModel;
using NewLife.Data;
using NewLife.Log;
using XCode;
using XCode.Membership;

namespace NewLife.YuqueWeb.Entity
{
    /// <summary>导航</summary>
    public partial class Nav : EntityTree<Nav>
    {
        #region 对象操作
        static Nav()
        {
            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            //var df = Meta.Factory.AdditionalFields;
            //df.Add(nameof(ParentId));

            // 过滤器 UserInterceptor、TimeInterceptor、IPInterceptor
            Meta.Interceptors.Add<UserInterceptor>();
            Meta.Interceptors.Add<TimeInterceptor>();
            Meta.Interceptors.Add<IPInterceptor>();
        }

        /// <summary>验证并修补数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            // 建议先调用基类方法，基类方法会做一些统一处理
            base.Valid(isNew);
        }

        /// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void InitData()
        {
            base.InitData();

            // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
            // Meta.Count是快速取得表记录数
            if (Meta.Count > 0) return;

            // 需要注意的是，如果该方法调用了其它实体类的首次数据库操作，目标实体类的数据初始化将会在同一个线程完成
            if (XTrace.Debug) XTrace.WriteLine("开始初始化{0}[{1}]数据……", typeof(Nav).Name, Meta.Table.DataTable.DisplayName);

            var header = Root.Add("头部");
            header.Add("首页", "/");
            header.Add("产品方案", "#")
                .Add("物联网平台", "http://iot.feifan.link")
                .Add("星尘分布式系统", "http://star.newlifex.com");
            header.Add("关于我们", "/About");

            var footer = Root.Add("尾部");
            var link = footer.Add("友情链接");
            link.Add("新生命团队", "https://www.NewLifeX.com");
            link.Add("开源项目", "https://github.com/NewLifeX");

            footer.Add("文档").Add("文档资料", "https://www.newlifex.com");
            footer.Add("关于")
                .Add("关于我们", "/About");

            var jumbotron = Root.Add("巨幕");
            jumbotron.Add("学无先后达者为师！");

            if (XTrace.Debug) XTrace.WriteLine("完成初始化{0}[{1}]数据！", typeof(Nav).Name, Meta.Table.DataTable.DisplayName);
        }
        #endregion

        #region 扩展属性
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static Nav FindById(Int32 id)
        {
            if (id <= 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Id == id);

            // 单对象缓存
            return Meta.SingleCache[id];

            //return Find(_.Id == id);
        }

        /// <summary>根据父类、名称查找</summary>
        /// <param name="parentId">父类</param>
        /// <param name="name">名称</param>
        /// <returns>实体对象</returns>
        public static Nav FindByParentIdAndName(Int32 parentId, String name)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.ParentId == parentId && e.Name.EqualIgnoreCase(name));

            return Find(_.ParentId == parentId & _.Name == name);
        }

        /// <summary>根据名称查找</summary>
        /// <param name="name">名称</param>
        /// <returns>实体列表</returns>
        public static IList<Nav> FindAllByName(String name)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Name.EqualIgnoreCase(name));

            return FindAll(_.Name == name);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="name">名称</param>
        /// <param name="parentId">父类</param>
        /// <param name="start">更新时间开始</param>
        /// <param name="end">更新时间结束</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<Nav> Search(String name, Int32 parentId, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (!name.IsNullOrEmpty()) exp &= _.Name == name;
            if (parentId >= 0) exp &= _.ParentId == parentId;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.Name.Contains(key) | _.Url.Contains(key) | _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }
        #endregion

        #region 业务操作
        /// <summary>添加导航</summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Nav Add(String name, String url = null)
        {
            var entity = new Nav()
            {
                ParentId = Id,
                Name = name,
                Url = url,
                Enable = true,

                // 排序，新的在后面
                Sort = Childs.Count + 1
            };
            entity.Insert();

            return entity;
        }
        #endregion
    }
}