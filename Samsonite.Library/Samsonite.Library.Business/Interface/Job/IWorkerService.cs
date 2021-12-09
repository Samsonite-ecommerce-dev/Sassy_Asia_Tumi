using Samsonite.Library.Business.Models;
using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Business
{
    public interface IWorkerService
    {
        /// <summary>
        /// 从SAP下载产品信息
        /// </summary>
        /// <returns></returns>
        CommonResult<SAPMaterialResponse> DownMaterialFromSAP();

        /// <summary>
        /// 从SAP下载产品库存
        /// </summary>
        /// <returns></returns>
        CommonResult<SAPSparePartInventoryResponse> DownSparePartInventoryFromSAP();

        /// <summary>
        /// 从SAP下载产品价格
        /// </summary>
        /// <returns></returns>
        CommonResult<SAPSparePartPriceResponse> DownSparePartPriceFromSAP();
    }
}
