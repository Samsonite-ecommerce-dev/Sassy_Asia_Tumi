using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class SparePartGroupSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class SparePartGroupEditRequest
    {
        public long ID { get; set; }

        public string GroupDescription { get; set; }

        public string GroupText { get; set; }
    }
}
