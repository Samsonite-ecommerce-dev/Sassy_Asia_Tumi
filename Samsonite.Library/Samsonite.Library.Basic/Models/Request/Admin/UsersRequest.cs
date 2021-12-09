using Newtonsoft.Json;

namespace Samsonite.Library.Basic.Models
{
    public class UsersSearchRequest : PageRequest
    {
        public string Keyword { get; set; }

        public int UserType { get; set; }

        public int Status { get; set; }
    }

    public class UsersAddRequest
    {
        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Password { get; set; }

        public string Memo { get; set; }

        public string Roles { get; set; }

        public int DefaultLanguage { get; set; }

        public int UserType { get; set; }
    }

    public class UsersEditRequest
    {
        public int ID { get; set; }

        public string RealName { get; set; }

        public string Memo { get; set; }

        public string Roles { get; set; }

        public int DefaultLanguage { get; set; }

        public int UserType { get; set; }

        public bool Status { get; set; }
    }

    public class UsersPasswordEditRequest
    {
        public int ID { get; set; }

        public string Password { get; set; }

        public string SurePassword { get; set; }
    }
}
