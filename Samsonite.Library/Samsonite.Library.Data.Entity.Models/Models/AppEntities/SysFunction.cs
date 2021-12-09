using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SysFunction
    {
        /// <summary>
        /// 功能ID，自动增长
        /// </summary>
        public int Funcid { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string FuncName { get; set; }

        /// <summary>
        /// 分组ID
        /// </summary>
        public int Groupid { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SeqNumber { get; set; }

        /// <summary>
        /// 1：栏目，2：功能
        /// </summary>
        public short FuncType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FuncSign { get; set; }

        /// <summary>
        /// 功能链接地址，适用于菜单
        /// </summary>
        public string FuncUrl { get; set; }

        /// <summary>
        /// 功能权限
        /// </summary>
        public string FuncPower { get; set; }

        /// <summary>
        /// 链接地址方式,值为blank,parent,self,top,iframe
        /// </summary>
        public string FuncTarget { get; set; }

        /// <summary>
        /// 简要描述
        /// </summary>
        public string FuncMemo { get; set; }

        /// <summary>
        /// 是否显示，1：显示，0：不显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
