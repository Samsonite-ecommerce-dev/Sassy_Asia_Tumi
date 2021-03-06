using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IConfigService
    {
        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Update(ConfigUpdateRequest request);
    }
}
