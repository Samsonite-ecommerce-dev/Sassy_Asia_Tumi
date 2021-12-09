using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class View_LanguagePack
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

        /// <summary>
        /// 
        /// </summary>
        public int FunctionID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PackKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SeqNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

    }
}
