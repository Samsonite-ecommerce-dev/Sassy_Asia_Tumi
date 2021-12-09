using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Core
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
