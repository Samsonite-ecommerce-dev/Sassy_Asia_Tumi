using Samsonite.Library.Core.WebApi.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetProductResponse: PageResponse
    {
        [JsonPropertyName("data")]
        public List<ProductInfo> Data { get; set; }
    }

    public class ProductInfo
    {
        [JsonPropertyName("sku")]
        public string Sku { get; set; }

        [JsonPropertyName("material")]
        public string Material { get; set; }

        [JsonPropertyName("material_description")]
        public string MaterialDesc { get; set; }

        [JsonPropertyName("grid")]
        public string Grid { get; set; }

        [JsonPropertyName("grid_description")]
        public string GridDesc { get; set; }

        [JsonPropertyName("material_Group")]
        public string MaterialGroup { get; set; }

        [JsonPropertyName("collection")]
        public string Collection { get; set; }
    }
}
