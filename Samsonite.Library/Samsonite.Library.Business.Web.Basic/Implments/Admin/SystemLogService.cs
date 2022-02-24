using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Linq;

namespace Samsonite.Library.Business.Web.Basic
{
    public class SystemLogService : ISystemLogService
    {
        private logEntities _logDB;
        public SystemLogService(IAppLogService appLogService, logEntities logEntities)
        {
            _logDB = logEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<WebAppErrorLog> GetQuery(SystemLogSearchRequest request)
        {
            QueryResponse<WebAppErrorLog> _result = new QueryResponse<WebAppErrorLog>();
            var _list = _logDB.WebAppErrorLog.AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.UserIP.Contains(request.Keyword) || p.LogMessage.Contains(request.Keyword));
            }

            if (!string.IsNullOrEmpty(request.Time1))
            {
                var _time1 = VariableHelper.SaferequestTime(request.Time1);
                _list = _list.Where(p => EF.Functions.DateDiffDay(p.AddTime, _time1) <= 0);
            }

            if (!string.IsNullOrEmpty(request.Time2))
            {
                var _time2 = VariableHelper.SaferequestTime(request.Time2);
                _list = _list.Where(p => EF.Functions.DateDiffDay(p.AddTime, _time2) >= 0);
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderByDescending(p => p.LogID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }
    }
}
