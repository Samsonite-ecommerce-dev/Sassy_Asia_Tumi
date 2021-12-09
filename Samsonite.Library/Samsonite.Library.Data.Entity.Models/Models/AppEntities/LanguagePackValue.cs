using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class LanguagePackValue
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int KeyID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LanguageTypeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PackValue { get; set; }

    }
}
