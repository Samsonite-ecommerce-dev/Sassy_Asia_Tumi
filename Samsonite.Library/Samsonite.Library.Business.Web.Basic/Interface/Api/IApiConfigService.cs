using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IApiConfigService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<WebApiAccount> GetQuery(ApiConfigSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(ApiConfigAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(ApiConfigEditRequest request);
    }
}
