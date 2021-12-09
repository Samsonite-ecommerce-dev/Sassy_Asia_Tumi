using Samsonite.Library.Business.Models;
using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Business
{
    public class WorkerService : IWorkerService
    {
        private ISAPService _sAPService;
        public WorkerService(ISAPService sAPService)
        {
            _sAPService = sAPService;
        }

        /// <summary>
        /// 从SAP下载产品信息
        /// </summary>
        /// <returns></returns>
        public CommonResult<SAPMaterialResponse> DownMaterialFromSAP()
        {
            return _sAPService.DownMaterial();
        }

        /// <summary>
        /// 从SAP下载产品库存
        /// </summary>
        /// <returns></returns>
        public CommonResult<SAPSparePartInventoryResponse> DownSparePartInventoryFromSAP()
        {
            return _sAPService.DownSparePartInventory();
        }

        /// <summary>
        /// 从SAP下载产品价格
        /// </summary>
        /// <returns></returns>
        public CommonResult<SAPSparePartPriceResponse> DownSparePartPriceFromSAP()
        {
            return _sAPService.DownSparePartPrice();
        }
    }
}
