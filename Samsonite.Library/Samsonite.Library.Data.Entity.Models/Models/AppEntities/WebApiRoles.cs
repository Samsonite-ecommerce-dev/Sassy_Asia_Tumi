using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class WebApiRoles
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// API用户账号ID
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// 功能接口ID
        /// </summary>
        public int InterfaceID { get; set; }

    }
}
