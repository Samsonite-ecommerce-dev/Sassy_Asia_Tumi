using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic
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
