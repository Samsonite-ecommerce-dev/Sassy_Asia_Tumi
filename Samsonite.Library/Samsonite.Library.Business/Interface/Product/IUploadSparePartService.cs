using Samsonite.Library.Business.Models;
using Samsonite.Library.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business
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
