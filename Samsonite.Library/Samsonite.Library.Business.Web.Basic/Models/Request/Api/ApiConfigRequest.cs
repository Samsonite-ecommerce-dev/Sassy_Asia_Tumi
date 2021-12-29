using Newtonsoft.Json;
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
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
