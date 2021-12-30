using Samsonite.Library.Business.WorkService.Models;
using Samsonite.Library.Web.WorkService.Models;

namespace Samsonite.Library.Business.WorkService
{
    public interface ISAPService
    {
        /// <summary>
        /// 下载产品信息
        /// </summary>
        /// <returns></returns>
        CommonResult<SAPMaterialResponse> DownMaterial();

        /// <summary>
        /// 下载产品库存
        /// </summary>
        /// <returns></returns>
        CommonResult<SAPSparePartInventoryResponse> DownSparePartInventory();

        /// <summary>
        /// 下载产品价格
        /// </summary>
        /// <returns></returns>
        CommonResult<SAPSparePartPriceResponse> DownSparePartPrice();
    }
}
