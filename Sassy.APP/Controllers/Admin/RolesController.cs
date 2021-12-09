using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samsonite.Library.APP.Controllers
{
    public class RolesController : BaseController
    {
        private IRolesService _rolesService;
        private IFunctionGroupService _functionGroupService;
        private IFunctionService _functionService;
        private appEntities _appBD;
        public RolesController(IBaseService baseService, IRolesService rolesService, IFunctionGroupService functionGroupService, IFunctionService functionService, appEntities appEntities) : base(baseService)
        {
            _rolesService = rolesService;
            _functionGroupService = functionGroupService;
            _functionService = functionService;
            _appBD = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            //加载语言包
            var _languagePack = this.GetLanguagePack;

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
                //栏目列表
                var objSysFunctionGroups = _functionGroupService.GetFunctionGroupObject();
                //功能列表
                var objSysFunctions = _functionService.GetFunctionObject().Where(p => p.IsShow);

                //返回数据
                return Json(new
                {
                    roleFuncionList = from gp in objSysFunctionGroups
                                      select new
                                      {
                                          groupName = _languagePack[$"menu_group_{gp.Groupid}"],
                                          functions = from fc in objSysFunctions.Where(p => p.Groupid == gp.Groupid)
                                                      select new
                                                      {
                                                          funcID = fc.Funcid,
                                                          funcName = _languagePack[$"menu_function_{fc.Funcid}"],
                                                          funcPermissions = from item in JsonHelper.JsonDeserialize<List<DefineUserPower>>(fc.FuncPower)
                                                                            select new
                                                                            {
                                                                                label = item.Name,
                                                                                value = $"{fc.Funcid}|{item.Value}"
                                                                            },
                                                          selected = false
                                                      }
                                      }
                });
            }
            else if (type == "edit")
            {
                int _roleID = VariableHelper.SaferequestInt(id);
                StringBuilder _str = new StringBuilder();
                SysRole objSysRole = _appBD.SysRole.Where(p => p.Roleid == _roleID).SingleOrDefault();
                if (objSysRole != null)
                {
                    //栏目列表
                    var objSysFunctionGroups = _functionGroupService.GetFunctionGroupObject();
                    //功能列表
                    var objSysFunctions = _functionService.GetFunctionObject().Where(p => p.IsShow);
                    //当前的功能
                    var objSysRoleFunctions = _appBD.SysRoleFunction.Where(p => p.Roleid == _roleID);
                    List<string> _roleFunctionArrs = new List<string>();
                    foreach (var sysRoleFunction in objSysRoleFunctions)
                    {
                        if (!string.IsNullOrEmpty(sysRoleFunction.Powers))
                        {
                            foreach (var item in sysRoleFunction.Powers.Split(','))
                            {
                                _roleFunctionArrs.Add($"{sysRoleFunction.Funid}|{item}");
                            }
                        }
                    }

                    //返回数据
                    return Json(new
                    {
                        roleFuncionList = from gp in objSysFunctionGroups
                                          select new
                                          {
                                              groupName = _languagePack[$"menu_group_{gp.Groupid}"],
                                              functions = from fc in objSysFunctions.Where(p => p.Groupid == gp.Groupid)
                                                          select new
                                                          {
                                                              funcID = fc.Funcid,
                                                              funcName = _languagePack[$"menu_function_{fc.Funcid}"],
                                                              funcPermissions = from item in JsonHelper.JsonDeserialize<List<DefineUserPower>>(fc.FuncPower)
                                                                                select new
                                                                                {
                                                                                    label = item.Name,
                                                                                    value = $"{fc.Funcid}|{item.Value}"
                                                                                },
                                                              selected = false
                                                          }
                                          },
                        model = new
                        {
                            id = objSysRole.Roleid,
                            roleName = objSysRole.RoleName,
                            roleWeight = objSysRole.RoleWeight,
                            roleFunctions = _roleFunctionArrs,
                            seqNumber = objSysRole.SeqNumber,
                            roleMemo = objSysRole.RoleMemo
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
        public JsonResult Index_Message(RolesSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _rolesService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.Roleid,
                           s1 = dy.RoleName,
                           s2 = dy.RoleWeight,
                           s3 = dy.SeqNumber,
                           s4 = dy.RoleMemo
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
        public JsonResult Add_Message(RolesAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _rolesService.Add(request);
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
            int _roleID = VariableHelper.SaferequestInt(id);
            StringBuilder _str = new StringBuilder();
            SysRole objSysRole = _appBD.SysRole.Where(p => p.Roleid == _roleID).SingleOrDefault();
            if (objSysRole != null)
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
        public JsonResult Edit_Message(RolesEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _rolesService.Edit(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Delete_Message(string ids)
        {
            //过滤参数
            int[] _ids = VariableHelper.SaferequestIntArray(ids);

            var _res = _rolesService.Delete(_ids);
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