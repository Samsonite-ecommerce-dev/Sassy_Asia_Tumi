using Samsonite.Library.Core.Web.Models;

namespace Samsonite.Library.Business.Web.Custom.Models
{
    public class UploadSparePartImportRequest : PageRequest
    {
        public string FileName { get; set; }
    }
}
