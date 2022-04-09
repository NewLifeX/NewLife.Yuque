using System.Runtime.Serialization;

namespace NewLife.YuQue.Models
{
    /// <summary>
    /// 文档
    /// </summary>
    public class Document
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>登录名</summary>
        public String Slug { get; set; }

        /// <summary>标题</summary>
        public String Title { get; set; }

        /// <summary>描述</summary>
        public String Description { get; set; }

        /// <summary>用户</summary>
        [DataMember(Name = "user_id")]
        public Int32 UserId { get; set; }

        /// <summary>仓库</summary>
        [DataMember(Name = "book_id")]
        public Int32 BookId { get; set; }

        /// <summary>格式</summary>
        public String Format { get; set; }

        /// <summary>公开状态 [1 - 公开, 0 - 私密]</summary>
        public Int32 Public { get; set; }

        /// <summary>状态 [1 - 正常, 0 - 草稿]</summary>
        public Int32 Status { get; set; }

        /// <summary>查看状态</summary>
        [DataMember(Name = "view_status")]
        public Int32 ViewStatus { get; set; }

        /// <summary>阅读状态</summary>
        [DataMember(Name = "read_status")]
        public Int32 ReadStatus { get; set; }

        /// <summary>喜欢数量</summary>
        [DataMember(Name = "likes_count")]
        public Int32 Likes { get; set; }

        /// <summary>阅读数量</summary>
        [DataMember(Name = "read_count")]
        public Int32 Reads { get; set; }

        /// <summary>评论数量</summary>
        [DataMember(Name = "comments_count")]
        public Int32 Comments { get; set; }

        /// <summary>内容更新时间</summary>
        [DataMember(Name = "content_updated_at")]
        public DateTime ContentUpdateTime { get; set; }

        /// <summary>创建时间</summary>
        [DataMember(Name = "created_at")]
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        [DataMember(Name = "updated_at")]
        public DateTime UpdateTime { get; set; }

        /// <summary>发布时间</summary>
        [DataMember(Name = "published_at")]
        public DateTime PublishTime { get; set; }

        /// <summary>首次发布时间</summary>
        [DataMember(Name = "first_published_at")]
        public DateTime FirstPublishTime { get; set; }

        /// <summary>草案版本</summary>
        [DataMember(Name = "draft_version")]
        public Int32 DraftVersion { get; set; }

        /// <summary>单词数</summary>
        [DataMember(Name = "word_count")]
        public Int32 WordCount { get; set; }

        /// <summary></summary>
        public String Cover { get; set; }

        /// <summary></summary>
        [DataMember(Name = "custom_description")]
        public String CustomDescription { get; set; }

        /// <summary>首次发布时间</summary>
        [DataMember(Name = "last_editor_id")]
        public Int32 LastEditorId { get; set; }

        /// <summary>最后编辑用户</summary>
        [DataMember(Name = "last_editor")]
        public UserDetail LastEditor { get; set; }

        /// <summary>知识库</summary>
        public BookDetail Book { get; set; }

        /// <summary>已重载。友好显示</summary>
        /// <returns></returns>
        public override String ToString() => Title;
    }
}