using Samsonite.Library.Core.WebApi.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Core.WebApi
{
    public interface IAuthorizeService
    {
        /// <summary>
        /// 账号权限列表
        /// </summary>
        /// <returns></returns>
        List<AuthorizeUser> GetApiAccounts();

        /// <summary>
        /// 登入账号权限判断
        /// </summary>
        /// <param name="authorizeValidRequest"></param>
        /// <returns></returns>
        AuthorizeValidResponse VisitValid(AuthorizeValidRequest authorizeValidRequest);
    }
}
