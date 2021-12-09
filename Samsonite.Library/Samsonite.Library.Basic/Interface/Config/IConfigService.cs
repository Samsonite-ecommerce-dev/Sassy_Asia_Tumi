using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Basic
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
