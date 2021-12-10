using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Linq;

namespace Samsonite.Library.Business.Basic
{
    public class ApiLogService : IApiLogService
    {
        private IAppLogService _appLogService;
        private logEntities _logDB;
        public ApiLogService(IAppLogService appLogService, logEntities logEntities)
        {
            _appLogService = appLogService;
            _logDB = logEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<WebApiAccessLog> GetQuery(ApiLogSearchRequest request)
        {
            QueryResponse<WebApiAccessLog> _result = new QueryResponse<WebApiAccessLog>();
            var _list = _logDB.WebApiAccessLog.AsQueryable();

            if (request.LogType>0)
            {
                _list = _list.Where(p => p.LogType== request.LogType);
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.Ip.Contains(request.Keyword) || p.Url.Contains(request.Keyword));
            }

            if (request.State > 0)
            {
                _list = _list.Where(p => p.State == (request.State == 2));
            }

            if (!string.IsNullOrEmpty(request.Time1))
            {
                var _time1 = VariableHelper.SaferequestTime(request.Time1);
                _list = _list.Where(p => EF.Functions.DateDiffDay(p.CreateTime, _time1) <= 0);
            }

            if (!string.IsNullOrEmpty(request.Time2))
            {
                var _time2 = VariableHelper.SaferequestTime(request.Time2);
                _list = _list.Where(p => EF.Functions.DateDiffDay(p.CreateTime, _time2) >= 0);
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderByDescending(p => p.id).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }
    }
}
