using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.APP.Helper;
using Samsonite.Library.Business.Web.Basic;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class ServiceOperationLogController : BaseController
    {
        private IServiceOperationLogService _serviceOperationLogService;
        private IServiceConfigService _serviceConfigService;
        private ServiceHelper _serviceHelper;
        private appEntities _appDB;
        public ServiceOperationLogController(IBaseService baseService, IServiceOperationLogService serviceOperationLogService, IServiceConfigService serviceConfigService, appEntities appEntities) : base(baseService)
        {
            _serviceOperationLogService = serviceOperationLogService;
            _serviceConfigService = serviceConfigService;
            _serviceHelper = new ServiceHelper();
            _appDB = appEntities;
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
                    //操作类型
                    jobTypeList = _serviceHelper.JobTypeObject().ToList(),
                    //操作状态
                    jobStatusList = _serviceHelper.JobStatusObject().Select(p => new { label = p.Label, value = (int)p.Value + 1 }).ToList()
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
        public JsonResult Index_Message(ServiceOperationLogSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _serviceOperationLogService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.JobID,
                           s1 = dy.ModuleTitle,
                           s2 = _serviceHelper.GetJobTypeDisplay(dy.OperType),
                           s3 = _serviceHelper.GetJobStatusDisplay(dy.Status, true),
                           s4 = dy.StatusMessage,
                           s5 = dy.AddTime.ToString("yyyy-MM-dd HH:mm:ss")
                       }
            };
            return Json(_result);
        }
        #endregion
    }
}