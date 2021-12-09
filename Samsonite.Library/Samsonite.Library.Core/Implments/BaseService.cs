using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Core
{
    public class BaseService : UserRoleService, IBaseService
    {
        private IAppConfigService _appConfigService;
        private int _currentFunctionID = 0;
        public BaseService(IAppConfigService appConfigService, IAppLanguageService appLanguageService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, appEntities appEntities) : base(appConfigService, appLanguageService, httpContextAccessor, memoryCache, appEntities)
        {
            _appConfigService = appConfigService;
            _currentFunctionID = this.CurrentFunctionID();
        }

        /// <summary>
        /// 获取语言包
        /// </summary>
        public Dictionary<string, string> CurrentLanguagePack()
        {
            return this.GetCurrentLanguagePack();
        }

        /// <summary>
        /// 获取站点配置信息
        /// </summary>
        public AppConfigModel CurrentApplicationConfig()
        {
            return _appConfigService.GetConfigCache();
        }

        /// <summary>
        /// 获取当前登录信息
        /// </summary>
        public UserSessionModel CurrentLoginUser()
        {
            return this.GetCurrentLoginUser();
        }

        /// <summary>
        /// 当前语言包ID
        /// </summary>
        /// <returns></returns>
        public int CurrentLanguage()
        {
            return this.GetCurrentLanguage();
        }

        /// <summary>
        /// 获取当前加载语言文件后缀
        /// </summary>
        /// <returns></returns>
        public string CurrentLanguageFile()
        {
            return this.GetCurrentLanguageFile();
        }

        /// <summary>
        /// 当前页面ID
        /// </summary>
        public int CurrentFunctionID()
        {
            return this.GetCurrentFunctionID();
        }

        /// <summary>
        /// 获取当前功能栏权限
        /// </summary>
        /// <returns></returns>
        public List<string> FunctionPowers()
        {
            return this.GetFunctionPowers(this._currentFunctionID);
        }

        /// <summary>
        /// 获取当前功能栏权限
        /// </summary>
        /// <param name="functionID"></param>
        /// <returns></returns>
        public List<string> FunctionPowers(int functionID)
        {
            return this.GetFunctionPowers(functionID);
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string MenuBar(string query = "")
        {
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();
            return this.GetMenuBar(this._currentFunctionID, _languagePack, query);
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public string MenuBar(int functionID, string query = "")
        {
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();
            return this.GetMenuBar(functionID, _languagePack, query);
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="additions"></param>
        /// <returns></returns>
        public string MenuBar(int functionID, List<string> additions)
        {
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();
            return this.GetMenuBar(functionID, _languagePack, "", additions);
        }

        /// <summary>
        /// 获取功能名称
        /// </summary>
        /// <param name="functionID"></param>
        /// <returns></returns>
        public string MenuName(int functionID)
        {
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();
            return this.GetMenuName(functionID, _languagePack);
        }

        /// <summary>
        /// 获取订单查询页面菜单Tab
        /// </summary>
        /// <returns></returns>
        public string GetSearchOrderTab()
        {
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();
            return this.GetSearchOrderTab(_languagePack);
        }
    }
}