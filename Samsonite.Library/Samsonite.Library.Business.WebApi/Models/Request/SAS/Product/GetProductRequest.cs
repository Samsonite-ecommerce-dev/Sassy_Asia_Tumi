using Samsonite.Library.Core.WebApi.Models;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetProductRequest : ApiRequest
    {
        public int PageSize { get; set; }

        public int CurrentPage { get; set; }  
    }
}
