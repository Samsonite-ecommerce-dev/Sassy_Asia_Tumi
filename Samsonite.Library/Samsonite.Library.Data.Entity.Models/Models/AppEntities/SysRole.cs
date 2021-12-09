using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SysRole
    {
        /// <summary>
        /// 
        /// </summary>
        public int Roleid { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 权重:1最大
        /// </summary>
        public int RoleWeight { get; set; }

        /// <summary>
        /// 角色简要说明
        /// </summary>
        public string RoleMemo { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SeqNumber { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
