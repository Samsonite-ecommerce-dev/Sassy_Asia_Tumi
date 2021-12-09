using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SparePart
    {
        /// <summary>
        /// 
        /// </summary>
        public long SparePartID { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        public string SparePartDescription { get; set; }

        /// <summary>
        /// 关联GroupInfo的主键ID
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 自定义编号
        /// </summary>
        public string CustomizeSparePart { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 基础价格
        /// </summary>
        public decimal BasicPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 货币单位
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UnitofMeasure { get; set; }

        /// <summary>
        /// 价格有效期
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// 价格有效期
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// 价格更新时间
        /// </summary>
        public DateTime? PriceUpdateDate { get; set; }

        /// <summary>
        /// 有效库存
        /// </summary>
        public int AvailableStock { get; set; }

        /// <summary>
        /// 库存数
        /// </summary>
        public int InventoryStock { get; set; }

        /// <summary>
        /// 库存更新时间
        /// </summary>
        public DateTime? InventoryUpdateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

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
