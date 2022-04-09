using System.Runtime.Serialization;

namespace NewLife.YuQue.Models
{
    /// <summary>组织明细</summary>
    public class GroupDetail
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>空间</summary>
        [DataMember(Name = "space_id")]
        public Int32 SpaceId { get; set; }

        /// <summary>组织编号</summary>
        [DataMember(Name = "organization_id")]
        public Int32 OrganizationId { get; set; }

        /// <summary>登录名</summary>
        public String Login { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>拥有者</summary>
        [DataMember(Name = "owner_id")]
        public Int32 OwnerId { get; set; }

        /// <summary>描述</summary>
        public String Description { get; set; }

        /// <summary>头像</summary>
        [DataMember(Name = "avatar_url")]
        public String Avatar { get; set; }

        /// <summary>知识库数</summary>
        [DataMember(Name = "books_count")]
        public Int32 Books { get; set; }

        /// <summary>公开知识库数</summary>
        [DataMember(Name = "public_books_count")]
        public Int32 PublicBooks { get; set; }

        /// <summary>主题数</summary>
        [DataMember(Name = "topics_count")]
        public Int32 Topics { get; set; }

        /// <summary>公开主题数</summary>
        [DataMember(Name = "public_topics_count")]
        public Int32 PublicTopics { get; set; }

        /// <summary>成员数</summary>
        [DataMember(Name = "members_count")]
        public Int32 Members { get; set; }

        /// <summary>公开</summary>
        public Int32 Public { get; set; }

        /// <summary>稻谷</summary>
        [DataMember(Name = "grains_sum")]
        public Int32 Grains { get; set; }

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
}