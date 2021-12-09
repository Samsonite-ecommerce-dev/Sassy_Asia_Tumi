using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Business.Models
{
    public class ProductsSearchRequest : PageRequest
    {
        public int SearchType { get; set; }

        public string Keyword { get; set; }
    }
}
