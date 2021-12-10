using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Web.Core
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