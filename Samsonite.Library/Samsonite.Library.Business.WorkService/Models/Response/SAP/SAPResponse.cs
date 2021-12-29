using System;

namespace Samsonite.Library.Business.WorkService.Models
{
    public class SAPMaterialDto
    {
        /// <summary>
        /// Material Type identifier if Finished Goods or Spare Part
        /// </summary>
        public string SapMaterialType { get; set; }

        /// <summary>
        /// Concatenation of line id (first 3 chars), line size (Last 3 chars) and line colour (5th and 6th char)
        /// </summary>
        public string SapManufacturerSku { get; set; }
        /// <summary>
        /// SAP Material ID
        /// </summary>
        public string SapMaterialId { get; set; }

        public int SapMaterialIdInt { set; get; }
        /// <summary>
        /// sap_color
        /// </summary>
        public string SapColor { get; set; }

        /// <summary>
        /// sap_name
        /// </summary>
        public string SapName { get; set; }

        /// <summary>
        /// Color description
        /// </summary>
        public string SapColorDescription { get; set; }

        /// <summary>
        /// Group ID for sparepart
        /// </summary>
        public string SapMaterialGroup { get; set; }
        public int SapMaterialGroupInt { set; get; }
        /// <summary>
        /// Finished goods collection or line description
        /// </summary>
        public string SapCollection { get; set; }

        /// <summary>
        /// Contruction Type ID - Division ID
        /// </summary>
        public string SapConstructionType { get; set; }

        public string SapStatus { get; set; }
    }

    public class SAPSparePartInventoryDto
    {
        public long SparePartId { get; set; }
        public int Quantity { get; set; }
        public DateTime StockDate { get; set; }
    }

    public class SAPSparePartPriceDto
    {
        public long SparePartId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        public string UnitofMeasure { get; set; }

        public string Currency { get; set; }
    }

    public class SAPMaterialResponse
    {
        public string SKU { get; set; }
        public long SparePartId { get; set; }
    }

    public class SAPSparePartInventoryResponse
    {
        public long SparePartId { get; set; }

        public int Quantity { get; set; }
    }

    public class SAPSparePartPriceResponse
    {
        public long SparePartId { get; set; }

        public decimal Price { get; set; }
    }
}
