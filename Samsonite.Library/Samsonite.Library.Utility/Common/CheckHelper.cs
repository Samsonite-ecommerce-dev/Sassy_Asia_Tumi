using System.Text.RegularExpressions;

namespace Samsonite.Library.Utility
{
    public class CheckHelper
    {
        /// <summary>
        /// 注册用户名验证(允许字母数字汉字)
        /// </summary>
        /// <param name="objRegName">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckRegAccount(string objRegName)
        {
            Regex _result = new Regex("^[A-Za-z0-9\u4e00-\u9fa5]+$");
            return _result.Match(objRegName).Success;
        }

        /// <summary>
        /// 注册密码验证(允许字母数字)
        /// </summary>
        /// <param name="objPassword">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckRegPassword(string objPassword)
        {
            Regex _result = new Regex("^[A-Za-z0-9]+$");
            return _result.Match(objPassword).Success;
        }

        /// <summary>
        /// 注册验证(允许字母数字和_)
        /// </summary>
        /// <param name="objReg">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckReg(string objReg)
        {
            Regex _result = new Regex("^[A-Za-z0-9_]+$");
            return _result.Match(objReg).Success;
        }

        /// <summary>
        /// 验证数字
        /// </summary>
        /// <param name="objNumber">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckNumber(string objNumber)
        {
            Regex _result = new Regex("^[0-9]+$");
            return _result.Match(objNumber).Success;
        }

        /// <summary>
        /// 验证电话号码(允许数字和-)
        /// </summary>
        /// <param name="objTel">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckTel(string objTel)
        {
            Regex _result = new Regex(@"^((0\d{2,3})-)?(\d{7,8})(-(\d{3,}))?$");
            return _result.Match(objTel).Success;
        }

        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="objMobile">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckMobile(string objMobile)
        {
            Regex _result = new Regex(@"^((\(\d{2,3}\))|(\d{3}\-))?(13|15|17|18)\d{9}$");
            return _result.Match(objMobile).Success;
        }

        /// <summary>
        /// 验证邮箱格式
        /// </summary>
        /// <param name="objEmail">字符串</param>
        /// <returns>bool</returns>
        public static bool CheckEmail(string objEmail)
        {
            Regex _result = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return _result.Match(objEmail).Success;
        }

        /// <summary>
        /// 验证密码(8~16个数字,英文字母或一些特殊符号组成)
        /// 至少需要3种类型字符组合
        /// </summary>
        /// <param name="objPassword">密码</param>
        /// <returns>bool</returns>
        public static bool ValidPassword(string objPassword)
        {
            Regex _result = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[~!#$%^&*_+]).{8,16}$");
            return _result.Match(objPassword).Success;
        }
    }
}
