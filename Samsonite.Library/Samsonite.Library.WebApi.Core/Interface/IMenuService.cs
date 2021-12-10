using Samsonite.Library.WebApi.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.WebApi.Core
{
    public interface IMenuService
    {
        /// <summary>
        /// 获取api接口用途
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        int GetAPIType(string controller);

        /// <summary>
        /// 功能组列表
        /// </summary>
        /// <returns></returns>
        List<ApiMenuGroup> InterfaceOptions();
    }
}
