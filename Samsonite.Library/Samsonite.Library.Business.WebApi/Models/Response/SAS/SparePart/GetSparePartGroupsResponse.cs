using Samsonite.Library.WebApi.Core.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartGroupsResponse
    {
        [JsonPropertyName("data")]
        public List<SparePartGroupInfo> Data { get; set; }
    }

    public class SparePartGroupInfo
    {
        [JsonPropertyName("group_id")]
        public long GroupID { get; set; }

        [JsonPropertyName("group_description")]
        public string GroupDesc { get; set; }
    }
}
