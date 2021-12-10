using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Basic.Models
{
    public class FtpConfigSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class FtpConfigAddRequest
    {
        public string FTPIdentify { get; set; }

        public string FTPName { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FilePath { get; set; }

        public string Remark { get; set; }
    }

    public class FtpConfigEditRequest
    {
        public int ID { get; set; }

        public string FTPIdentify { get; set; }

        public string FTPName { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FilePath { get; set; }

        public int Sort { get; set; }

        public string Remark { get; set; }
    }
}
