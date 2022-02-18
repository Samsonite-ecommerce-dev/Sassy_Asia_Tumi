using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Samsonite.Library.Business.Web.Basic.Models
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
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("value")]
        public string PackKey { get; set; }

        [JsonPropertyName("languages")]
        public List<LanguagePackValueAttr> Languages { get; set; }
    }

    public class LanguagePackValueAttr
    {
        [JsonPropertyName("id")]
        public int LanguageTypeID { get; set; }

        [JsonPropertyName("label")]
        public string LanguageName { get; set; }

        [JsonPropertyName("value")]
        public string LanguageValue { get; set; }
    }
}
