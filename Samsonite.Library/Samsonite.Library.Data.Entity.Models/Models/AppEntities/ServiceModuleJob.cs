using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ServiceModuleJob
    {
        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// ServiceModuleInfo表主键ID
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// 操作指令:1.启动;2.暂停;3.继续
        /// </summary>
        public int OperType { get; set; }

        /// <summary>
        /// 状态:0.未处理;1.处理中;2.成功;3.失败
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态信息
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
