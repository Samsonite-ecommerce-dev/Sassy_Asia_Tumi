using Samsonite.Library.WebApi.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.WebApi.Core
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
        AuthorizeResult VisitValid(AuthorizeValidRequest authorizeValidRequest);
    }
}
