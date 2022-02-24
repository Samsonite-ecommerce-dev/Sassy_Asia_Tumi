using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;

namespace Samsonite.Library.Business.Web.Custom
{
    public interface IProductsService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<Product> GetQuery(ProductsSearchRequest request);
    }
}
