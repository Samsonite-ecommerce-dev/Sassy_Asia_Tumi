using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;

namespace Samsonite.Library.Core
{
    public interface ISystemLogService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<WebAppErrorLog> GetQuery(SystemLogSearchRequest request);
    }
}
