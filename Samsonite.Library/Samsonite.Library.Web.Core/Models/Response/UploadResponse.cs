using System.Collections.Generic;

namespace Samsonite.Library.Web.Core.Models
{
    public class UploadSaveResponse : PostResponse
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}
