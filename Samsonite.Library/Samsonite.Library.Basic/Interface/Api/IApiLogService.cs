using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;

namespace Samsonite.Library.Basic
{
    public interface IApiLogService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<WebApiAccessLog> GetQuery(ApiLogSearchRequest request);
    }
}
