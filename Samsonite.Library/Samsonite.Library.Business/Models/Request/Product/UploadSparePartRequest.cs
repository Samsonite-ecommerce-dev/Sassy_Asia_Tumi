using Samsonite.Library.Core.Models;

namespace Samsonite.Library.Business.Models
{
    public class UploadSparePartImportRequest : PageRequest
    {
        public string FileName { get; set; }
    }
}
