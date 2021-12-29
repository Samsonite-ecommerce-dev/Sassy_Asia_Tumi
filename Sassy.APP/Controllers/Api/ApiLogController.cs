using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Web.Basic;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class ApiLogController : BaseController
    {
        private IApiLogService _apiLogService;
        public ApiLogController(IBaseService baseService, IApiLogService apiLogService) : base(baseService)
        {
            _apiLogService = apiLogService;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type)
        {
            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers()
                });
            }
            else
            {
                return Json(new { });
            }
        }
        #endregion

        #region 查询
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public JsonResult Index_Message(ApiLogSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _apiLogService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.id,
                           s1 = dy.Ip,
                           s2 = dy.Url,
                           s3 = dy.RequestID,
                           s4 = (dy.State) ? "<label class=\"text-primary\">成功</label>" : "<label class=\"text-danger\">失败<label>",
                           s5 = dy.Remark,
                           s6 = dy.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                       }
            };
            return Json(_result);
        }
        #endregion
    }
}