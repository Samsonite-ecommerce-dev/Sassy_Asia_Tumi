using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic.Models
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
