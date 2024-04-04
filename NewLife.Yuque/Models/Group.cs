using System.Runtime.Serialization;

namespace NewLife.Yuque.Models;

/// <summary>组织</summary>
public class Group
{
    /// <summary>编号</summary>
    public Int32 Id { get; set; }

    /// <summary>类型</summary>
    public String Type { get; set; }

    /// <summary>登录名</summary>
    public String Login { get; set; }

    /// <summary>名称</summary>
    public String Name { get; set; }

    /// <summary>头像</summary>
    [DataMember(Name = "avatar_url")]
    public String Avatar { get; set; }

    /// <summary>知识库数</summary>
    [DataMember(Name = "books_count")]
    public Int32 Books { get; set; }

    /// <summary>公开知识库数</summary>
    [DataMember(Name = "public_books_count")]
    public Int32 PublicBooks { get; set; }

    ///// <summary>主题数</summary>
    //[DataMember(Name = "topics_count")]
    //public Int32 Topics { get; set; }

    ///// <summary>公开主题数</summary>
    //[DataMember(Name = "public_topics_count")]
    //public Int32 PublicTopics { get; set; }

    /// <summary>成员数</summary>
    [DataMember(Name = "members_count")]
    public Int32 Members { get; set; }

    /// <summary>公开状态 [1 - 公开, 0 - 私密]</summary>
    public Int32 Public { get; set; }

    /// <summary>描述</summary>
    public String Description { get; set; }

    /// <summary>创建时间</summary>
    [DataMember(Name = "created_at")]
    public DateTime CreateTime { get; set; }

    /// <summary>更新时间</summary>
    [DataMember(Name = "updated_at")]
    public DateTime UpdateTime { get; set; }

    /// <summary>已重载。友好显示</summary>
    /// <returns></returns>
    public override String ToString() => !Name.IsNullOrEmpty() ? Name : Login;
}