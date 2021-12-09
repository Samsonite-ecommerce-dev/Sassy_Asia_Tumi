using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class UserRoles
    {
        /// <summary>
        /// 
        /// </summary>
        public int UserRoleid { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int Userid { get; set; }

        /// <summary>
        /// 权限组ID
        /// </summary>
        public int Roleid { get; set; }

    }
}
