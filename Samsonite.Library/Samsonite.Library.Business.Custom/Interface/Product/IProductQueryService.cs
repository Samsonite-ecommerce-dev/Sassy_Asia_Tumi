using Samsonite.Library.Business.Custom.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Custom
{
    public interface IProductQueryService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<View_ProductSparePart> GetQuery(ProductQuerySearchRequest request);

        /// <summary>
        /// 查询Line列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<ProductLine> GetLineQuery(ProductQuerySearchLineRequest request);

        /// <summary>
        /// 返回Color列表
        /// </summary>
        /// <returns></returns>
        List<ProductLineColor> GetColorQuery(ProductQuerySearchColorRequest request);

        /// <summary>
        /// 查询Size列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<ProductLineSize> GetSizeQuery(ProductQuerySearchSizeRequest request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Delete(int[] ids);

        /// <summary>
        /// 查询Group列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<GroupInfo> GetGroupQuery(ProductQuerySearchGroupRequest request);
    }
}
