using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class ProductsSearchRequest : PageRequest
    {
        public int SearchType { get; set; }

        public string Keyword { get; set; }
    }
}
