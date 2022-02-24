﻿using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IEmailGroupConfigService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<SendMailGroup> GetQuery(EmailGroupConfigSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(EmailGroupConfigAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(EmailGroupConfigEditRequest request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Delete(int[] ids);
    }
}
