using System.Text.Json.Serialization;
using Samsonite.Library.Core.Web.Models;
using System;

namespace Samsonite.Library.Business.Web.Basic.Models
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
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
