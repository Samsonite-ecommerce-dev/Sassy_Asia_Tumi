using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;

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
