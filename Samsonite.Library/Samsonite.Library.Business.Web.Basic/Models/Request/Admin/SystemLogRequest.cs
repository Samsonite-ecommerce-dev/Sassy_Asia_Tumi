using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class SystemLogSearchRequest : PageRequest
    {
        public string Keyword { get; set; }

        public string Time1 { get; set; }

        public string Time2 { get; set; }
    }
}
