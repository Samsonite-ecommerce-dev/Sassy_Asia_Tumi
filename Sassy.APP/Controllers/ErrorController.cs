using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Utility;
using System.Web;

namespace Samsonite.Library.APP.Controllers
{
    public class ErrorController : Controller
    {
        private IBaseService _baseService;
        public ErrorController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public JsonResult Initialize_Info(ErrorRequest request)
        {
            //过滤参数
            ValidateHelper.Validate<ErrorRequest>(request);

            string _message = string.Empty;
            switch (request.Type)
            {
                case (int)ErrorType.NoExsit:
                    _message = "对不起,您访问的页面不存在";
                    break;
                case (int)ErrorType.NoMessage:
                    _message = "对不起,该信息不存在";
                    break;
                case (int)ErrorType.NoPower:
                    _message = "对不起,您没有操作权限";
                    break;
                case (int)ErrorType.LoginTimeOut:
                    _message = "Login timeout.please login again.<a href=\"javascript:appVue.goLogin();\" class=\"href-blue-line\">登录</a>";
                    break;
                case (int)ErrorType.Other:
                    _message = HttpUtility.UrlDecode(request.Message);
                    break;
                default:
                    break;
            }

            //返回数据
            return Json(new
            {
                errMessage = _message
            });
        }

        public ActionResult Index(ErrorRequest request)
        {
            return View();
        }
    }
}