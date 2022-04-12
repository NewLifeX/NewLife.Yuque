using NewLife.Data;
using NewLife.YuQueWeb.Entity;

namespace NewLife.YuqueWeb.Models
{
    /// <summary>
    /// 知识库列表模型
    /// </summary>
    public class BookIndexModel
    {
        /// <summary>
        /// 知识库
        /// </summary>
        public Book Book { get; set; }

        /// <summary>
        /// 文档列表
        /// </summary>
        public IList<Document> Documents { get; set; }

        /// <summary>
        /// 分页
        /// </summary>
        public PageParameter Page { get; set; }
    }
}