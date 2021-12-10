using System.Text.Json.Serialization;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class ApiResponse
    {
        /// <summary>
        /// 请求处理单号
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestID { get; set; }

        /// <summary>
        /// 结果代码（100表示成功，其它表示失败）
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class ApiGetResponse : ApiResponse
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; }
    }

    /// <summary>
    /// WebApi的返回结果
    /// </summary>
    public class ApiPostResponse : ApiResponse
    {
        /// <summary>
        /// 返回成功数据
        /// </summary>
        [JsonPropertyName("successful_data")]
        public object SuccessfulData { get; set; }

        /// <summary>
        /// 返回失败数据
        /// </summary>
        [JsonPropertyName("fail_data")]
        public object FailData { get; set; }
    }

    /// <summary>
    /// API分页返回格式
    /// </summary>
    public class ApiPageResponse : ApiGetResponse
    {
        /// <summary>
        /// 每页数据大小
        /// </summary>
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页面
        /// </summary>
        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [JsonPropertyName("page_total")]
        public long PageTotal { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        [JsonPropertyName("total_record")]
        public long TotalRecord { get; set; }
    }
}
