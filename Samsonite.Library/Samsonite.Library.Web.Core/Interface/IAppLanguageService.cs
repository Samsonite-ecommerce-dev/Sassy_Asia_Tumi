using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Web.Core
{
    public interface IAppLanguageService
    {
        /// <summary>
        /// 语言分类
        /// </summary>
        /// <returns></returns>
        List<LanguageType> LanguageTypeOption();

        /// <summary>
        /// 获取默认语言
        /// </summary>
        /// <returns></returns>
        LanguageType DefaultLanguagePack();

        /// <summary>
        /// 初始化语言缓存
        /// </summary>
        /// <returns></returns>
        void LoadLanguagePacks();

        /// <summary>
        /// 重置语言包缓存
        /// </summary>
        void ResetLanguagePacks();

        /// <summary>
        /// 设置当前语言包
        /// </summary>
        /// <returns></returns>
        void SetLanguage(int type);

        /// <summary>
        /// 根据站点配置获取当前语言列表
        /// </summary>
        /// <returns></returns>
        List<LanguageType> CurrentLanguageOption();
    }
}