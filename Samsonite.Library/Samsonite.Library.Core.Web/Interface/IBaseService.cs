﻿using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Web
{
    public interface IBaseService
    {
        /// <summary>
        /// 获取语言包
        /// </summary>
        Dictionary<string, string> CurrentLanguagePack { get; }

        /// <summary>
        /// 获取站点配置信息
        /// </summary>
        AppConfigModel CurrentApplicationConfig { get; }

        /// <summary>
        /// 获取当前登录信息
        /// </summary>
        UserSessionModel CurrentLoginUser { get; }

        /// <summary>
        /// 当前语言包ID
        /// </summary>
        /// <returns></returns>
        int CurrentLanguage { get; }

        /// <summary>
        /// 获取当前加载语言文件后缀
        /// </summary>
        /// <returns></returns>
        string CurrentLanguageFileExt { get; }

        /// <summary>
        /// 当前页面ID
        /// </summary>
        int CurrentFunctionID{ get; }

        /// <summary>
        /// 获取当前功能栏权限
        /// </summary>
        /// <returns></returns>
        List<string> FunctionPowers();

        /// <summary>
        /// 获取当前功能栏权限
        /// </summary>
        /// <param name="functionID"></param>
        /// <returns></returns>
        List<string> FunctionPowers(int functionID);

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        string MenuBar(string query = "");

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        string MenuBar(int functionID, string query = "");

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="additions"></param>
        /// <returns></returns>
        string MenuBar(int functionID, List<string> additions);

        /// <summary>
        /// 获取功能名称
        /// </summary>
        /// <param name="functionID"></param>
        /// <returns></returns>
        string MenuName(int functionID);

        /// <summary>
        /// 获取订单查询页面菜单Tab
        /// </summary>
        /// <returns></returns>
        string GetSearchOrderTab();
    }
}
