using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class ProductQuerySearchRequest : PageRequest
    {
        public int SearchType { get; set; }

        public string Keyword { get; set; }
    }

    public class ProductQuerySearchLineRequest : PageRequest
    {
        public string SearchKey { get; set; }
    }

    public class ProductQuerySearchSizeRequest
    {
        public string LineID { get; set; }
    }

    public class ProductQuerySearchColorRequest
    {
        public string LineID { get; set; }
    }

    public class ProductQuerySearchGroupRequest
    {
        public string LineID { get; set; }

        public string SizeID { get; set; }
    }
}
