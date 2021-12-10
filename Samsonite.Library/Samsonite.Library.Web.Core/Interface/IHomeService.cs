using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Web.Core
{
    public interface IHomeService
    {
        /// <summary>
        /// 根据当前权限获取菜单列表
        /// </summary>
        /// <param name="powers"></param>
        /// <returns></returns>
        List<DefineMenu> GetMenuList(List<int> powers);

        /// <summary>
        /// 重新设置语言
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PostResponse ResetLanguage(int id);

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PostResponse ResetPassword(EditPasswordRequest request);
    }
}
