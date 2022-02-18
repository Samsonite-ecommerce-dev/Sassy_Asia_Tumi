using Samsonite.Library.Core.WebApi.Models;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartRelatedRequest : ApiRequest
    {
        public string Material { get; set; }

        public string Grid { get; set; }

        public string Sku { get; set; }

        public int GroupID { get; set; }
    }
}
