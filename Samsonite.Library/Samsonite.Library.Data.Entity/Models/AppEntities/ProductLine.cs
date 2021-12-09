using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ProductLine
    {
        /// <summary>
        /// 
        /// </summary>
        public string LineID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string LineDescription { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string LineText { get; set; }

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
