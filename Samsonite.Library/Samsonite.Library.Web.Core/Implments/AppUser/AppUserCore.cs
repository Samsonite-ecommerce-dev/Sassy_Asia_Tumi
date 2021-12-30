using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Samsonite.Library.Web.Core
{
    public class AppUserCore
    {
        private IAppLanguageService _appLanguageService;
        private IHttpContextAccessor _httpContextAccessor;
        private IMemoryCache _memoryCache;
        private appEntities _appDB;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private IResponseCookies _cookie_res => _httpContextAccessor.HttpContext.Response.Cookies;
        private IRequestCookieCollection _cookie_req => _httpContextAccessor.HttpContext.Request.Cookies;
        private GlobalConfigModel _globalConfig;
        public AppUserCore(IAppConfigService appConfigService, IAppLanguageService appLanguageService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, appEntities appEntities)
        {
            _appLanguageService = appLanguageService;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _appDB = appEntities;
            _globalConfig = appConfigService.GetConfigCache().GlobalConfig;
        }

        private string _cacheName
        {
            get
            {
                return $"{_globalConfig.CacheKey}_LANGUAGE_PACK";
            }
        }

        private string _cookieName
        {
            get
            {
                return $"{_globalConfig.CookieKey}_LANGUAGE_PACK";
            }
        }

        /// <summary>
        /// 获取当前登录用户，不存在返回null
        /// </summary>
        /// <returns></returns>
        public UserSessionModel GetCurrentLoginUser()
        {
            UserSessionModel objUserSessionModel = null;
            try
            {
                string _sessionKey = _session.GetString($"{_globalConfig.SessionKey}_LoginMessage");
                if (!string.IsNullOrEmpty(_sessionKey))
                {
                    objUserSessionModel = JsonSerializer.Deserialize<UserSessionModel>(_sessionKey);
                }
                else
                {
                    //如果不存在，则去读cookie
                    string _UserName = string.Empty;
                    _cookie_req.TryGetValue($"{_globalConfig.CookieKey}_LoginMessage_Uname", out _UserName);
                    string _UserPass = string.Empty;
                    _cookie_req.TryGetValue($"{_globalConfig.CookieKey}_LoginMessage_Upass", out _UserPass);
                    if (!string.IsNullOrEmpty(_UserName) && !string.IsNullOrEmpty(_UserPass))
                    {
                        //Encrypt解密
                        _UserName = EncryptHelper.DESDecrypt(_UserName);
                        _UserPass = EncryptHelper.DESDecrypt(_UserPass);
                        objUserSessionModel = GetUserMessage(_UserName, _UserPass);
                    }
                }
            }
            catch
            {
                objUserSessionModel = null;
            }
            return objUserSessionModel;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public string GetRequestIP()
        {
            return HttpHelper.GetRequestIP(_httpContextAccessor.HttpContext);
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        public int GetCurrentUserID()
        {
            UserSessionModel objUserSessionModel = GetCurrentLoginUser();
            return (objUserSessionModel != null) ? objUserSessionModel.Userid : 0;
        }

        /// <summary>
        /// 获取当前语言包类型
        /// </summary>
        public int GetCurrentLanguage()
        {
            string _lgPack = string.Empty;
            if (_cookie_req.TryGetValue($"{this._cookieName}_LgPack", out _lgPack))
            {
                return VariableHelper.SaferequestInt(EncryptHelper.DESDecrypt(_lgPack));
            }
            else
            {
                //如果不存在cookie,则读取用户的默认语言
                UserSessionModel objUserSessionModel = this.GetCurrentLoginUser();
                if (objUserSessionModel != null)
                {
                    return objUserSessionModel.DefaultLanguage;
                }
                else
                {
                    //如果没有设置语言,则获取默认语言包
                    return _appLanguageService.DefaultLanguagePack().ID;
                }
            }
        }

        /// <summary>
        /// 获取当前加载语言文件后缀
        /// </summary>
        public string GetCurrentLanguageFileExt()
        {
            return _appLanguageService.LanguageTypeOption().Where(p => p.ID == this.GetCurrentLanguage()).SingleOrDefault().Lang;
        }

        /// <summary>
        /// 获取当前语言包
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCurrentLanguagePack()
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            try
            {
                object _O;
                if (_memoryCache.TryGetValue($"{this._cacheName}_{this.GetCurrentLanguage()}", out _O))
                {
                    _result = (Dictionary<string, string>)_O;
                }
                else
                {
                    //重新加载语言包
                    _appLanguageService.LoadLanguagePacks();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        /// <summary>
        /// 根据账户和密码(md5加密后)获取信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        private UserSessionModel GetUserMessage(string userName, string userPass)
        {
            UserSessionModel objUserSessionModel = new UserSessionModel();
            UserInfo objUserInfo = _appDB.UserInfo.SingleOrDefault<UserInfo>(p => p.UserName == userName && p.Pwd == userPass.ToLower());
            if (objUserInfo != null)
            {
                //读取权重集合
                var userWeights = (from ur in _appDB.UserRoles.Where(p => p.Userid == objUserInfo.Userid)
                                   join sr in _appDB.SysRole on ur.Roleid equals sr.Roleid
                                   select sr.RoleWeight).ToList();

                objUserSessionModel.Userid = objUserInfo.Userid;
                objUserSessionModel.UserName = objUserInfo.UserName;
                objUserSessionModel.Passwd = objUserInfo.Pwd;
                objUserSessionModel.UserType = objUserInfo.Type;
                objUserSessionModel.RoleWeight = userWeights.Any() ? userWeights.Min() : 99;
                objUserSessionModel.UserStatus = objUserInfo.Status;
                objUserSessionModel.UserPowers = GetUserFunctions(objUserSessionModel.Userid);
                objUserSessionModel.DefaultLanguage = objUserInfo.DefaultLanguage;
                //写入Session
                SetUserSession(objUserSessionModel);
            }
            else
            {
                objUserSessionModel = null;
            }
            return objUserSessionModel;
        }

        /// <summary>
        /// 保存session
        /// </summary>
        /// <param name="userSessionModel"></param>
        protected void SetUserSession(UserSessionModel userSessionModel)
        {
            _session.SetString($"{_globalConfig.SessionKey}_LoginMessage", JsonSerializer.Serialize<UserSessionModel>(userSessionModel));
        }

        /// <summary>
        /// 保存cookie
        /// </summary>
        /// <param name="userSessionModel"></param>
        /// <param name="time"></param>
        protected void SetUserCookie(UserSessionModel userSessionModel, int time)
        {
            //Encrypt加密
            string _Uname = EncryptHelper.DESEncrypt(userSessionModel.UserName);
            string _Upass = EncryptHelper.DESEncrypt(userSessionModel.Passwd);
            //插入cookie
            _cookie_res.Append($"{_globalConfig.CookieKey}_LoginMessage_Uname", _Uname, new CookieOptions { Expires = DateTime.Now.AddDays(time) });
            _cookie_res.Append($"{_globalConfig.CookieKey}_LoginMessage_Upass", _Upass, new CookieOptions { Expires = DateTime.Now.AddDays(time) });
        }

        /// <summary>
        /// 获取用户的权限
        /// </summary>
        /// <param name="userid"></param>
        protected List<UserSessionModel.UserPower> GetUserFunctions(int userid)
        {
            List<UserSessionModel.UserPower> _result = new List<UserSessionModel.UserPower>();
            //获取用户权限组
            List<int> objUserRoless = _appDB.UserRoles.Where(p => p.Userid == userid).Select(p => p.Roleid).ToList();
            if (objUserRoless.Count > 0)
            {
                //获取具体权限
                var objSysRoleFunctions = _appDB.SysRoleFunction.Where(p => objUserRoless.Contains(p.Roleid));
                foreach (var _o in objSysRoleFunctions)
                {
                    List<string> _OperPowers = new List<string>();
                    //操作权限
                    foreach (string _str in _o.Powers.Split(','))
                    {
                        _OperPowers.Add(_str.ToLower());
                    }
                    var _p = _result.Where(p => p.FunctionID == _o.Funid).FirstOrDefault();
                    if (_p != null)
                    {
                        _p.FunctionPower = this.MergePowers(_p.FunctionPower, _OperPowers);
                    }
                    else
                    {
                        _result.Add(new UserSessionModel.UserPower() { FunctionID = _o.Funid, FunctionPower = _OperPowers });
                    }
                }
            }
            return _result;
        }

        /// <summary>
        /// 合并多个操作权限集合
        /// </summary>
        /// <param name="orgPower"></param>
        /// <param name="newPower"></param>
        /// <returns></returns>
        private List<string> MergePowers(List<string> orgPower, List<string> newPower)
        {
            foreach (string _str in newPower)
            {
                if (!orgPower.Contains(_str))
                {
                    orgPower.Add(_str.ToLower());
                }
            }
            return orgPower;
        }
    }
}