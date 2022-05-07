﻿using Samsonite.Library.Core.Web.Models;
using System.Threading.Tasks;

namespace Samsonite.Library.Core.Web
{
    public interface IUploadService
    {
        /// <summary>
        /// 查询文件列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<UploadFileCollection> GetQuery(UploadSearchRequest request);

        /// <summary>
        /// 通用保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<UploadSaveResponse> SaveFileAsync(UploadSaveRequest request);
    }
}
