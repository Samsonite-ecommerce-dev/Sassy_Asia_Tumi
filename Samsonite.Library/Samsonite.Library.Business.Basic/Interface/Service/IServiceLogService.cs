using Samsonite.Library.Business.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Basic
{
    public interface IServiceLogService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<ServiceLog> GetQuery(ServiceLogSearchRequest request);

        /// <summary>
        ///Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        void Debug(object message, int serviceId);

        /// <summary>
        ///Warn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        void Warn(object message, int serviceId);

        /// <summary>
        ///Info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        void Info(object message, int serviceId);

        /// <summary>
        ///Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        void Error(object message, int serviceId);
    }
}
