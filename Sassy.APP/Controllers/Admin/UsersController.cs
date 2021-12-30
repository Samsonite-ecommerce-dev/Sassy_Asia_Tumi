using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.APP.Helper;
using Samsonite.Library.Business.Web.Basic;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class UsersController : BaseController
    {
        private IBaseService _baseService;
        private IUsersService _usersService;
        private IRolesService _rolesService;
        private IAppLanguageService _appLanguageService;
        private appEntities _appBD;
        private UserHelper _userHelper;
        public UsersController(IBaseService baseService, IUsersService usersService, IRolesService rolesService, IAppLanguageService appLanguageService, appEntities appEntities) : base(baseService)
        {
            _baseService = baseService;
            _usersService = usersService;
            _rolesService = rolesService;
            _appLanguageService = appLanguageService;
            _appBD = appEntities;
            _userHelper = new UserHelper(baseService);
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            var _languagePack = this.GetLanguagePack;
            //只显示当前用户等额的权重分组权限
            var _roleList = _rolesService.GetRoleObject(_baseService.CurrentLoginUser.RoleWeight).Select(p => new { label = p.RoleName, value = p.Roleid }).ToList();
            var _languageList = _appLanguageService.CurrentLanguageOption().Select(p => new { label = p.LanguageName, value = p.ID }).ToList();
            var _userTypeList = _userHelper.UserTypeObject().ToList();

            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers(),
                    //账号类型
                    userTypeList = _userTypeList,
                    statusList = new List<DefineSelectOption>()
                    {
                        new DefineSelectOption{
                            Label = _languagePack["common_actived"],
                            Value = 0
                        },
                        new DefineSelectOption
                        {
                            Label = _languagePack["common_locked"],
                            Value = 1
                        }
                    }
                });
            }
            else if (type == "add")
            {
                //返回数据
                return Json(new
                {
                    //角色组列表
                    roleList = _roleList,
                    //语言集合
                    languageList = _languageList,
                    //账号类型
                    userTypeList = _userTypeList
                });
            }
            else if (type == "edit")
            {
                int _userID = VariableHelper.SaferequestInt(id);
                UserInfo objUser = _appBD.UserInfo.Where(p => p.Userid == _userID).SingleOrDefault();
                if (objUser != null)
                {
                    //返回数据
                    return Json(new
                    {
                        //角色组列表
                        roleList = _roleList,
                        //语言集合
                        languageList = _languageList,
                        //账号类型
                        userTypeList = _userTypeList,
                        model = new
                        {
                            id = objUser.Userid,
                            userName = objUser.UserName,
                            realName = objUser.RealName,
                            email = objUser.Email,
                            roles = _appBD.UserRoles.Where(p => p.Userid == objUser.Userid).Select(p => p.Roleid).ToList(),
                            defaultLanguage = objUser.DefaultLanguage,
                            userType = objUser.Type,
                            status = !(objUser.Status == (int)UserStatus.Locked),
                            memo = objUser.Remark,
                        }
                    });
                }
                else
                {
                    return Json(new { });
                }
            }
            else if (type == "edit_password")
            {
                //过滤参数
                int _userID = VariableHelper.SaferequestInt(id);
                UserInfo objUserInfo = _appBD.UserInfo.Where(p => p.Userid == _userID).SingleOrDefault();
                if (objUserInfo != null)
                {
                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objUserInfo.Userid,
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
        public JsonResult Index_Message(UsersSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _usersService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.Userid,
                           s1 = dy.UserName,
                           s2 = dy.RealName,
                           s3 = dy.Email,
                           s4 = _userHelper.GetUserTypeDisplay(dy.Type),
                           s5 = dy.Remark,
                           s6 = (dy.Status == (int)UserStatus.Locked) ? "<label class=\"el-icon-lock text-danger\"></label>" : "<label class=\"el-icon-check text-primary\"></label>",
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
        public JsonResult Add_Message(UsersAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _usersService.Add(request);
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
            int _userID = VariableHelper.SaferequestInt(id);
            UserInfo objUser = _appBD.UserInfo.Where(p => p.Userid == _userID).SingleOrDefault();
            if (objUser != null)
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
        public JsonResult Edit_Message(UsersEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _usersService.Edit(request);
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

            var _res = _usersService.Delete(_ids);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 恢复
        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Restore_Message(string ids)
        {
            //过滤参数
            int[] _ids = VariableHelper.SaferequestIntArray(ids);

            var _res = _usersService.Restore(_ids);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 编辑密码
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult EditPassword(int id)
        {
            //过滤参数
            int _userID = VariableHelper.SaferequestInt(id);
            UserInfo objUserInfo = _appBD.UserInfo.Where(p => p.Userid == _userID).SingleOrDefault();
            if (objUserInfo != null)
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
        public JsonResult EditPassword_Message(UsersPasswordEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _usersService.EditPassword(request);
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