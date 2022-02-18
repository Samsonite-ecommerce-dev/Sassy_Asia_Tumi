using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Core.Web.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Samsonite.Library.Core.Web
{
    public class AppLogService : AppUserCore, IAppLogService
    {
        private logEntities _logDB;
        public AppLogService(IAppConfigService appConfigService, IAppLanguageService appLanguageService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, appEntities appEntities, logEntities logEntities) : base(appConfigService, appLanguageService, httpContextAccessor, memoryCache, appEntities)
        {
            _logDB = logEntities;
        }

        #region 系统操作日志
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        public void InsertLog<T>(T data, string record)
        {
            try
            {
                _logDB.WebAppOperationLog.Add(new WebAppOperationLog()
                {
                    OperationType = (int)AppOperationLogType.Insert,
                    TableName = data.GetType().Name,
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    RecordID = record,
                    LogMessage = JsonSerializer.Serialize(data),
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        public void InsertLog<T>(List<T> data, string record)
        {
            try
            {
                _logDB.WebAppOperationLog.Add(new WebAppOperationLog()
                {
                    OperationType = (int)AppOperationLogType.Insert,
                    TableName = data.GetType().Name,
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    RecordID = record,
                    LogMessage = JsonSerializer.Serialize(data),
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        public void UpdateLog<T>(T data, string record)
        {
            try
            {
                _logDB.WebAppOperationLog.Add(new WebAppOperationLog()
                {
                    OperationType = (int)AppOperationLogType.Update,
                    TableName = data.GetType().Name,
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    RecordID = record,
                    LogMessage = JsonSerializer.Serialize(data),
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        public void UpdateLog<T>(List<T> data, string record)
        {
            try
            {
                _logDB.WebAppOperationLog.Add(new WebAppOperationLog()
                {
                    OperationType = (int)AppOperationLogType.Update,
                    TableName = data.GetType().Name,
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    RecordID = record,
                    LogMessage = JsonSerializer.Serialize(data),
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="record"></param>
        /// <param name="message"></param>
        public void DeleteLog(string tableName, string record, string message = "")
        {
            try
            {
                _logDB.WebAppOperationLog.Add(new WebAppOperationLog()
                {
                    OperationType = (int)AppOperationLogType.Delete,
                    TableName = tableName,
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    RecordID = record,
                    LogMessage = message,
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 还原日志
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="record"></param>
        /// <param name="message"></param>
        public void RestoreLog(string tableName, string record, string message = "")
        {
            try
            {
                _logDB.WebAppOperationLog.Add(new WebAppOperationLog()
                {
                    OperationType = (int)AppOperationLogType.Delete,
                    TableName = tableName,
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    RecordID = record,
                    LogMessage = message,
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public void SystemLog(string message, string logLevel = "")
        {
            try
            {
                _logDB.WebAppErrorLog.Add(new WebAppErrorLog()
                {
                    UserID = this.GetCurrentUserID(),
                    UserIP = this.GetRequestIP(),
                    LogLevel = (logLevel == string.Empty) ? ErrorLogType.System.ToString() : logLevel,
                    LogMessage = message,
                    AddTime = DateTime.Now
                });
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 登入日志
        /// <summary>
        /// 登录日志
        /// </summary>
        /// <param name="webAppLoginLog"></param>
        public void LoginLog(WebAppLoginLog webAppLoginLog)
        {
            try
            {
                _logDB.WebAppLoginLog.Add(webAppLoginLog);
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 密码修改日志
        /// </summary>
        /// <param name="objWebAppPasswordLog"></param>
        public void PasswordLog(WebAppPasswordLog objWebAppPasswordLog)
        {
            try
            {
                _logDB.WebAppPasswordLog.Add(objWebAppPasswordLog);
                _logDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}