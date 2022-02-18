using System;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Web.Models
{
    [Serializable]
    public class UserSessionModel
    {
        /// <summary>
        /// 用户ID号
        /// </summary>
        public int Userid { get; set; }
        /// <summary>
        /// 用户登录账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码，通过MD5加密
        /// </summary>
        public string Passwd { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int RoleWeight { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        public int UserStatus { get; set; }
        /// <summary>
        /// 账号权限
        /// </summary>
        public List<UserPower> UserPowers { get; set; }
        /// <summary>
        /// 默认语言
        /// </summary>
        public int DefaultLanguage { get; set; }

        /// <summary>
        /// 权限dto
        /// </summary>
        public class UserPower
        {
            /// <summary>
            /// 权限id
            /// </summary>
            public int FunctionID { get; set; }

            /// <summary>
            /// 操作权限
            /// </summary>
            public List<string> FunctionPower { get; set; }
        }
    }
}