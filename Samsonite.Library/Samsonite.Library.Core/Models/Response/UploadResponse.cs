using System.Collections.Generic;

namespace Samsonite.Library.Core.Models
{
    public class UploadSaveResponse : PostResponse
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}
