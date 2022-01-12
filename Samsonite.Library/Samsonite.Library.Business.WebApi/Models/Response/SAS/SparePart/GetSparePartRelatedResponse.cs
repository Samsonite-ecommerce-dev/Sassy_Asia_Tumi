using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartRelatedResponse
    {
        [JsonPropertyName("data")]
        public List<SkuRelated> Data { get; set; }
    }

    public class SkuRelated
    {
        [JsonPropertyName("material")]

        public string Material { get; set; }

        [JsonPropertyName("grid")]

        public string Grid { get; set; }

        [JsonPropertyName("sku")]

        public string Sku { get; set; }

        [JsonPropertyName("spareparts")]
        public List<SparePartInfo> SpareParts { get; set; }
    }
}
