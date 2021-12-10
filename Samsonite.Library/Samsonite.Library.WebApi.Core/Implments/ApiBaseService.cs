using Samsonite.Library.Utility;
using Samsonite.Library.WebApi.Core.Models;
using Samsonite.Library.WebApi.Core.Utils;
using System;

namespace Samsonite.Library.WebApi.Core
{
    public class ApiBaseService : IApiBaseService
    {
        #region 全局配置
        /// <summary>
        /// 是否开启日志
        /// </summary>
        /// <returns></returns>
        public bool IsApiDebugLog()
        {
            return GlobalConfig.IsApiDebugLog;
        }
        #endregion

        #region 获取api接口用途
        /// <summary>
        /// 获取api接口用途
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public int GetAPIType(string controller)
        {
            int _result = 0;
            switch (controller)
            {
                case "SAS":
                    _result = (int)ApiType.SAS;
                    break;
            }
            return _result;
        }
        #endregion

        #region 创建请求ID
        /// <summary>
        /// 生成唯一随机数
        /// 注:GUID+4位随机数
        /// </summary>
        /// <returns></returns>
        public string GreateRequestID()
        {
            string _result = string.Empty;
            _result = Guid.NewGuid().ToString("N");
            Random _rnd = new Random();
            int value = _rnd.Next(1000, 10000);
            _result += value.ToString();
            return _result;
        }
        #endregion

        #region 写日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="requestD"></param>
        /// <param name="logs"></param>
        public void WriteLogger(string controllerName, string actionName, string requestD, string[] logs)
        {
            FileLogHelper.WriteLog(logs, requestD, $"{controllerName}/{actionName}/{DateTime.Now.ToString("HH")}");
        }
        #endregion
    }
}
