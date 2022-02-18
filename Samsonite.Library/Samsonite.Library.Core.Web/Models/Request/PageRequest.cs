namespace Samsonite.Library.Core.Web.Models
{
    public class PageRequest
    {
        /// <summary>
        /// 每页数量
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Prop { get; set; }

        /// <summary>
        /// 排序类型asc/desc
        /// </summary>
        public string Order { get; set; }
    }
}
