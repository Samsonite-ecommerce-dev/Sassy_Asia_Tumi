using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Business.Web.Basic
{
    public class ServiceOperationLogService : IServiceOperationLogService
    {
        private appEntities _appDB;
        public ServiceOperationLogService(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<ServiceOperationLogSearchResponse> GetQuery(ServiceOperationLogSearchRequest request)
        {
            QueryResponse<ServiceOperationLogSearchResponse> _result = new QueryResponse<ServiceOperationLogSearchResponse>();
            var _list = (from smj in _appDB.ServiceModuleJob
                         join smi in _appDB.ServiceModuleInfo on smj.ModuleID equals smi.ModuleID
                         select new
                         {
                             smi.ModuleID,
                             smi.ModuleTitle,
                             JobID = smj.ID,
                             smj.OperType,
                             smj.Status,
                             smj.StatusMessage,
                             smj.AddTime
                         }).AsQueryable();

            //搜索条件
            if (request.ModuleID > 0)
            {
                _list = _list.Where(p => p.ModuleID == request.ModuleID);
            }

            if (request.JobType > 0)
            {
                _list = _list.Where(p => p.OperType == request.JobType);
            }

            if (request.JobStatus > 0)
            {
                _list = _list.Where(p => p.Status == request.JobStatus-1);
            }

            if (!string.IsNullOrEmpty(request.Time1))
            {
                var _time1 = VariableHelper.SaferequestTime(request.Time1);
                _list = _list.Where(p => EF.Functions.DateDiffDay(p.AddTime, _time1) <= 0);
            }

            if (!string.IsNullOrEmpty(request.Time1))
            {
                var _time2 = VariableHelper.SaferequestTime(request.Time2);
                _list = _list.Where(p => EF.Functions.DateDiffDay(p.AddTime, _time2) >= 0);
            }

            List<ServiceOperationLogSearchResponse> _res = new List<ServiceOperationLogSearchResponse>();
            var _data = _list.AsNoTracking().OrderByDescending(p => p.JobID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            foreach (var item in _data)
            {
                _res.Add(new ServiceOperationLogSearchResponse()
                {
                    ModuleID = item.ModuleID,
                    ModuleTitle = item.ModuleTitle,
                    JobID = item.JobID,
                    OperType = item.OperType,
                    Status = item.Status,
                    StatusMessage = item.StatusMessage,
                    AddTime = item.AddTime
                });
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _res;
            return _result;
        }
    }
}
