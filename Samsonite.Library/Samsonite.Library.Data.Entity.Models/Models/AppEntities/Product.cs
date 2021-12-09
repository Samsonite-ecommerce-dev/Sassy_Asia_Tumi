using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class Product
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gridval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaterialDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColorDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaterialGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Collection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConstructionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

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
