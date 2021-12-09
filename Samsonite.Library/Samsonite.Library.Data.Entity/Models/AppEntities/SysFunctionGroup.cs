using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SysFunctionGroup
    {
        /// <summary>
        /// 分组ID，自动增长
        /// </summary>
        public int Groupid { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 小图标
        /// </summary>
        public string GroupIcon { get; set; }

        /// <summary>
        /// 主排序号
        /// </summary>
        public int Rootid { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SeqNumber { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int Parentid { get; set; }

        /// <summary>
        /// 简要描述
        /// </summary>
        public string GroupMemo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
