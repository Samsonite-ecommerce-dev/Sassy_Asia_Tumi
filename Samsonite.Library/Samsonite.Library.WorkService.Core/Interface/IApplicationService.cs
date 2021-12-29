using Samsonite.Library.WorkService.Core.Model;
using System;

namespace Samsonite.Library.WorkService.Core
{
    public interface IApplicationService
    {
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        BaseModel InitBase<T>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <returns></returns>
        ServiceModel InitService<T>();

        /// <summary>
        /// 计算下次执行时间
        /// </summary>
        /// <param name="serviceConfig"></param>
        /// <returns></returns>
        ServiceModel CalculationNextTime(ServiceModel serviceConfig);

        /// <summary>
        /// 主任务
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="serviceConfig"></param>
        /// <param name="actionFunction"></param>
        void ThreadMethod(BaseModel baseModel, ServiceModel serviceConfig, Action actionFunction);

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <param name="isStop"></param>
        /// <param name="isPause"></param>
        ServiceStatus GetCurrentStatus(bool isStop, bool isPause);

        /// <summary>
        /// 设置服务状态
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="serviceConfig"></param>
        /// <param name="status"></param>
        void SetServiceModuleStatus(BaseModel baseModel, ServiceModel serviceConfig, ServiceStatus status);

        /// <summary>
        /// 设置工作流ID状态
        /// </summary>
        /// <param name="baseModel"></param>
        void CompleteModuleJob(BaseModel baseModel);

        /// <summary>
        /// 清空下次执行时间
        /// </summary>
        /// <param name="serviceConfig"></param>
        void ClearNextRunTime(ServiceModel serviceConfig);


        void DBLogInformation(int serviceType, string message, string remark = "");


        void DBLogWarning(int serviceType, string message, string remark = "");


        void DBLogError(int serviceType, string message, string remark = "");


        void DBLogDebug(int serviceType, string message, string remark = "");
    }
}
