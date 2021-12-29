using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface ILanguageService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<View_LanguagePack> GetQuery(LanguageSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(LanguageAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(LanguageEditRequest request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Delete(long[] ids);

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        PostResponse Restore(long[] ids);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Sort(LanguageSortRequest request);

        /// <summary>
        /// 根据分类查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<LanguagePackKey> GetQueryByKey(LanguageSearchByKeyRequest request);

        /// <summary>
        /// 功能组合菜单
        /// </summary>
        /// <returns></returns>
        List<DefineGroupSelectOption> GetLanguageGroupObject();
    }
}
