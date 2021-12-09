using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SysConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public int ConfigID { get; set; }

        /// <summary>
        /// 配置标识
        /// </summary>
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置参数
        /// </summary>
        public string ConfigValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
