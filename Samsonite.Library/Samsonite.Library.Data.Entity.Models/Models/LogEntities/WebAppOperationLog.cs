using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebAppOperationLog
    {
        /// <summary>
        /// 
        /// </summary>
        public long LogID { get; set; }

        /// <summary>
        /// 操作类型(1:添加,2:修改,3:删除)
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// 操作的表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 操作人IP
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 操作表ID
        /// </summary>
        public string RecordID { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
