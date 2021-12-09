using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class ServiceModuleInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// 服务标识
        /// </summary>
        public string ModuleMark { get; set; }

        /// <summary>
        /// 服务运行模块名
        /// </summary>
        public string ModuleTitle { get; set; }

        /// <summary>
        /// 工作流ID
        /// </summary>
        public string ModuleWorkflowID { get; set; }

        /// <summary>
        /// 服务运行程序集
        /// </summary>
        public string ModuleAssembly { get; set; }

        /// <summary>
        /// 服务运行程序集的类型
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// BLL名
        /// </summary>
        public string ModuleBLL { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 运行中(0否1是)
        /// </summary>
        public bool IsRun { get; set; }

        /// <summary>
        /// 是否开启错误通知(0否1是)
        /// </summary>
        public bool IsNotify { get; set; }

        /// <summary>
        /// 定时执行类型(0不设置,1间隔执行,2定时某一时刻执行)
        /// </summary>
        public int FixType { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public string FixTime { get; set; }

        /// <summary>
        /// 排序ID
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 状态:0:停止;1:运行;2.暂停
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>
        public DateTime? NextRunTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
