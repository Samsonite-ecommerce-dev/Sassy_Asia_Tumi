using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Core
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
