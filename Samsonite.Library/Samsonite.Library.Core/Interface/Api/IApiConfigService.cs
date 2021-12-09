using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;

namespace Samsonite.Library.Core
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
