using Microsoft.AspNetCore.Http;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Web.Core
{
    public class HomeService : IHomeService
    {
        private IBaseService _baseService;
        private ILoginService _loginService;
        private IAppLanguageService _appLanguageService;
        private IAppLogService _appLogService;
        private appEntities _appDB;
        private logEntities _logDB;
        private IHttpContextAccessor _httpContextAccessor;
        public HomeService(IBaseService baseService, ILoginService loginService, IAppLanguageService appLanguageService, IAppLogService appLogService, appEntities appEntities, logEntities logEntities, IHttpContextAccessor httpContextAccessor)
        {
            _baseService = baseService;
            _loginService = loginService;
            _appLanguageService = appLanguageService;
            _appLogService = appLogService;
            _appDB = appEntities;
            _logDB = logEntities;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 根据当前权限获取菜单列表
        /// </summary>
        /// <param name="powers"></param>
        /// <returns></returns>
        public List<DefineMenu> GetMenuList(List<int> powers)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            List<DefineMenu> _result = new List<DefineMenu>();
            List<DefineMenu.MenuChild> _children = new List<DefineMenu.MenuChild>();
            List<SysFunctionGroup> _SysFunctionGroups = _appDB.SysFunctionGroup.Where(p => p.Parentid == 0).OrderBy(p => p.Rootid).ToList();
            List<SysFunction> _SysFunctions = _appDB.SysFunction.Where(p => p.FuncType == 1 && p.IsShow && powers.Contains(p.Funcid)).OrderBy(p => p.SeqNumber).ToList();
            List<SysFunction> _SysFunction_Nexts = new List<SysFunction>();
            foreach (SysFunctionGroup _SysFunctionGroup in _SysFunctionGroups)
            {
                //是否存在子功能
                _SysFunction_Nexts = _SysFunctions.Where<SysFunction>(p => p.Groupid == _SysFunctionGroup.Groupid && p.FuncType == 1 && p.IsShow).OrderBy(p => p.SeqNumber).ToList();
                if (_SysFunction_Nexts.Count > 0)
                {
                    _children = new List<DefineMenu.MenuChild>();
                    foreach (SysFunction _SysFunction in _SysFunction_Nexts)
                    {
                        _children.Add(new DefineMenu.MenuChild() { ID = _SysFunction.Funcid, Name = _languagePack[string.Format("menu_function_{0}", _SysFunction.Funcid)], Url = _SysFunction.FuncUrl, Target = _SysFunction.FuncTarget });
                    }
                    _result.Add(new DefineMenu() { ID = _SysFunctionGroup.Groupid, Name = _languagePack[string.Format("menu_group_{0}", _SysFunctionGroup.Groupid)], Icon = _SysFunctionGroup.GroupIcon, Children = _children });
                }
            }
            return _result;
        }

        /// <summary>
        /// 重新设置语言
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PostResponse ResetLanguage(int id)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                //是否存在该语言
                LanguageType objAppLanguagePackModel = _appLanguageService.LanguageTypeOption().Where(p => p.ID == id).SingleOrDefault();
                if (objAppLanguagePackModel != null)
                {
                    //设置语言包
                    _appLanguageService.SetLanguage(id);
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _languagePack["home_languageconfig_message_config_success"]
                    };
                }
                else
                {
                    throw new Exception(_languagePack["common_data_load_false"]);
                }
            }
            catch
            {
                return new PostResponse()
                {
                    Result = false,
                    Message = _languagePack["home_languageconfig_message_config_false"]
                };
            }
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PostResponse ResetPassword(EditPasswordRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                if (string.IsNullOrEmpty(request.OldPassword))
                {
                    throw new Exception(_languagePack["home_editpassword_message_no_old_password"]);
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new Exception(_languagePack["home_editpassword_message_no_password"]);
                }
                else
                {
                    if (request.OldPassword == request.Password)
                    {
                        throw new Exception(_languagePack["home_editpassword_message_need_new_password"]);
                    }
                    else
                    {
                        if (!CheckHelper.ValidPassword(request.Password))
                        {
                            throw new Exception(_languagePack["home_editpassword_message_password_error"]);
                        }
                    }
                }

                if (string.IsNullOrEmpty(request.SurePassword))
                {
                    throw new Exception(_languagePack["home_editpassword_message_no_reset_password"]);
                }
                else
                {
                    if (request.Password != request.SurePassword)
                    {
                        throw new Exception(_languagePack["home_editpassword_message_not_same"]);
                    }
                }

                UserInfo objData = _appDB.UserInfo.Where(p => p.Userid == request.UserID).SingleOrDefault();
                if (objData != null)
                {
                    if (objData.Pwd.ToLower() != _loginService.EncryptPassword(request.OldPassword, objData.PrivateKey))
                    {
                        throw new Exception(_languagePack["home_editpassword_message_error_old_password"]);
                    }

                    string _encryptPassword = _loginService.EncryptPassword(request.Password, objData.PrivateKey);
                    //检查是否存在N次密码修改存在重复
                    List<string> objWebAppPasswordLogs = _logDB.WebAppPasswordLog.Where(p => p.UserID == objData.Userid).OrderByDescending(p => p.LogID).Select(p => p.Password).Take(_baseService.CurrentApplicationConfig.GlobalConfig.PwdPastNum).ToList();
                    if (objWebAppPasswordLogs.Contains(_encryptPassword))
                    {
                        throw new Exception(_languagePack["home_editpassword_message_password_repeat_error"]);
                    }

                    objData.Pwd = _encryptPassword;
                    objData.PwdErrorNum = 0;
                    objData.LastPwdEditTime = DateTime.Now;
                    //如果密码过期
                    if (objData.Status == (int)UserStatus.ExpiredPwd)
                    {
                        objData.Status = (int)UserStatus.Normal;
                    }
                    _appDB.SaveChanges();
                    //添加密码日志
                    _appLogService.PasswordLog(new WebAppPasswordLog()
                    {
                        Account = objData.UserName,
                        Password = _encryptPassword,
                        UserID = objData.Userid,
                        IP = HttpHelper.GetRequestIP(_httpContextAccessor.HttpContext),
                        Remark = string.Empty,
                        AddTime = DateTime.Now
                    });
                    //添加日志
                    _appLogService.UpdateLog<UserInfo>(objData, objData.Userid.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _languagePack["common_data_save_success"]
                    };
                }
                else
                {
                    throw new Exception(_languagePack["common_data_load_false"]);
                }
            }
            catch (Exception ex)
            {
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }
    }
}
