using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class UserInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int Userid { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 状态(0:正常,1:锁定,2:首次登入未修改密码)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 账号类型(1:内部员工,2:客户)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 默认语言
        /// </summary>
        public int DefaultLanguage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 连续密码输入错误次数
        /// </summary>
        public int PwdErrorNum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastPwdEditTime { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
