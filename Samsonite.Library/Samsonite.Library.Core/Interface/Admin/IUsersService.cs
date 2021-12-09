using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Core
{
    public interface IUsersService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<UserInfo> GetQuery(UsersSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(UsersAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(UsersEditRequest request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Delete(int[] ids);

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Restore(int[] ids);

        /// <summary>
        /// 编辑密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse EditPassword(UsersPasswordEditRequest request);

        /// <summary>
        /// 根据ID获取会员
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UserInfo GetUser(int userID);

        /// <summary>
        /// 根据ID获取会员集合
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        List<UserInfo> GetUsers(List<int> userIDs);
    }
}
