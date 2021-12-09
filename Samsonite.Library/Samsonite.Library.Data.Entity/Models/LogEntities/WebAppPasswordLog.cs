using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebAppPasswordLog
    {
        /// <summary>
        /// 
        /// </summary>
        public long LogID { get; set; }

        /// <summary>
        /// 登入帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 账号ID
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
