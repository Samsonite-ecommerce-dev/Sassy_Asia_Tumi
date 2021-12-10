using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartResponse
    {
        [JsonPropertyName("data")]
        public List<SkuRelated> Data { get; set; }
    }

    public class SkuRelated
    {
        [JsonPropertyName("sku")]

        public string Sku { get; set; }

        [JsonPropertyName("spareparts")]
        public List<SparePartInfo> SpareParts { get; set; }
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
    }
}
