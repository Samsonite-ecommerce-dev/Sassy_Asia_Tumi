using System.Text.Json.Serialization;

namespace Samsonite.Library.WebApi.Core.Models
{
    public class PageResponse
    {
        /// <summary>
        /// 记录总数
        /// </summary>
        [JsonPropertyName("total_record")]
        public long TotalRecord { get; set; }
    }
}
