using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebApiAccount
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 账号ID
        /// </summary>
        public string Appid { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// ip限制
        /// </summary>
        public string Ips { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否使用(0无效1有效)
        /// </summary>
        public bool IsUsed { get; set; }

    }
}
