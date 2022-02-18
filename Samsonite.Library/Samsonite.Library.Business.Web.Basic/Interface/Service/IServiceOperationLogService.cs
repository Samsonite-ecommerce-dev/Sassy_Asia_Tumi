using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IServiceOperationLogService
    {
        /// <summary>
        /// 查询操作记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<ServiceOperationLogSearchResponse> GetQuery(ServiceOperationLogSearchRequest request);
    }
}
