using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebAppErrorLog
    {
        /// <summary>
        /// 
        /// </summary>
        public long LogID { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 操作IP
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// 日志记录
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
