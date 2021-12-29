using Samsonite.Library.Web.Core.Models;
using System;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class ServiceConfigSearchRequest : PageRequest
    {
        public string Keyword { get; set; }

        public int Status { get; set; }

        public int IsRun { get; set; }
    }

    public class ServiceConfigAddRequest
    {
        public string ModuleTitle { get; set; }

        public string ModuleWorkflowID { get; set; }

        public string ModuleMark { get; set; }

        public string ModuleAssembly { get; set; }

        public string ModuleType { get; set; }

        public string ModuleBLL { get; set; }

        public int FixType { get; set; }

        public string FixTime { get; set; }

        public string Remark { get; set; }

        public bool IsRun { get; set; }
    }

    public class ServiceConfigEditRequest
    {
        public int ID { get; set; }

        public string ModuleTitle { get; set; }

        public string ModuleWorkflowID { get; set; }

        public string ModuleMark { get; set; }

        public string ModuleAssembly { get; set; }

        public string ModuleType { get; set; }

        public string ModuleBLL { get; set; }

        public int FixType { get; set; }

        public string FixTime { get; set; }

        public string Remark { get; set; }

        public int Sort { get; set; }

        public bool IsRun { get; set; }
    }

    public class ServiceConfigOperRequest
    {
        public Int64 ID { get; set; }

        public int OperType { get; set; }
    }
}
