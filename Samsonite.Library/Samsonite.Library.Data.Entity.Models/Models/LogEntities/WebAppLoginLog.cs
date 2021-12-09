using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebAppLoginLog
    {
        /// <summary>
        /// 
        /// </summary>
        public long LogID { get; set; }

        /// <summary>
        /// 登入状态(0失败1成功)
        /// </summary>
        public bool LoginStatus { get; set; }

        /// <summary>
        /// 登入方式:0.密码;1.邮箱;
        /// </summary>
        public int LoginType { get; set; }

        /// <summary>
        /// 登入帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 登入密码(如果是成功登入则不需要填写)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
