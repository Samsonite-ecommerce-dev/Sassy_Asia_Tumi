using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class LanguagePackKey
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        public int FunctionID { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string PackKey { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SeqNumber { get; set; }

        /// <summary>
        /// 是否删除(0否1是)
        /// </summary>
        public bool IsDelete { get; set; }

    }
}
