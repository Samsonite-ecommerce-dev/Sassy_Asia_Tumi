using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Business.Models
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
