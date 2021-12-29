using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Web.Custom
{
    public interface IUploadSparePartService
    {

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<UploadSparePartImportResponse> Import(UploadSparePartImportRequest request);

        /// <summary>
        /// 保存导入Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<UploadSparePartImportResponse> ImportSave(UploadSparePartImportRequest request);
    }
}
