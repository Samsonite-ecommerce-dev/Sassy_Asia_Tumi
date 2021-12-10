using Newtonsoft.Json;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Basic.Models
{
    public class LanguageSearchRequest : PageRequest
    {
        public string Keyword { get; set; }

        public int TypeID { get; set; }

        public int FunctionID { get; set; }

        public int IsDelete { get; set; }
    }

    public class LanguageAddRequest
    {
        public int FunctionID { get; set; }

        public int KeyID { get; set; }

        public string packKeys { get; set; }
    }

    public class LanguageEditRequest
    {
        public int ID { get; set; }

        public string LanguageKey { get; set; }

        public string LanguageValue { get; set; }
    }

    public class LanguageSortRequest
    {
        public int ID { get; set; }

        public string Type { get; set; }
    }

    public class LanguageSearchByKeyRequest
    {
        public string Key { get; set; }

        public int FunctionID { get; set; }
    }

    public class LanguagePackAttr
    {
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string PackKey { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public List<LanguagePackValueAttr> Languages { get; set; }
    }

    public class LanguagePackValueAttr
    {
        [JsonProperty(PropertyName = "id")]
        public int LanguageTypeID { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string LanguageName { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string LanguageValue { get; set; }
    }
}
