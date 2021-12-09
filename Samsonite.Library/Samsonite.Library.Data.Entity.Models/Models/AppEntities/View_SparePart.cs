using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class View_SparePart
    {
        /// <summary>
        /// 
        /// </summary>
        public long SparePartID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SparePartDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomizeSparePart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BasicPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UnitofMeasure { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? PriceUpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AvailableStock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int InventoryStock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? InventoryUpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime AddDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupName { get; set; }

    }
}
