using Samsonite.Library.Bussness.WebApi.Models;

namespace Samsonite.Library.Bussness.WebApi
{
    public interface IProductService
    {
        /// <summary>
        /// 获取产品集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetProductResponse GetProductQuery(GetProductRequest request);
    }
}
