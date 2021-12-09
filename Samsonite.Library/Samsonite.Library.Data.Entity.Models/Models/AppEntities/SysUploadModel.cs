using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SysUploadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 上传类型名
        /// </summary>
        public string UploadName { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string ModelMark { get; set; }

        /// <summary>
        /// 1图片2文件3flash
        /// </summary>
        public int FileType { get; set; }

        /// <summary>
        /// 限制大小
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// 最大文件上传数量限制
        /// </summary>
        public int MaxFileCount { get; set; }

        /// <summary>
        /// 允许上传的格式
        /// </summary>
        public string AllowFile { get; set; }

        /// <summary>
        /// 保存目录
        /// </summary>
        public string SaveCatalog { get; set; }

        /// <summary>
        /// 保存方式(dateorder/fileorder)
        /// </summary>
        public string SaveStyle { get; set; }

        /// <summary>
        /// 文件重命名(0否1是)
        /// </summary>
        public bool IsRename { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
