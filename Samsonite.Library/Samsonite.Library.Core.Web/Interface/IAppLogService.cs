using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Web
{
    public interface IAppLogService
    {
        #region 系统操作日志
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        void InsertLog<T>(T data, string record);

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        void InsertLog<T>(List<T> data, string record);

        /// <summary>
        /// 更新日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        void UpdateLog<T>(T data, string record);

        /// <summary>
        /// 更新日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="record"></param>
        void UpdateLog<T>(List<T> data, string record);

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="record"></param>
        /// <param name="message"></param>
        void DeleteLog(string tableName, string record, string message = "");

        /// <summary>
        /// 还原日志
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="record"></param>
        /// <param name="message"></param>
        void RestoreLog(string tableName, string record, string message = "");

        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        void SystemLog(string message, string logLevel = "");
        #endregion

        #region 登入日志
        /// <summary>
        /// 登录日志
        /// </summary>
        /// <param name="webAppLoginLog"></param>
        void LoginLog(WebAppLoginLog webAppLoginLog);

        /// <summary>
        /// 密码修改日志
        /// </summary>
        /// <param name="objWebAppPasswordLog"></param>
        void PasswordLog(WebAppPasswordLog objWebAppPasswordLog);
        #endregion
    }
}
