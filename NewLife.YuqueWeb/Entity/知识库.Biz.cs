using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Log;
using XCode;

namespace NewLife.YuqueWeb.Entity;

/// <summary>知识库。管理知识库</summary>
public partial class Book : Entity<Book>
{
    #region 对象操作
    static Book()
    {
        // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
        //var df = Meta.Factory.AdditionalFields;
        //df.Add(nameof(Docs));

        // 过滤器 UserInterceptor、TimeInterceptor、IPInterceptor
        Meta.Interceptors.Add<UserInterceptor>();
        //Meta.Interceptors.Add<TimeInterceptor>();
        Meta.Interceptors.Add<IPInterceptor>();
        Meta.Interceptors.Add<TraceInterceptor>();
    }

    /// <summary>验证并修补数据，通过抛出异常的方式提示验证失败。</summary>
    /// <param name="isNew">是否插入</param>
    public override void Valid(Boolean isNew)
    {
        // 如果没有脏数据，则不需要进行任何处理
        if (!HasDirty) return;

        // 这里验证参数范围，建议抛出参数异常，指定参数名，前端用户界面可以捕获参数异常并聚焦到对应的参数输入框
        if (Name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(Name), "名称不能为空！");

        // 建议先调用基类方法，基类方法会做一些统一处理
        base.Valid(isNew);

        //var span = DefaultSpan.Current;
        //if (span != null) TraceId = span.TraceId;
    }
    #endregion

    #region 扩展属性
    /// <summary>知识组</summary>
    [XmlIgnore, IgnoreDataMember]
    //[ScriptIgnore]
    public Group Group => Extends.Get(nameof(Group), k => Group.FindById(GroupId));

    /// <summary>知识组</summary>
    [Map(nameof(GroupId), typeof(Group), "Id")]
    public String GroupName => Group?.Name;
    #endregion

    #region 扩展查询
    /// <summary>根据编号查找</summary>
    /// <param name="id">编号</param>
    /// <returns>实体对象</returns>
    public static Book FindById(Int32 id)
    {
        if (id <= 0) return null;

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Id == id);

        // 单对象缓存
        return Meta.SingleCache[id];

        //return Find(_.Id == id);
    }

    /// <summary>根据编码查找</summary>
    /// <param name="code">编码</param>
    /// <returns>实体对象</returns>
    public static Book FindByCode(String code)
    {
        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Code.EqualIgnoreCase(code));

        return Find(_.Code == code);
    }

    /// <summary>根据名称查找</summary>
    /// <param name="name">名称</param>
    /// <returns>实体列表</returns>
    public static IList<Book> FindAllByName(String name)
    {
        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Name.EqualIgnoreCase(name));

        return FindAll(_.Name == name);
    }

    /// <summary>
    /// 获取可用知识库，并处理好排序
    /// </summary>
    /// <returns></returns>
    public static IList<Book> GetValids() => FindAllWithCache().Where(e => e.Enable).OrderByDescending(e => e.Sort).ToList();
    #endregion

    #region 高级查询
    /// <summary>高级查询</summary>
    /// <param name="groupId">知识组</param>
    /// <param name="enable">启用</param>
    /// <param name="start">更新时间开始</param>
    /// <param name="end">更新时间结束</param>
    /// <param name="key">关键字</param>
    /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
    /// <returns>实体列表</returns>
    public static IList<Book> Search(Int32 groupId, Boolean? enable, DateTime start, DateTime end, String key, PageParameter page)
    {
        var exp = new WhereExpression();

        //if (!code.IsNullOrEmpty()) exp &= _.Code == code;
        //if (!name.IsNullOrEmpty()) exp &= _.Name == name;
        if (groupId >= 0) exp &= _.GroupId == groupId;
        if (enable != null) exp &= _.Enable == enable;
        exp &= _.UpdateTime.Between(start, end);
        if (!key.IsNullOrEmpty()) exp &= _.Code.Contains(key) | _.Name.Contains(key) | _.Type.Contains(key) | _.UserName.Contains(key) | _.Slug.Contains(key) | _.Namespace.Contains(key) | _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

        return FindAll(exp, page);
    }

    // Select Count(Id) as Id,Category From Book Where CreateTime>'2020-01-24 00:00:00' Group By Category Order By Id Desc limit 20
    //static readonly FieldCache<Book> _CategoryCache = new FieldCache<Book>(nameof(Category))
    //{
    //Where = _.CreateTime > DateTime.Today.AddDays(-30) & Expression.Empty
    //};

    ///// <summary>获取类别列表，字段缓存10分钟，分组统计数据最多的前20种，用于魔方前台下拉选择</summary>
    ///// <returns></returns>
    //public static IDictionary<String, String> GetCategoryList() => _CategoryCache.FindAllName();
    #endregion

    #region 业务操作
    public void Fill(NewLife.Yuque.Models.Book repo)
    {
        var book = this;

        if (book.Code.IsNullOrEmpty()) book.Code = repo.Slug;
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
    }

    public void Fill(NewLife.Yuque.Models.BookDetail repo)
    {
        var book = this;

        //book.Code = repo.Slug;
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
        book.Remark = repo.Description;
        book.CreateTime = repo.CreateTime;
        book.UpdateTime = repo.UpdateTime;
    }
    #endregion
}