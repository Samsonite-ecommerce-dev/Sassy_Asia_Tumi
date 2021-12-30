using System.Text.Json.Serialization;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class ApiConfigSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class ApiConfigAddRequest
    {
        public string AppID { get; set; }

        public string Token { get; set; }

        public string Ips { get; set; }

        public string Interfaces { get; set; }

        public string Remark { get; set; }

        public bool IsUsed { get; set; }
    }

    public class ApiConfigEditRequest
    {
        public int ID { get; set; }

        public string AppID { get; set; }

        public string Token { get; set; }

        public string Ips { get; set; }

        public string Interfaces { get; set; }

        public string Remark { get; set; }

        public bool IsUsed { get; set; }
    }

    public class IpsAttr
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
