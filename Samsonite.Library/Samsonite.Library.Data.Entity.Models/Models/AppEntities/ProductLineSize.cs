using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ProductLineSize
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
        public string SizeID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string SizeDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string SizeText { get; set; }

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
