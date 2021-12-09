using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ProductLineColor
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ProductLine表主键ID
        /// </summary>
        public string LineID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColorID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ColorDescription { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string ColorText { get; set; }

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
