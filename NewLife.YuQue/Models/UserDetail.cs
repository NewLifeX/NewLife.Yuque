using System.Runtime.Serialization;

namespace NewLife.YuQue.Models
{
    /// <summary>用户信息</summary>
    public class UserDetail
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>类型</summary>
        public String Type { get; set; }

        /// <summary>空间</summary>
        [DataMember(Name = "space_id")]
        public Int32 SpaceId { get; set; }

        /// <summary>账号编号</summary>
        [DataMember(Name = "account_id")]
        public Int32 AccountId { get; set; }

        /// <summary>登录名</summary>
        public String Login { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>描述</summary>
        public String Description { get; set; }

        /// <summary>头像</summary>
        [DataMember(Name = "avatar_url")]
        public String Avatar { get; set; }

        /// <summary>知识库数</summary>
        [DataMember(Name = "books_count")]
        public Int32 BooksCount { get; set; }

        /// <summary>公开知识库数</summary>
        [DataMember(Name = "public_books_count")]
        public Int32 PublicBooksCount { get; set; }

        /// <summary>关注者</summary>
        [DataMember(Name = "followers_count")]
        public Int32 FollowersCount { get; set; }

        /// <summary>关注了</summary>
        [DataMember(Name = "following_count")]
        public Int32 FollowingCount { get; set; }

        /// <summary>公开</summary>
        public Int32 Public { get; set; }

        /// <summary>创建时间</summary>
        [DataMember(Name = "created_at")]
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        [DataMember(Name = "updated_at")]
        public DateTime UpdateTime { get; set; }
    }
}