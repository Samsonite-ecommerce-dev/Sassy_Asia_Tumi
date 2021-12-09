using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ServiceLog
    {
        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 日志分类
        /// </summary>
        public int LogType { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public int LogLevel { get; set; }

        /// <summary>
        /// 日志描述
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 日志备注
        /// </summary>
        public string LogRemark { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string LogIp { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
