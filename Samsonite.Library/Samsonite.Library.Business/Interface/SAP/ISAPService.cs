using Samsonite.Library.Business.Models;
using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Business
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
