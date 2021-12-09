using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ProductSparePart
    {
        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 关联ProductLine表主键ID
        /// </summary>
        public string LineID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SizeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColorID { get; set; }

        /// <summary>
        /// 关联GroupInfo表主键ID
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionID { get; set; }

        /// <summary>
        /// SparePart表主键ID
        /// </summary>
        public long SparePartID { get; set; }

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
