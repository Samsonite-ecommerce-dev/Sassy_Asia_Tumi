using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Core
{
    public interface IAppConfigService
    {
        /// <summary>
        /// 获取系统配置信息
        /// </summary>
        /// <returns></returns>
        AppConfigModel GetConfig();

        /// <summary>
        /// 加载系统配置缓存
        /// </summary>
        void LoadConfigCache();

        /// <summary>
        /// 获取系统配置缓存
        /// </summary>
        /// <returns></returns>
        AppConfigModel GetConfigCache();

        /// <summary>
        /// 重置系统配置缓存
        /// </summary>
        void ResetConfigCache();
    }
}