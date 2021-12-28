using Samsonite.Library.WebApi.Core.Models;
using System;

namespace Samsonite.Library.WebApi.Core
{
    public class ApiException : Exception
    {
        public ApiException(string errorException) : base(errorException)
        {
            this.ErrorCode = (int)ApiResultCode.SystemError;
        }

        public ApiException(int errorCode, string errorException) : base(errorException)
        {
            //默认提示系统错误
            if (errorCode == 0) errorCode = (int)ApiResultCode.SystemError;
            this.ErrorCode = errorCode;
        }

        public int ErrorCode { get; set; }
    }
}
