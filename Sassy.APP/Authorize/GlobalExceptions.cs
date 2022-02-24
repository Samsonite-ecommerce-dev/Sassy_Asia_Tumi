using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Samsonite.Library.Core.Web;

namespace Sassy.APP
{
    public class GlobalExceptions : IExceptionFilter
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IAppLogService _appLogService;
        public GlobalExceptions(IWebHostEnvironment webHostEnvironment, IAppLogService appLogService)
        {
            _webHostEnvironment = webHostEnvironment;
            _appLogService = appLogService;
        }

        public void OnException(ExceptionContext context)
        {
            _appLogService.SystemLog(context.Exception.ToString());
        }
    }
}
