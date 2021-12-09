using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.APP.Helper;
using Samsonite.Library.Basic;
using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class ServiceConfigController : BaseController
    {
        private IServiceConfigService _serviceConfigService;
        private ServiceHelper _serviceHelper;
        private appEntities _appDB;
        public ServiceConfigController(IBaseService baseService, IServiceConfigService serviceConfigService, appEntities appEntities) : base(baseService)
        {
            _serviceConfigService = serviceConfigService;
            _serviceHelper = new ServiceHelper();
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
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
                    //执行状态
                    serviceStatusList = _serviceHelper.ServiceStatusObject().Select(p => new { label = p.Label, value = (int)p.Value + 1 }).ToList()
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                int _serviceID = VariableHelper.SaferequestInt(id);
                ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleID == _serviceID).SingleOrDefault();
                if (objServiceModuleInfo != null)
                {
                    return Json(new
                    {
                        model = new
                        {
                            id = objServiceModuleInfo.ModuleID,
                            moduleTitle = objServiceModuleInfo.ModuleTitle,
                            moduleWorkflowID = objServiceModuleInfo.ModuleWorkflowID,
                            moduleMark = objServiceModuleInfo.ModuleMark,
                            moduleAssembly = objServiceModuleInfo.ModuleAssembly,
                            moduleType = objServiceModuleInfo.ModuleType,
                            moduleBLL = objServiceModuleInfo.ModuleBLL,
                            fixType = objServiceModuleInfo.FixType.ToString(),
                            fixTime = objServiceModuleInfo.FixTime,
                            isRun = objServiceModuleInfo.IsRun,
                            sort = objServiceModuleInfo.SortID,
                            remark = objServiceModuleInfo.Remark
                        }
                    });
                }
                else
                {
                    return Json(new { });
                }
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
        public JsonResult Index_Message(ServiceConfigSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _serviceConfigService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.ModuleID,
                           s1 = dy.ModuleTitle,
                           s2 = dy.ModuleMark,
                           s3 = dy.ModuleType,
                           s4 = GetModuleRunType(dy.FixType, dy.FixTime),
                           s5 = dy.SortID,
                           s6 = _serviceHelper.GetServiceStatusDisplay(dy.Status, true),
                           s7 = GetOperButton(dy.ModuleID, dy.IsRun, dy.Status),
                           s8 = (dy.NextRunTime == null) ? "--" : dy.NextRunTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                           s9 = (dy.IsRun) ? "<label class=\"el-icon-check text-primary\"></label>" : "<label class=\"el-icon-close text-danger\"></label>",
                           s10 = dy.Remark
                       }
            };
            return Json(_result);
        }

        private string GetModuleRunType(int objFixType, string objFixTime)
        {
            string _result = string.Empty;
            if (objFixType == 1)
            {
                _result = $"每隔{(VariableHelper.SaferequestDouble(objFixTime) / 60)}分钟执行一次服务";
            }
            else
            {
                _result = $"{objFixTime}";
            }
            return _result;
        }

        private string GetOperButton(int objModuleID, bool objIsRun, int objStatus)
        {
            string _result = string.Empty;
            if (objIsRun)
            {
                switch (objStatus)
                {
                    case (int)ServiceStatus.Stop:
                        _result = $"<a href=\"javascript:appVue.operService({objModuleID},{(int)JobType.Start})\" class=\"el-icon-video-play fontSize-20 text-primary\">&nbsp;</a>";
                        break;
                    case (int)ServiceStatus.Runing:
                        _result = $"<a href=\"javascript:appVue.operService({objModuleID},{(int)JobType.Pause})\" class=\"el-icon-video-pause fontSize-20 text-warning\">&nbsp;</a>";
                        break;
                    case (int)ServiceStatus.Pause:
                        _result = $"<a href=\"javascript:appVue.operService({objModuleID},{(int)JobType.Continue})\" class=\"el-icon-video-play fontSize-20 text-success\">&nbsp;</a>";
                        break;
                    default:
                        break;
                }
            }
            return _result;
        }
        #endregion

        #region 添加
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Add_Message(ServiceConfigAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _serviceConfigService.Add(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 编辑
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Edit(int id)
        {
            //过滤参数
            int _serviceID = VariableHelper.SaferequestInt(id);
            ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleID == _serviceID).SingleOrDefault();
            if (objServiceModuleInfo != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error", new { Type = (int)ErrorType.NoMessage });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Edit_Message(ServiceConfigEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _serviceConfigService.Edit(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 操作
        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Oper_Message(ServiceConfigOperRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _serviceConfigService.Oper(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Oper_Delete_Message(string ids)
        {
            //过滤参数
            long[] _ids = VariableHelper.SaferequestInt64Array(ids);

            var _res = _serviceConfigService.OperDelete(_ids);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

    }
}