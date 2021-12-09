using Samsonite.Library.Business.Models;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;

namespace Samsonite.Library.Business
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
