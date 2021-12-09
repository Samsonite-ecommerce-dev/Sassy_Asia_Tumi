using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.APP.Helper;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class HomeController : BaseController
    {
        private IBaseService _baseService;
        private IHomeService _homeService;
        private IUsersService _usersService;
        private IAppLanguageService _appLanguageService;
        private UserHelper _userHelper;
        private appEntities _appDB;
        private logEntities _logDB;
        public HomeController(IBaseService baseService, IHomeService homeService, IUsersService usersService, IAppLanguageService appLanguageService, appEntities appEntities, logEntities logEntities) : base(baseService)
        {
            _homeService = homeService;
            _baseService = baseService;
            _usersService = usersService;
            _appLanguageService = appLanguageService;
            _userHelper = new UserHelper(baseService);
            _appDB = appEntities;
            _logDB = logEntities;
        }

        #region 框架页
        [HttpGet]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult Index_Info()
        {
            //登入信息
            UserSessionModel _userSessionInfo = this.CurrentLoginUser;
            if (_userSessionInfo != null)
            {
                //加载语言包
                var _languagePack = this.GetLanguagePack;
                //登录信息
                string _userName = _userSessionInfo.UserName;
                string _defaultPage = string.Empty;
                if (_userSessionInfo.UserStatus == (int)UserStatus.ExpiredPwd)
                {
                    _defaultPage = Url.Action("EditPassword", "Home");
                }
                else
                {
                    _defaultPage = Url.Action("Main", "Home");
                }

                //返回信息
                return Json(new
                {
                    userName = _userName,
                    defaultPage = _defaultPage,
                    languagePack = _languagePack
                });
            }
            else
            {
                return Json(new { });
            }
        }

        [ServiceFilter(typeof(UserLoginAuthorize))]
        public IActionResult Index()
        {
            //权限功能列表
            List<int> _powers = new List<int>();
            UserSessionModel _userSessionInfo = this.CurrentLoginUser;
            if (_userSessionInfo != null)
            {
                _powers = _userSessionInfo.UserPowers.Select(p => p.FunctionID).ToList();
            }

            //菜单栏
            List<DefineMenu> _menuList = _homeService.GetMenuList(_powers);
            return View(_menuList);
        }
        #endregion

        #region 主页
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public ActionResult Main()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult Main_Info()
        {
            var objUserInfo = _appDB.UserInfo.Where(p => p.Userid == _baseService.CurrentLoginUser().Userid).SingleOrDefault();
            if (objUserInfo != null)
            {
                return Json(new
                {
                    //基础信息
                    userInfo = new
                    {
                        userName = objUserInfo.UserName,
                        type = _userHelper.GetUserTypeDisplay(objUserInfo.Type, false),
                        email = objUserInfo.Email,
                        lastLoginTime = this.GetLastLoginTime()
                    },
                });
            }
            else
            {
                return Json(new { });
            }
        }

        private string GetLastLoginTime()
        {
            string _result = string.Empty;
            //上次登录时间
            var _loginLogs = _logDB.WebAppLoginLog.Where(p => p.UserID == _baseService.CurrentLoginUser().Userid).Take(2).OrderByDescending(p => p.LogID).ToList();
            string _lastLoginTime = string.Empty;
            if (_loginLogs.Count == 2)
            {
                _result = _loginLogs.LastOrDefault().AddTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return _result;
        }
        #endregion

        #region 选择语言
        [ServiceFilter(typeof(UserLoginAuthorize))]

        public ActionResult LanguageConfig()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult LanguageConfig_Info()
        {
            //返回数据
            return Json(new
            {
                //语言包集合
                languageList = _appLanguageService.CurrentLanguageOption().Select(p => new
                {
                    label = p.LanguageName,
                    value = p.ID
                }).ToList(),
                //当前默认语言
                languageType = _baseService.CurrentLanguage()
            });
        }

        [HttpPost]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult LanguageConfig_Message(int id)
        {
            //过滤参数
            id = VariableHelper.SaferequestInt(id);

            var _res = _homeService.ResetLanguage(id);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 修改密码
        [HttpGet]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult EditPassword_Info()
        {
            bool _isExpired = false;
            string _expiredMsg = string.Empty;
            var objUserInfo = _usersService.GetUser(this.CurrentLoginUser.Userid);
            if (objUserInfo != null)
            {
                //如果是密码过期
                if (objUserInfo.Status == (int)Samsonite.Library.Core.Models.UserStatus.ExpiredPwd)
                {
                    _isExpired = true;
                    //如果是首次登入
                    if (objUserInfo.LastPwdEditTime == null)
                    {
                        _expiredMsg = "首次登入系统者,需要在修改新密码之后退出重新登入";
                    }
                    //密码过期
                    else
                    {
                        _expiredMsg = "密码已经过期,请先修改密码";
                    }
                }
            }
            //返回信息
            return Json(new
            {
                isExpired = _isExpired,
                expiredMsg = _expiredMsg
            });
        }

        [ServiceFilter(typeof(UserLoginAuthorize))]
        public ActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult EditPassword_Message(EditPasswordRequest request)
        {

            //过滤参数
            ValidateHelper.Validate<EditPasswordRequest>(request);

            request.UserID = this.CurrentLoginUser.Userid;

            var _res = _homeService.ResetPassword(request);
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
