using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Basic.Models
{
    public class FunctionGroupSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class FunctionGroupAddRequest
    {
        public string GroupName { get; set; }

        public string GroupIcon { get; set; }

        public string GroupMemo { get; set; }
    }

    public class FunctionGroupEditRequest
    {
        public int ID { get; set; }

        public string GroupName { get; set; }

        public string GroupIcon { get; set; }

        public string GroupMemo { get; set; }
    }
}
