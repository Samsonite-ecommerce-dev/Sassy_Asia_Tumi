using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class SparePartQuerySearchRequest : PageRequest
    {
        public string SparePartKey { get; set; }

        public string Status { get; set; }
    }

    public class SparePartQueryEditRequest
    {
        public long ID { get; set; }

        public string ImageUrl { get; set; }
    }
}
