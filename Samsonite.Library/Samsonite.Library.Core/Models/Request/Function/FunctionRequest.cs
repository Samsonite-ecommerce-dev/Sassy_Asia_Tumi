using Newtonsoft.Json;
using System;

namespace Samsonite.Library.Core.Models
{
    public class FunctionSearchRequest : PageRequest
    {
        public string Keyword { get; set; }

        public int GroupID { get; set; }
    }

    public class FunctionAddRequest
    {
        public string FuncName { get; set; }

        public int GroupID { get; set; }

        public Int16 FuncType { get; set; }

        public string FuncSign { get; set; }

        public string FuncUrl { get; set; }

        public string FuncPowers { get; set; }

        public string FuncTarget { get; set; }

        public bool IsShow { get; set; }

        public string FuncMemo { get; set; }
    }

    public class FunctionEditRequest
    {
        public int ID { get; set; }

        public string FuncName { get; set; }

        public int GroupID { get; set; }

        public Int16 FuncType { get; set; }

        public string FuncSign { get; set; }

        public string FuncUrl { get; set; }

        public string FuncPowers { get; set; }

        public string FuncTarget { get; set; }

        public bool IsShow { get; set; }

        public string FuncMemo { get; set; }
    }

    public class FuncPowerAttr
    {
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
