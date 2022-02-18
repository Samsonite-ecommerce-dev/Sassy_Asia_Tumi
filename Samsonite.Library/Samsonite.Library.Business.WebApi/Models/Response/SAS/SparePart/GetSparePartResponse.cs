using Samsonite.Library.Core.WebApi.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartResponse:PageResponse
    {
        [JsonPropertyName("data")]
        public List<SparePartInfo> Data { get; set; }
    }

    public class SparePartInfo
    {
        [JsonPropertyName("sparepart_id")]
        public long SparePartID { get; set; }

        [JsonPropertyName("sparepart_description")]
        public string SpartPartDesc { get; set; }

        [JsonPropertyName("sparepart_imageurl")]
        public string SpartPartImage { get; set; }

        [JsonPropertyName("groupid")]
        public int GroupID { get; set; }

        [JsonPropertyName("group_description")]
        public string GroupDesc { get; set; }

        [JsonPropertyName("base_price")]
        public decimal BasePrice { get; set; }

        [JsonPropertyName("unit")]
        public string UnitofMeasure { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
