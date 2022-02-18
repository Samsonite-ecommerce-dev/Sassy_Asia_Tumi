using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Basic
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
