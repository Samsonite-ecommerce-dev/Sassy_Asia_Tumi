using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class LanguageType
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 语言名称
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        /// 语言编号
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 语言包文件名
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// 是否默认:1.是,2:否
        /// </summary>
        public bool IsDefault { get; set; }

    }
}
