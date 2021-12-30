using System;
using System.Collections.Generic;

namespace Samsonite.Library.WorkService.Core.Model
{

    /// <summary>
    /// 服务初始参数
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 检测时间间隔
        /// </summary>
        public int LoopTime { get; set; }

        /// <summary>
        /// 当前错误次数
        /// </summary>
        public int CurrentErrorTimes { get; set; }

        /// <summary>
        /// 最多执行错误次数
        /// </summary>
        public int MaxErrorTimes { get; set; }

        /// <summary>
        /// 模块名
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 线程名
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public ServiceStatus CurrentStatus { get; set; }

        /// <summary>
        /// 当前工作流程ID
        /// </summary>
        public Int64 CurrentJobID { get; set; }
    }

    /// <summary>
    /// 服务当前参数
    /// </summary>
    public class ServiceModel
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        public int ServiceID { get; set; }

        /// <summary>
        /// 工作流ID
        /// </summary>
        public string WorkflowID { get; set; }

        /// <summary>
        /// 执行类型
        /// 1.间隔执行
        /// 2.定时执行
        /// </summary>
        public int RunType { get; set; }

        /// <summary>
        /// 时间
        /// 1.每隔多少秒执行
        /// 2.HH:mm
        /// </summary>
        public string RunTime { get; set; }

        /// <summary>
        /// 是否发送错误通知
        /// </summary>
        public bool IsNotify { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime LastRunTime { get; set; }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime NextRunTime { get; set; }
    }

    /// <summary>
    /// 服务对象
    /// </summary>
    public class ModuleModel
    {
        public int ModuleID { get; set; }

        public IModule ModuleInstance { get; set; }
    }

    public class InitializationResponse
    {
        public bool IsInit { get; set; }

        public List<ModuleModel> Modules { get; set; }
    }
}
