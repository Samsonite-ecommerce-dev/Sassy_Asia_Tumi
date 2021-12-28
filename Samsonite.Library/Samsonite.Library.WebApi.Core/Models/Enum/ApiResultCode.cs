using System;

namespace Samsonite.Library.WebApi.Core.Models
{
    public enum ApiResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 参数错误
        /// </summary>
        InvalidParameter = 10,
        /// <summary>
        /// 系统错误
        /// </summary>
        SystemError = 99
    }
}
