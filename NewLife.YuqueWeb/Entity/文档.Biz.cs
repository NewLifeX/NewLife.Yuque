using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using XCode;
using XCode.Membership;

namespace NewLife.YuqueWeb.Entity
{
    /// <summary>文档。文档内容</summary>
    public partial class Document : Entity<Document>
    {
        #region 对象操作
        static Document()
        {
            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            var df = Meta.Factory.AdditionalFields;
            df.Add(nameof(LocalHits));

            // 过滤器 UserModule、TimeModule、IPModule
            Meta.Modules.Add<UserModule>();
            //Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();
        }

        /// <summary>验证并修补数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            //// 这里验证参数范围，建议抛出参数异常，指定参数名，前端用户界面可以捕获参数异常并聚焦到对应的参数输入框
            //if (Title.IsNullOrEmpty()) throw new ArgumentNullException(nameof(Title), "标题不能为空！");

            // 建议先调用基类方法，基类方法会做一些统一处理
            base.Valid(isNew);

            TotalHits = Hits + LocalHits;
        }
        #endregion

        #region 扩展属性
        /// <summary>知识库</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        public Book Book => Extends.Get(nameof(Book), k => Book.FindById(BookId));

        /// <summary>知识库</summary>
        [Map(nameof(BookId), typeof(Book), "Id")]
        public String BookName => Book?.Name;

        /// <summary>知识库</summary>
        [XmlIgnore, IgnoreDataMember]
        public String BookCode => Book?.Code;
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static Document FindById(Int32 id)
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
        public static Document FindByCode(String code)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Code.EqualIgnoreCase(code));

            return Find(_.Code == code);
        }

        /// <summary>根据知识库查找</summary>
        /// <param name="bookId">知识库</param>
        /// <returns>实体列表</returns>
        public static IList<Document> FindAllByBookId(Int32 bookId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.BookId == bookId);

            return FindAll(_.BookId == bookId);
        }

        /// <summary>根据标题查找</summary>
        /// <param name="title">标题</param>
        /// <returns>实体列表</returns>
        public static IList<Document> FindAllByTitle(String title)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Title.EqualIgnoreCase(title));

            return FindAll(_.Title == title);
        }

        /// <summary>
        /// 根据知识库和编码查找
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Document FindByBookAndCode(Int32 bookId, String code)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.BookId == bookId && e.Code.EqualIgnoreCase(code));

            return Find(_.BookId == bookId & _.Code == code);
        }

        public static Document FindByBookAndSlug(Int32 bookId, String slug)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.BookId == bookId && e.Slug.EqualIgnoreCase(slug));

            return Find(_.BookId == bookId & _.Slug == slug);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="code">编码。路径唯一标识，默认取Slug</param>
        /// <param name="bookId">知识库</param>
        /// <param name="enable">是否启用</param>
        /// <param name="pub">是否公开</param>
        /// <param name="status">正式或草稿</param>
        /// <param name="start">更新时间开始</param>
        /// <param name="end">更新时间结束</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<Document> Search(String code, Int32 bookId, Boolean? enable, Boolean? pub, Boolean? status, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (!code.IsNullOrEmpty()) exp &= _.Code == code;
            if (bookId >= 0) exp &= _.BookId == bookId;
            if (enable != null) exp &= _.Enable == enable;
            if (pub != null) exp &= _.Public == pub;
            if (status != null) exp &= _.Status == status;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.Code == key | _.Title.Contains(key) | _.UserName.Contains(key) | _.Format == key | _.Html.Contains(key) | _.Cover.Contains(key) | _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }

        public static IList<Document> SearchByUpdateTime(DateTime start, DateTime end, PageParameter page)
        {
            var exp = new WhereExpression();

            exp &= _.UpdateTime.Between(start, end);

            return FindAll(exp, page);
        }

        public static IList<Document> SearchBySyncTime(DateTime end, PageParameter page)
        {
            var exp = new WhereExpression();

            exp &= _.SyncTime < end | _.SyncTime.IsNull();

            return FindAll(exp, page);
        }
        #endregion

        #region 业务操作
        public void Fill(NewLife.Yuque.Models.Document detail)
        {
            var doc = this;

            //doc.Id = detail.Id;
            if (doc.Code.IsNullOrEmpty()) doc.Code = detail.Slug;

            // 未正式公开时，允许修改Code
            if (detail.Status == 0) doc.Code = detail.Slug;

            doc.Title = detail.Title;
            doc.Slug = detail.Slug;
            doc.BookId = detail.BookId;
            doc.Public = detail.Public > 0;
            doc.Status = detail.Status > 0;

            doc.UserName = detail.LastEditor?.Name;
            doc.Format = detail.Format;
            //doc.Hits = item.Hits;
            doc.Likes = detail.Likes;
            doc.Reads = detail.Reads;
            doc.Comments = detail.Comments;
            doc.WordCount = detail.WordCount;
            if (!detail.Cover.IsNullOrEmpty()) doc.Cover = detail.Cover;
            doc.Remark = detail.Description;

            doc.DraftVersion = detail.DraftVersion;
            doc.ContentUpdateTime = detail.ContentUpdateTime;
            //doc.SyncTime = DateTime.Now;
            doc.PublishTime = detail.PublishTime;
            doc.FirstPublishTime = detail.FirstPublishTime;
            doc.CreateTime = detail.CreateTime;
            doc.UpdateTime = detail.UpdateTime;
        }

        public void Fill(NewLife.Yuque.Models.DocumentDetail detail)
        {
            var doc = this;

            //doc.Id = detail.Id;
            if (doc.Code.IsNullOrEmpty()) doc.Code = detail.Slug;

            // 未正式公开时，允许修改Code
            if (detail.Status == 0) doc.Code = detail.Slug;

            doc.Title = detail.Title;
            doc.Slug = detail.Slug;
            doc.BookId = detail.BookId;

            doc.UserName = detail.Creator?.Name;
            doc.Format = detail.Format;
            doc.Public = detail.Public > 0;
            doc.Status = detail.Status > 0;

            doc.Body = detail.Body;
            doc.BodyHtml = detail.BodyHtml;
            doc.ContentUpdateTime = detail.ContentUpdateTime;

            doc.Hits = detail.Hits;
            //doc.Reads = detail.Reads;
            doc.Likes = detail.Likes;
            doc.Comments = detail.Comments;
            doc.WordCount = detail.WordCount;

            if (!detail.Cover.IsNullOrEmpty()) doc.Cover = detail.Cover;
            doc.Remark = detail.Description;

            doc.PublishTime = detail.PublishTime;
            doc.FirstPublishTime = detail.FirstPublishTime;
            doc.CreateTime = detail.CreateTime;
            doc.UpdateTime = detail.UpdateTime;

            if (detail.DeleteTime.Year > 2000) doc.Enable = false;
        }
        #endregion
    }
}