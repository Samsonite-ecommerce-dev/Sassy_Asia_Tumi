using System;

namespace Samsonite.Library.Basic.Models
{
    public class ServiceOperationLogSearchRequest : PageRequest
    {
        public int ModuleID { get; set; }

        public int JobType { get; set; }

        public int JobStatus { get; set; }

        public string Time1 { get; set; }

        public string Time2 { get; set; }
    }
}
