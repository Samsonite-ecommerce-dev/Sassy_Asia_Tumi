using System;

namespace Samsonite.Library.WebApi.Core
{
    public interface IApiBaseService
    {
        /// <summary>
        /// 是否开启日志
        /// </summary>
        /// <returns></returns>
        bool IsApiDebugLog();

        /// <summary>
        /// 获取api接口用途
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        int GetAPIType(string controller);

        /// <summary>
        /// 生成唯一随机数
        /// 注:GUID+4位随机数
        /// </summary>
        /// <returns></returns>
        string GreateRequestID();

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="requestD"></param>
        /// <param name="logs"></param>
        void WriteLogger(string controllerName, string actionName, string requestD, string[] logs);
    }
}
