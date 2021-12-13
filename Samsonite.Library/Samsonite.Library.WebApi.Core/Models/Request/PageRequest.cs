using System.Text.Json.Serialization;

namespace Samsonite.Library.WebApi.Core.Models
{
    public class PageRequest
    {
        /// <summary>
        /// 记录总数
        /// </summary>
        [JsonPropertyName("total_record")]
        public long TotalRecord { get; set; }
    }
}
