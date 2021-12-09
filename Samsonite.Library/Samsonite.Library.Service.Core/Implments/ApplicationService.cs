using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Service.Core.Enum;
using Samsonite.Library.Service.Core.Interface;
using Samsonite.Library.Service.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Service.Core.Implments
{

    public class ApplicationService : IApplicationService
    {
        private appEntities _appDB;
        private logEntities _logDB;
        public ApplicationService(appEntities appEntities, logEntities logEntities)
        {
            _appDB = appEntities;
            _logDB = logEntities;
        }

        #region 应用线程
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        public BaseModel InitBase<T>()
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            string _assemblyName = typeof(T).Name;
            return new BaseModel()
            {
                //10秒检测一次
                LoopTime = Config.ThreadIntervalTime,
                CurrentErrorTimes = 0,
                MaxErrorTimes = Config.MaxErrorTimes,
                AssemblyName = _assemblyName,
                ThreadName = $"{Config.ThreadPrefix}_thread_{_assemblyName}",
                CurrentStatus = ServiceStatus.Stop,
                CurrentJobID = 0
            };
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <returns></returns>
        public ServiceModel InitService<T>()
        {
            ServiceModel _result = new ServiceModel();
            string _assemblyName = typeof(T).Name.ToUpper();
            try
            {
                ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleType.ToUpper() == _assemblyName).SingleOrDefault();
                if (objServiceModuleInfo != null)
                {
                    _result.ServiceID = objServiceModuleInfo.ModuleID;
                    _result.WorkflowID = objServiceModuleInfo.ModuleWorkflowID;
                    _result.RunType = objServiceModuleInfo.FixType;
                    _result.RunTime = objServiceModuleInfo.FixTime;
                    _result.IsNotify = objServiceModuleInfo.IsNotify;
                    //开启服务,默认先执行一次
                    if (objServiceModuleInfo.FixType == 1)
                    {
                        _result.NextRunTime = DateTime.Now;
                    }
                    else
                    {
                        //读取时间集合
                        string[] _Times = _result.RunTime.Split(',');
                        List<DateTime> _FixTimes = new List<DateTime>();
                        foreach (string _ts in _Times)
                        {
                            _FixTimes.Add(Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + _ts));
                        }
                        //按时间从小到大排序
                        _FixTimes = _FixTimes.OrderBy(p => p.Ticks).ToList();
                        _result.NextRunTime = _FixTimes.Where(p => p.Ticks <= DateTime.Now.Ticks).OrderByDescending(p => p.Ticks).FirstOrDefault();
                        if (_result.NextRunTime == default(DateTime))
                        {
                            _result.NextRunTime = _FixTimes[0];
                        }
                    }
                    _result.LastRunTime = _result.NextRunTime;
                    //更新下次执行时间
                    objServiceModuleInfo.NextRunTime = _result.NextRunTime;
                    _appDB.SaveChanges();
                }
                else
                {
                    throw new Exception("Configuration information read failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        /// <summary>
        /// 计算下次执行时间
        /// </summary>
        /// <param name="serviceConfig"></param>
        /// <returns></returns>
        public ServiceModel CalculationNextTime(ServiceModel serviceConfig)
        {
            //记录上次执行时间
            serviceConfig.LastRunTime = DateTime.Now;
            //计算下次执行时间
            if (serviceConfig.RunType == 1)
            {
                serviceConfig.NextRunTime = DateTime.Now.AddSeconds(Convert.ToInt32(serviceConfig.RunTime));
            }
            else
            {
                //读取时间集合
                string[] _Times = serviceConfig.RunTime.Split(',');
                List<DateTime> _FixTimes = new List<DateTime>();
                foreach (string _ts in _Times)
                {
                    _FixTimes.Add(Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + _ts));
                }
                //按时间从小到大排序
                _FixTimes = _FixTimes.OrderBy(p => p.Ticks).ToList();
                serviceConfig.NextRunTime = _FixTimes.Where(p => p.Ticks > serviceConfig.NextRunTime.Ticks).OrderBy(p => p.Ticks).FirstOrDefault();
                if (serviceConfig.NextRunTime == default(DateTime))
                {
                    serviceConfig.NextRunTime = _FixTimes[0].AddDays(1);
                }
            }
            //更新下次执行时间
            this.UpdateNextRunTime(serviceConfig);

            return serviceConfig;
        }

        /// <summary>
        /// 主任务
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="serviceConfig"></param>
        /// <param name="actionFunction"></param>
        public void ThreadMethod(BaseModel baseModel, ServiceModel serviceConfig, Action actionFunction)
        {
            try
            {
                if (DateTime.Compare(DateTime.Now, serviceConfig.NextRunTime) >= 0)
                {
                    actionFunction.Invoke();
                }
            }
            catch (Exception ex)
            {
                baseModel.CurrentErrorTimes++;
                if (baseModel.CurrentErrorTimes < baseModel.MaxErrorTimes)
                {
                    //写入错误日志
                    this.DBLogError(serviceConfig.ServiceID, $" Error Times:{baseModel.CurrentErrorTimes},Error Message：{Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace}.");
                }
                else
                {
                    //计算下次执行时间
                    this.CalculationNextTime(serviceConfig);
                    //写入错误日志
                    this.DBLogError(serviceConfig.ServiceID, $" Error Times:{baseModel.CurrentErrorTimes},Error Message：{Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace},Next Time:{serviceConfig.NextRunTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    //重置错误时间
                    baseModel.CurrentErrorTimes = 0;

                    ////3次执行失败邮件提醒
                    //if (objServiceConfig.IsNotify)
                    //{
                    //    NotificationService.SendServiceModuleNotification(objServiceConfig.WorkflowID, AppNotificationLevel.Error, ex.ToString());
                    //}
                }
            }
        }
        #endregion

        #region 应用操作
        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <param name="isStop"></param>
        /// <param name="isPause"></param>
        public ServiceStatus GetCurrentStatus(bool isStop, bool isPause)
        {
            if (isStop)
            {
                return ServiceStatus.Stop;
            }
            else
            {
                if (isPause)
                {
                    return ServiceStatus.Pause;
                }
                else
                {
                    return ServiceStatus.Runing;
                }
            }
        }

        /// <summary>
        /// 设置服务状态
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="serviceConfig"></param>
        /// <param name="status"></param>
        public void SetServiceModuleStatus(BaseModel baseModel, ServiceModel serviceConfig, ServiceStatus status)
        {
            try
            {
                if (status != baseModel.CurrentStatus)
                {
                    int result = _appDB.Database.ExecuteSqlRaw("Update ServiceModuleInfo set Status={1} where ModuleID={0}", serviceConfig.ServiceID, (int)status);
                    if (result > 0)
                    {
                        baseModel.CurrentStatus = status;
                    }
                }
            }
            catch { };
        }

        /// <summary>
        /// 设置工作流ID状态
        /// </summary>
        /// <param name="baseModel"></param>
        public void CompleteModuleJob(BaseModel baseModel)
        {
            try
            {
                if (baseModel.CurrentJobID > 0)
                {
                    int result = _appDB.Database.ExecuteSqlRaw("Update ServiceModuleJob set Status={1} where ID={0}", baseModel.CurrentJobID, (int)JobStatus.Success);
                    if (result > 0)
                    {
                        baseModel.CurrentJobID = 0;
                    }
                }
            }
            catch { };
        }

        /// <summary>
        /// 更新下次执行时间
        /// </summary>
        /// <param name="serviceConfig"></param>
        private void UpdateNextRunTime(ServiceModel serviceConfig)
        {
            try
            {
                _appDB.Database.ExecuteSqlRaw("update ServiceModuleInfo set NextRunTime={1} where ModuleID={0}", serviceConfig.ServiceID, serviceConfig.NextRunTime);
            }
            catch { }
        }

        /// <summary>
        /// 清空下次执行时间
        /// </summary>
        /// <param name="serviceConfig"></param>
        public void ClearNextRunTime(ServiceModel serviceConfig)
        {
            try
            {
                _appDB.Database.ExecuteSqlRaw("update ServiceModuleInfo set NextRunTime=NULL where ModuleID={0}", serviceConfig.ServiceID);
            }
            catch { }
        }
        #endregion

        #region 日志
        public void DBLogInformation(int serviceType, string message, string remark = "")
        {
            LogBase(serviceType, LogLevel.Info, message, remark);
        }

        public void DBLogWarning(int serviceType, string message, string remark = "")
        {
            LogBase(serviceType, LogLevel.Warning, message, remark);
        }

        public void DBLogError(int serviceType, string message, string remark = "")
        {
            LogBase(serviceType, LogLevel.Error, message, remark);
        }

        public void DBLogDebug(int serviceType, string message, string remark = "")
        {
            LogBase(serviceType, LogLevel.Debug, message, remark);
        }

        private void LogBase(int objServiceType, LogLevel objLogLevel, string objMessage, string objRemark)
        {
            try
            {
                _logDB.ServiceLog.Add(new ServiceLog()
                {
                    LogType = objServiceType,
                    LogLevel = (int)objLogLevel,
                    LogMessage = objMessage,
                    LogRemark = objRemark,
                    LogIp = string.Empty,
                    CreateTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch { }
        }
        #endregion
    }
}
