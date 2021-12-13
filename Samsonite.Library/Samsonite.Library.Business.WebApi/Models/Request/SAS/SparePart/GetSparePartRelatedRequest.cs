using Samsonite.Library.WebApi.Core.Models;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartRelatedRequest : ApiRequest
    {
        public string Sku { get; set; }

        public int GroupID { get; set; }
    }
}
