using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Linq;

namespace Samsonite.Library.Business.Basic
{
    public class ServiceLogService : IServiceLogService
    {
        private logEntities _logDB;
        public ServiceLogService(IAppLogService appLogService, logEntities logEntities)
        {
            _logDB = logEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<ServiceLog> GetQuery(ServiceLogSearchRequest request)
        {
            QueryResponse<ServiceLog> _result = new QueryResponse<ServiceLog>();
            var _list = _logDB.ServiceLog.AsQueryable();

            //搜索条件
            if (request.Type > 0)
            {
                _list = _list.Where(p => p.LogType == request.Type);
            }

            if (request.Level > 0)
            {
                _list = _list.Where(p => p.LogLevel == request.Level);
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.LogMessage.Contains(request.Keyword));
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
            _result.Items = _list.OrderByDescending(p => p.ID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        ///Info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        public void Info(object message, int serviceId)
        {
            InsertLog(message, serviceId, LogLevel.Info);
        }

        /// <summary>
        ///Warn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        public void Warn(object message, int serviceId)
        {
            InsertLog(message, serviceId, LogLevel.Warning);
        }

        /// <summary>
        ///Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        public void Error(object message, int serviceId)
        {
            InsertLog(message, serviceId, LogLevel.Error);
        }

        /// <summary>
        ///Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serviceId"></param>
        public void Debug(object message, int serviceId)
        {
            InsertLog(message, serviceId, LogLevel.Debug);
        }

        private void InsertLog(object data, int logType, LogLevel logLevel)
        {
            _logDB.ServiceLog.Add(new ServiceLog
            {
                LogType = logType,
                LogLevel = (int)logLevel,
                CreateTime = DateTime.Now,
                LogMessage = JsonHelper.JsonSerialize(data),
            });
            _logDB.SaveChanges();
        }
    }
}
