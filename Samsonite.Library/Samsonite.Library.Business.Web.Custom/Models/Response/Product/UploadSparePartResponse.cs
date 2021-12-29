using System.Collections.Generic;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class UploadSparePartImportResponse
    {
        public long SparePartId { get; set; }

        public string SparePartDesc { get; set; }

        public string VersionID { get; set; }

        public int GroupID { get; set; }

        public List<UploadSparePartSku> Skus { get; set; }

        public bool Result { get; set; }

        public string ResultMsg { get; set; }
    }

    public class UploadSparePartSku
    {
        public string SKU { get; set; }

        public string LineID { get; set; }

        public string ColorID { get; set; }

        public string SizeID { get; set; }
    }
}
