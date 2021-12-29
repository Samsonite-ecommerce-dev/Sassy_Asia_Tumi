using System;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class ServiceOperationLogSearchResponse
    {
        public int ModuleID { get; set; }

        public string ModuleTitle { get; set; }

        public long JobID { get; set; }

        public int OperType { get; set; }

        public int Status { get; set; }

        public string StatusMessage { get; set; }

        public DateTime AddTime { get; set; }
    }
}
