using Newtonsoft.Json;

namespace Samsonite.Library.Basic.Models
{
    public class EmailGroupConfigSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class EmailGroupConfigAddRequest
    {
        public string GroupName { get; set; }

        public string MailAddresses { get; set; }

        public string Remark { get; set; }
    }

    public class EmailGroupConfigEditRequest
    {
        public int ID { get; set; }

        public string GroupName { get; set; }

        public string MailAddresses { get; set; }

        public string Remark { get; set; }
    }

    public class MailAddressesAttr
    {
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
