namespace Samsonite.Library.Core.Models
{
    public class UploadConfigSearchRequest : PageRequest
    {
        public string Keyword { get; set; }
    }

    public class UploadConfigAddRequest
    {
        public string UploadName { get; set; }

        public string ModelMark { get; set; }

        public int FileType { get; set; }

        public int MaxFileSize { get; set; }

        public int MaxFileCount { get; set; }

        public string AllowFile { get; set; }

        public string SaveStyle { get; set; }

        public string SaveCatalog { get; set; }

        public bool IsRename { get; set; }
    }

    public class UploadConfigEditRequest
    {
        public int ID { get; set; }

        public string UploadName { get; set; }

        public string ModelMark { get; set; }

        public int FileType { get; set; }

        public int MaxFileSize { get; set; }

        public int MaxFileCount { get; set; }

        public string AllowFile { get; set; }

        public string SaveStyle { get; set; }

        public string SaveCatalog { get; set; }

        public bool IsRename { get; set; }
    }
}
