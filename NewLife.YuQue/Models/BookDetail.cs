using System.Runtime.Serialization;

namespace NewLife.YuQue.Models
{
    /// <summary>
    /// 知识库明细
    /// </summary>
    public class BookDetail
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>类型</summary>
        public String Type { get; set; }

        /// <summary>登录名</summary>
        public String Slug { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>用户</summary>
        [DataMember(Name = "user_id")]
        public Int32 UserId { get; set; }

        /// <summary>描述</summary>
        public String Description { get; set; }

        /// <summary>创建者</summary>
        [DataMember(Name = "creator_id")]
        public Int32 CreatorId { get; set; }

        /// <summary>公开状态 [1 - 公开, 0 - 私密]</summary>
        public Int32 Public { get; set; }

        /// <summary>文章数</summary>
        [DataMember(Name = "items_count")]
        public Int32 Items { get; set; }

        /// <summary>喜欢数量</summary>
        [DataMember(Name = "likes_count")]
        public Int32 Likes { get; set; }

        /// <summary> 订阅数量</summary>
        [DataMember(Name = "watches_count")]
        public Int32 Watches { get; set; }

        /// <summary>内容更新时间</summary>
        [DataMember(Name = "content_updated_at")]
        public DateTime ContentUpdated { get; set; }

        /// <summary>创建时间</summary>
        [DataMember(Name = "created_at")]
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        [DataMember(Name = "updated_at")]
        public DateTime UpdateTime { get; set; }

        /// <summary>命名空间</summary>
        public String Namespace { get; set; }

        /// <summary>用户</summary>
        public UserDetail User { get; set; }

        /// <summary>已重载。友好显示</summary>
        /// <returns></returns>
        public override String ToString() => Name;
    }
}