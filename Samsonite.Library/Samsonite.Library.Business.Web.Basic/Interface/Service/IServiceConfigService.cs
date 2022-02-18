using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IServiceConfigService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<ServiceModuleInfo> GetQuery(ServiceConfigSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(ServiceConfigAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(ServiceConfigEditRequest request);

        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Oper(ServiceConfigOperRequest request);

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse OperDelete(long[] ids);

        /// <summary>
        /// 返回服务集合
        /// </summary>
        /// <returns></returns>
        List<ServiceModuleInfo> GetModuleObject();
    }
}
