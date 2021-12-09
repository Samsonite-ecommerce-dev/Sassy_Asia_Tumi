using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samsonite.Library.Service.Core
{
    public class Config
    {
        //线程前缀
        public static string ThreadPrefix
        {
            get
            {
                return "SASSY_ASIA_TUMI";
            }
        }

        /// <summary>
        /// 线程循环监测时间间隔
        /// </summary>
        public const int ThreadIntervalTime = 1000 * 10;

        /// <summary>
        /// 出错最大执行次数
        /// </summary>
        public const int MaxErrorTimes = 3;

        /// <summary>
        /// 工作流循环监测时间间隔
        /// </summary>
        public const int JobIntervalTime = 1000 * 60;

        /// <summary>
        /// 是否开始控制台日志
        /// </summary>
        public const bool IsConsoleLogger = true;
    }
}
