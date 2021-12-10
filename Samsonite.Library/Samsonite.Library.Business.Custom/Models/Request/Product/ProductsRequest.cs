using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Custom.Models
{
    public class ProductsSearchRequest : PageRequest
    {
        public int SearchType { get; set; }

        public string Keyword { get; set; }
    }
}
