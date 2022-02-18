using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Core.Web
{
    public class UserRoleService : AppUserCore, IUserRoleService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private appEntities _appDB;
        public UserRoleService(IAppConfigService appConfigService, IAppLanguageService appLanguageService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, appEntities appEntities) : base(appConfigService, appLanguageService, httpContextAccessor, memoryCache, appEntities)
        {
            _httpContextAccessor = httpContextAccessor;
            _appDB = appEntities;
        }

        /// <summary>
        /// 根据获取当前页面的FuncID
        /// </summary>
        /// <param name="areas">如果是Area目录在的,此处需要增加Area目录名</param>
        /// <returns></returns>
        public int GetCurrentFunctionID(string areas = "")
        {
            string _currentUrl = HttpHelper.GetAbsolutePath(_httpContextAccessor.HttpContext);
            int t = _currentUrl.IndexOf("?");
            if (t > -1) _currentUrl = _currentUrl.Substring(0, t);
            int _FunID = 0;
            if (!string.IsNullOrEmpty(_currentUrl))
            {
                //取功能标识
                if (_currentUrl.IndexOf("/") > -1)
                {
                    string[] _urlArray = _currentUrl.Split('/');
                    string _func_sign = string.Empty;
                    //因为有时候会省略index,所以需要加以分别判断
                    if (!string.IsNullOrEmpty(areas))
                    {
                        if (areas.ToUpper() == _urlArray[1].ToUpper())
                        {
                            _func_sign = _urlArray[2];
                        }
                        else
                        {
                            _func_sign = _urlArray[1];
                        }
                    }
                    else
                    {
                        _func_sign = _urlArray[1];
                    }

                    SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.FuncSign.ToLower() == _func_sign.ToLower()).SingleOrDefault();
                    if (objSysFunction != null)
                    {
                        _FunID = objSysFunction.Funcid;
                    }
                }
            }
            return _FunID;
        }

        /// <summary>
        /// 获取功能权限
        /// </summary>
        /// <param name="functionID"></param>
        /// <returns></returns>
        public List<string> GetFunctionPowers(int functionID)
        {
            List<string> _result = new List<string>();
            UserSessionModel objUserSessionModel = this.GetCurrentLoginUser();
            UserSessionModel.UserPower objUserPower = objUserSessionModel.UserPowers.Where(p => p.FunctionID == functionID).FirstOrDefault();
            if (objUserPower != null)
            {
                _result = objUserPower.FunctionPower;
            }
            return _result;
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="languagePack"></param>
        /// <param name="query"></param>
        /// <param name="additions"></param>
        /// <returns></returns>
        public string GetMenuBar(int functionID, Dictionary<string, string> languagePack, string query = "", List<string> additions = null)
        {
            string _result = string.Empty;
            SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.Funcid == functionID).SingleOrDefault();
            if (objSysFunction != null)
            {
                string _url = $"/{objSysFunction.FuncUrl}";
                if (!string.IsNullOrEmpty(query))
                {
                    _url += "?" + query;
                }
                SysFunctionGroup objSysFunctionGroup = _appDB.SysFunctionGroup.Where(p => p.Groupid == objSysFunction.Groupid).SingleOrDefault();
                if (objSysFunctionGroup != null)
                {
                    _result = string.Format("<div class=\"main-top-category\">{0}</div>", languagePack[$"menu_function_{objSysFunction.Funcid}"]);
                    _result += string.Format("<ol><li>{0}</li><li class=\"split\">/</li><li><a href=\"{1}\">{2}</a></li>", languagePack[$"menu_group_{objSysFunctionGroup.Groupid}"], _url, languagePack[$"menu_function_{objSysFunction.Funcid}"]);
                    if (additions != null)
                    {
                        foreach (var addition in additions)
                        {
                            _result += string.Format("<li class=\"split\">/</li><li>{0}</li>", addition);
                        }
                    }
                    _result += "</ol>";
                }
            }
            return _result;
        }

        /// <summary>
        /// 获取功能名称
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="languagePack"></param>
        /// <returns></returns>
        public string GetMenuName(int functionID, Dictionary<string, string> languagePack)
        {
            string _result = string.Empty;
            SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.Funcid == functionID).SingleOrDefault();
            if (objSysFunction != null)
            {
                _result = languagePack[$"menu_function_{objSysFunction.Funcid}"];
            }
            return _result;
        }

        /// <summary>
        /// 获取订单查询页面菜单Tab
        /// </summary>
        /// <param name="languagePack"></param>
        /// <returns></returns>
        public string GetSearchOrderTab(Dictionary<string, string> languagePack)
        {
            string _result = string.Empty;
            SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.Funcid == 1).SingleOrDefault();
            if (objSysFunction != null)
            {
                _result = languagePack[$"menu_function_{objSysFunction.Funcid}"];
            }
            return _result;
        }
    }
}