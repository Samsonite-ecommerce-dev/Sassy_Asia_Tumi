using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class GroupInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string GroupText { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddDate { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }

    }
}
