using System;

namespace Samsonite.Library.Core
{
    public interface IAntiforgeryService
    {
        /// <summary>
        /// 创建Token值
        /// </summary>
        /// <returns></returns>
        string AntiForgeryTokenValue();

        /// <summary>
        /// 创建隐藏域
        /// </summary>
        /// <returns></returns>
        string AntiForgeryToken();

        /// <summary>
        /// 验证Token是否匹配
        /// </summary>
        bool ValidateRequest();
    }
}
