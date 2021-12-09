using System.Collections.Generic;

namespace Samsonite.Library.Core.Models
{
    public class QueryResponse<T>
    {
        /// <summary>
        /// 返回数据记录总数
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public List<T> Items { get; set; }
    }

    public class PostResponse
    {
        public bool Result { get; set; }

        public string Message { get; set; }
    }

    public class ErrorResponse
    {
        public bool IsError { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
