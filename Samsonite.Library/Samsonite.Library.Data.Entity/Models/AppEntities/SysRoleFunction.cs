using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SysRoleFunction
    {
        /// <summary>
        /// 
        /// </summary>
        public int RoleFunid { get; set; }

        /// <summary>
        /// 权限组ID
        /// </summary>
        public int Roleid { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        public int Funid { get; set; }

        /// <summary>
        /// 权限集合
        /// </summary>
        public string Powers { get; set; }

    }
}
