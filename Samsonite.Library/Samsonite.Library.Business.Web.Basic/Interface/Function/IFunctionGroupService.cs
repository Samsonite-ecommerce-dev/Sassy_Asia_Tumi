using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IFunctionGroupService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<SysFunctionGroup> GetQuery(FunctionGroupSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(FunctionGroupAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(FunctionGroupEditRequest request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Delete(int[] ids);

        /// <summary>
        /// 返回栏目集合
        /// </summary>
        /// <returns></returns>
        List<SysFunctionGroup> GetFunctionGroupObject();
    }
}
