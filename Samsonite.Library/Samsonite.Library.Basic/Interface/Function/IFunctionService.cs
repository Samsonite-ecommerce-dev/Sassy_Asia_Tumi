using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Basic
{
    public interface IFunctionService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<View_SysFunction> GetQuery(FunctionSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(FunctionAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(FunctionEditRequest request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Delete(int[] ids);

        /// <summary>
        /// 返回功能集合
        /// </summary>
        /// <returns></returns>
        List<SysFunction> GetFunctionObject();
    }
}
