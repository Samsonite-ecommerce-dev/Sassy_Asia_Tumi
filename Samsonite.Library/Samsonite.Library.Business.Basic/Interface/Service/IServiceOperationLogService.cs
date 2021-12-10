using Samsonite.Library.Business.Basic.Models;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Basic
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
