using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.APP.Helper;
using Samsonite.Library.Basic;
using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class ServiceLogController : BaseController
    {
        private IServiceLogService _serviceLogService;
        private IServiceConfigService _serviceConfigService;
        private LogHelper _logHelper;
        public ServiceLogController(IBaseService baseService, IServiceLogService serviceLogService, IServiceConfigService serviceConfigService) : base(baseService)
        {
            _serviceLogService = serviceLogService;
            _serviceConfigService = serviceConfigService;
            _logHelper = new LogHelper();
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
                    userAuthorization = this.FunctionPowers(),
                    //栏目列表
                    moduleList = _serviceConfigService.GetModuleObject().Select(p => new { label = p.ModuleTitle, value = p.ModuleID }).ToList(),
                    //日志等级
                    logLevelList = _logHelper.LogLevelObject().ToList()
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
        public JsonResult Index_Message(ServiceLogSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //服务列表
            List<ServiceModuleInfo> objServiceModuleInfos = _serviceConfigService.GetModuleObject();
            //查询
            var _list = _serviceLogService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.ID,
                           s1 = (objServiceModuleInfos.Where(p => p.ModuleID == dy.LogType).SingleOrDefault() != null) ? objServiceModuleInfos.Where(p => p.ModuleID == dy.LogType).SingleOrDefault().ModuleTitle : string.Empty,
                           s2 = _logHelper.GetLogTypeDisplay(dy.LogLevel),
                           s3 = dy.LogMessage,
                           s4 = dy.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                       }
            };
            return Json(_result);
        }
        #endregion
    }
}