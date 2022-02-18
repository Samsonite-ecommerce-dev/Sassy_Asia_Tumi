﻿using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Basic.Models
{
    public class ServiceLogSearchRequest : PageRequest
    {
        public int Type { get; set; }

        public int Level { get; set; }

        public string Keyword { get; set; }

        public string Time1 { get; set; }

        public string Time2 { get; set; }
    }
}
