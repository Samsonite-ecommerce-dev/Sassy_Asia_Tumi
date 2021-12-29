using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Web.Basic;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using Samsonite.Library.WebApi.Core;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class ApiConfigController : BaseController
    {
        private IMenuService _menuService;
        private IApiConfigService _apiConfigService;
        private appEntities _appDB;
        public ApiConfigController(IBaseService baseService, IMenuService menuService, IApiConfigService apiConfigService, appEntities appEntities) : base(baseService)
        {
            _menuService = menuService;
            _apiConfigService = apiConfigService;
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
                    userAuthorization = this.FunctionPowers()
                });
            }
            else if (type == "add")
            {
                //返回数据
                return Json(new
                {
                    interfaceList = from g in _menuService.InterfaceOptions()
                                    select new
                                    {
                                        groupID = g.GroupID,
                                        groupName = g.GroupName,
                                        indeterminate = false,
                                        selected = false,
                                        interfaces = from i in g.Interfaces
                                                     select new
                                                     {
                                                         label = i.InterfaceName,
                                                         value = i.ID
                                                     }
                                    }
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                int _appID = VariableHelper.SaferequestInt(id);

                WebApiAccount objWebApiAccount = _appDB.WebApiAccount.Where(p => p.ID == _appID).SingleOrDefault();
                if (objWebApiAccount != null)
                {
                    List<string> _attrs = JsonHelper.JsonDeserialize<List<string>>(objWebApiAccount.Ips);
                    List<int> _interfaces = _appDB.WebApiRoles.Where(p => p.AccountID == objWebApiAccount.ID).Select(p => p.InterfaceID).ToList();

                    //返回数据
                    return Json(new
                    {
                        interfaceList = from g in _menuService.InterfaceOptions()
                                        select new
                                        {
                                            groupID = g.GroupID,
                                            groupName = g.GroupName,
                                            indeterminate = (g.Interfaces.Select(p => p.ID).Where(o => _interfaces.Contains(o)).Count() < g.Interfaces.Count && g.Interfaces.Select(p => p.ID).Where(o => _interfaces.Contains(o)).Count() > 0),
                                            selected = g.Interfaces.Select(p => p.ID).Where(o => _interfaces.Contains(o)).Count() == g.Interfaces.Count,
                                            interfaces = from i in g.Interfaces
                                                         select new
                                                         {
                                                             label = i.InterfaceName,
                                                             value = i.ID
                                                         }
                                        },
                        model = new
                        {
                            id = objWebApiAccount.ID,
                            appID = objWebApiAccount.Appid,
                            token = objWebApiAccount.Token,
                            ips = from item in _attrs
                                  select new
                                  {
                                      index = _attrs.IndexOf(item),
                                      value = item
                                  },
                            interfaces = _interfaces,
                            isUsed = objWebApiAccount.IsUsed,
                            remark = objWebApiAccount.Remark
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
        public JsonResult Index_Message(ApiConfigSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _apiConfigService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.ID,
                           s1 = dy.Appid,
                           s2 = dy.Token,
                           s3 = string.Join(",", JsonHelper.JsonDeserialize<List<string>>(dy.Ips)),
                           s4 = dy.Remark,
                           s5 = (dy.IsUsed) ? "<label class=\"el-icon-check text-primary\"></label>" : "<label class=\"el-icon-close text-danger\"></label>"
                       }
            };
            return Json(_result);
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
        public JsonResult Add_Message(ApiConfigAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _apiConfigService.Add(request);
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
            int _appID = VariableHelper.SaferequestInt(id);

            WebApiAccount objWebApiAccount = _appDB.WebApiAccount.Where(p => p.ID == _appID).SingleOrDefault();
            if (objWebApiAccount != null)
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
        public JsonResult Edit_Message(ApiConfigEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _apiConfigService.Edit(request);
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