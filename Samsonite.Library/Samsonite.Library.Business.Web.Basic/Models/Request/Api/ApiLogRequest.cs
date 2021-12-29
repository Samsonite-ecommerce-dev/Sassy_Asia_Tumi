using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class ApiLogSearchRequest : PageRequest
    {
        public int LogType { get; set; }

        public string Keyword { get; set; }

        public int State { get; set; }

        public string Time1 { get; set; }

        public string Time2 { get; set; }
    }
}
