using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Samsonite.Library.WebApi.Models;

namespace Samsonite.Library.API
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {

        public override void OnException(ExceptionContext context)
        {
            //返回错误信息
            context.Result = new JsonResult(new ApiResponse
            {
                Code = (int)ApiResultCode.Fail,
                Message = context.Exception.Message
            });

            base.OnException(context);
        }
    }
}
