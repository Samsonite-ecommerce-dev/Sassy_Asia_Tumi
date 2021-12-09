using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebApiAccessLog
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 日志来源
        /// </summary>
        public int LogType { get; set; }

        /// <summary>
        /// 访问的URL地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求ID
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 访问结果(1成功0失败)
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
