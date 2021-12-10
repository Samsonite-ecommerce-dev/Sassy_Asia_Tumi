using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.APP.Controllers
{
    public class BaseController : Controller
    {
        private IBaseService _baseService;
        public BaseController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// 获取站点配置信息
        /// </summary>
        public AppConfigModel GetApplicationConfig
        {
            get
            {
                return _baseService.CurrentApplicationConfig();
            }
        }

        /// <summary>
        /// 获取语言包
        /// </summary>
        public Dictionary<string, string> GetLanguagePack
        {
            get
            {
                return _baseService.CurrentLanguagePack();
            }
        }

        /// <summary>
        /// 获取当前登录信息
        /// </summary>
        public UserSessionModel CurrentLoginUser
        {
            get
            {
                return _baseService.CurrentLoginUser();
            }
        }

        /// <summary>
        /// 当前页面ID
        /// </summary>
        public int CurrentFunctionID
        {
            get
            {
                return _baseService.CurrentFunctionID();
            }
        }

        /// <summary>
        /// 获取当前功能栏权限
        /// </summary>
        /// <returns></returns>
        public List<string> FunctionPowers()
        {
            return _baseService.FunctionPowers();
        }

        /// <summary>
        /// 获取当前功能栏权限
        /// </summary>
        /// <param name="objFunctionID"></param>
        /// <returns></returns>
        public List<string> FunctionPowers(int objFunctionID)
        {
            return _baseService.FunctionPowers(objFunctionID);
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="objQuery"></param>
        /// <returns></returns>
        public string MenuBar(string objQuery = "")
        {
            //加载语言包
            return _baseService.MenuBar(objQuery);
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="objFunctionID"></param>
        /// <param name="objQuery"></param>
        /// <returns></returns>
        public string MenuBar(int objFunctionID, string objQuery = "")
        {
            //加载语言包
            return _baseService.MenuBar(objFunctionID, objQuery);
        }

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="objFunctionID"></param>
        /// <param name="objAdditions"></param>
        /// <returns></returns>
        public string MenuBar(int objFunctionID, List<string> objAdditions)
        {
            //加载语言包
            return _baseService.MenuBar(objFunctionID, objAdditions);
        }

        /// <summary>
        /// 获取功能名称
        /// </summary>
        /// <param name="objFunctionID"></param>
        /// <returns></returns>
        public string MenuName(int objFunctionID)
        {
            //加载语言包
            return _baseService.MenuBar(objFunctionID);
        }
    }
}