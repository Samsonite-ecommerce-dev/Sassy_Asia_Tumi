using System.Collections.Generic;

namespace Samsonite.Library.Core.Web
{
    public interface IUserRoleService
    {
        /// <summary>
        /// 根据获取当前页面的FuncID
        /// </summary>
        /// <param name="objContext"></param>
        /// <param name="areas"></param>
        /// <returns></returns>
        int GetCurrentFunctionID(string areas = "");

        /// <summary>
        /// 获取功能权限
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="languagePack"></param>
        /// <returns></returns>
        List<string> GetFunctionPowers(int functionID);

        /// <summary>
        /// 获取当前导航栏
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="query"></param>
        /// <param name="additions"></param>
        /// <returns></returns>
        string GetMenuBar(int functionID, Dictionary<string, string> languagePack, string query = "", List<string> additions = null);

        /// <summary>
        /// 获取功能名称
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="languagePack"></param>
        /// <returns></returns>
        string GetMenuName(int functionID, Dictionary<string, string> languagePack);

        /// <summary>
        /// 获取订单查询页面菜单Tab
        /// </summary>
        /// <param name="languagePack"></param>
        /// <returns></returns>
        string GetSearchOrderTab(Dictionary<string, string> languagePack);
    }
}
