using System.Runtime.Serialization;

namespace NewLife.Yuque.Models
{
    /// <summary>
    /// 文档详细信息
    /// </summary>
    public class DocumentDetail
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>文档路径</summary>
        public String Slug { get; set; }

        /// <summary>标题</summary>
        public String Title { get; set; }

        /// <summary>仓库</summary>
        [DataMember(Name = "book_id")]
        public Int32 BookId { get; set; }

        /// <summary>知识库</summary>
        public BookDetail Book { get; set; }

        /// <summary>用户</summary>
        [DataMember(Name = "user_id")]
        public Int32 UserId { get; set; }

        /// <summary>创建者</summary>
        public UserDetail Creator { get; set; }

        /// <summary>描述了正文的格式 [lake , markdown]</summary>
        public String Format { get; set; }

        /// <summary>正文 Markdown 源代码</summary>
        public String Body { get; set; }

        /// <summary>草稿 Markdown 源代码</summary>
        [DataMember(Name = "body_draft")]
        public String BodyDraft { get; set; }

        /// <summary>转换过后的正文 HTML</summary>
        /// <remarks>https://www.yuque.com/yuque/developer/yr938f</remarks>
        [DataMember(Name = "body_html")]
        public String BodyHtml { get; set; }

        /// <summary>语雀 lake 格式的文档内容</summary>
        [DataMember(Name = "body_lake")]
        public String BodyLake { get; set; }

        /// <summary>语雀 lake 格式的草稿内容</summary>
        [DataMember(Name = "body_draft_lake")]
        public String BodyDraftLake { get; set; }

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

        /// <summary>点赞数量</summary>
        [DataMember(Name = "likes_count")]
        public Int32 Likes { get; set; }

        /// <summary>评论数量</summary>
        [DataMember(Name = "comments_count")]
        public Int32 Comments { get; set; }

        /// <summary>内容更新时间</summary>
        [DataMember(Name = "content_updated_at")]
        public DateTime ContentUpdateTime { get; set; }

        /// <summary>删除时间。未删除为0001-01-01</summary>
        [DataMember(Name = "deleted_at")]
        public DateTime DeleteTime { get; set; }

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

        /// <summary>单词数</summary>
        [DataMember(Name = "word_count")]
        public Int32 WordCount { get; set; }

        /// <summary>封面</summary>
        public String Cover { get; set; }

        /// <summary></summary>
        public String Description { get; set; }

        /// <summary></summary>
        [DataMember(Name = "custom_description")]
        public String CustomDescription { get; set; }

        /// <summary></summary>
        public Int32 Hits { get; set; }

        /// <summary>已重载。友好显示</summary>
        /// <returns></returns>
        public override String ToString() => Title;
    }
}