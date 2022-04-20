using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class SparePartQuerySearchRequest : PageRequest
    {
        public string SparePartKey { get; set; }

        public int GroupID { get; set; }

        public string Status { get; set; }
    }

    public class SparePartQueryAddRequest
    {
        public long ID { get; set; }

        public string SparePartDesc { get; set; }

        public int GroupID { get; set; }

        public string ImageUrl { get; set; }
    }

    public class SparePartQueryEditRequest
    {
        public long ID { get; set; }

        public string SparePartDesc { get; set; }

        public int GroupID { get; set; }

        public string ImageUrl { get; set; }
    }
}
