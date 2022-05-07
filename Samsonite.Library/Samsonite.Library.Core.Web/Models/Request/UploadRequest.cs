using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Web.Models
{
    public class UploadSearchRequest : PageRequest
    {
        public string Filepath { get; set; }
    }

    public class UploadSaveRequest
    {
        public string Model { get; set; }

        public string Catalog { get; set; }

        /// <summary>
        /// Post的参数名file，必须要和上传页面中的Input type="file"标签的name属性值一样
        /// </summary>
        public List<IFormFile> file { get; set; }
    }

    public class UploadFileCollection
    {
        public string FileName { get; set; }

        public string FileExt { get; set; }

        public string FileSize { get; set; }

        public DateTime EditTime { get; set; }
    }
}
