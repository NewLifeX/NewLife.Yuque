using NewLife.Data;

namespace NewLife.YuqueWeb.Models
{
    /// <summary>
    /// 分页模型
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// 分页参数
        /// </summary>
        public PageParameter Page { get; set; }

        /// <summary>
        /// 基础Url
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// 获取分页Url
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public String GetUrl(Int32 pageIndex)
        {
            if (pageIndex <= 1)
                return Url.Replace("", null);
            else
                return Url.Replace("-pageIndex", "-" + pageIndex);
        }
    }
}