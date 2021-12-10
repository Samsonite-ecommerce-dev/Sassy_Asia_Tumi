using Samsonite.Library.Business.Basic.Models;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Basic
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
