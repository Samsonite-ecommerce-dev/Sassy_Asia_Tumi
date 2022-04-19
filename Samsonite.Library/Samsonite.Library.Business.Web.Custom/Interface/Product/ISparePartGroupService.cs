using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Web.Custom
{
    public interface ISparePartGroupService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<GroupInfo> GetQuery(SparePartGroupSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(SparePartGroupAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(SparePartGroupEditRequest request);

        /// <summary>
        /// 返回分组集合
        /// </summary>
        /// <returns></returns>
       List<GroupInfo> GetFunctionGroupObject();
    }
}
