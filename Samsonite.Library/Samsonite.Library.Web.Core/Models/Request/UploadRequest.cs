using System;

namespace Samsonite.Library.Web.Core.Models
{
    public class UploadSearchRequest : PageRequest
    {
        public string Filepath { get; set; }
    }

    public class UploadSaveRequest
    {
        public string Model { get; set; }

        public string Catalog { get; set; }
    }

    public class UploadFileCollection
    {
        public string FileName { get; set; }

        public string FileExt { get; set; }

        public string FileSize { get; set; }

        public DateTime EditTime { get; set; }
    }
}
