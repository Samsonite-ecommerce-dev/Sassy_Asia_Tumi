using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Core
{
    public interface ILoginService
    {
        /// <summary>
        /// 账号登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isRemember">默认保存1天，记住密码保存30天</param>
        /// <returns></returns>
        PostResponse UserLogin(string userName, string password, bool isRemember);

        /// <summary>
        /// 用户退出
        /// </summary>
        void UserLoginOut();

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse ForgetPassword(ForgetPasswordRequest request);

        string CreateRandomPassword();

        /// <summary>
        /// 创建密钥
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        string CreatePrivateKey(int length);

        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="password"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string EncryptPassword(string password, string key);
    }
}
