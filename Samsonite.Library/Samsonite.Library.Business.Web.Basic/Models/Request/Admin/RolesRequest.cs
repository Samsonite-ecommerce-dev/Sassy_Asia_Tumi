using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class RolesSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class RolesAddRequest
    {
        public string RoleName { get; set; }

        public int RoleWeight { get; set; }

        public string RoleFunctions { get; set; }

        public string RoleMemo { get; set; }
    }

    public class RolesEditRequest
    {
        public int ID { get; set; }

        public string RoleName { get; set; }

        public int RoleWeight { get; set; }

        public string RoleFunctions { get; set; }

        public int SeqNumber { get; set; }

        public string RoleMemo { get; set; }
    }

    public class RolesFunctionAttr
    {
        public int FunctionID { get; set; }

        public string FunctionValue { get; set; }
    }
}
