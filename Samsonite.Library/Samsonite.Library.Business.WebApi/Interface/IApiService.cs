using Samsonite.Library.Bussness.WebApi.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Bussness.WebApi
{
    public interface IApiService
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
