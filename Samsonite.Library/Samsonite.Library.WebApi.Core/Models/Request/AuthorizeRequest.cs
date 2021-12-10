using System.Collections.Generic;

namespace Samsonite.Library.WebApi.Core.Models
{
    public class AuthorizeValidRequest
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string ActionRoute { get; set; }

        public string RequestUrl { get; set; }

        public string PostBody { get; set; }

        public string RequestIp { get; set; }

        public Dictionary<string, string> RequestParam { get; set; }

        public List<AuthorizeUser> AuthorizeUsers { get; set; }
    }
}
