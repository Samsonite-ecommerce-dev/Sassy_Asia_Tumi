using Samsonite.Library.Business.WorkService;
using Samsonite.Library.Core.WorkService;
using Samsonite.Library.Core.WorkService.Model;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sassy.Service.Application
{
    public class DataProductInventoryFromSAP : IModule
    {
        private IApplicationService _applicationService;
        private ISAPService _sAPService;
        private appEntities _appDB;
        public DataProductInventoryFromSAP(IApplicationService applicationService, ISAPService sAPService, appEntities appEntities)
        {
            _applicationService = applicationService;
            _sAPService = sAPService;
            _appDB = appEntities;

            baseModel = _applicationService.InitBase<DataProductFromSAP>();
        }

        //初始化标记
        bool isInit = false;
        //停止执行标记
        bool isStop = false;
        //暂停执行标记
        bool isPause = false;
        //锁定标记对象
        object lockObj = new object();
        //基本参数
        private BaseModel baseModel = new BaseModel();
        //定时配置
        private ServiceModel serviceConfig = new ServiceModel();

        #region 获取初始化状态
        /// <summary>
        /// 获取执行标记
        /// </summary>
        public bool IsInit
        {
            get
            {
                lock (lockObj)
                {
                    return isInit;
                }
            }
            set
            {
                lock (lockObj)
                {
                    isStop = value;
                }
            }
        }

        public bool IsStop
        {
            get
            {
                lock (lockObj)
                {
                    return isStop;
                }
            }
            set
            {
                lock (lockObj)
                {
                    isStop = value;
                }
            }
        }

        public bool IsPause
        {
            get
            {
                lock (lockObj)
                {
                    return isPause;
                }
            }
            set
            {
                lock (lockObj)
                {
                    isStop = value;
                }
            }
        }
        #endregion

        #region Init
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (IsInit == true) return;
            try
            {
                //读取配置
                serviceConfig = _applicationService.InitService<DataProductInventoryFromSAP>();
                FileLogHelper.WriteLog($"{baseModel.ThreadName}:Module initial successful.Type:{serviceConfig.RunType},Inteval:{serviceConfig.RunTime},Next Time:{serviceConfig.NextRunTime.ToString("yyyy-MM-dd HH:mm:ss")}", baseModel.ThreadName);
                isInit = true;
            }
            catch (Exception ex)
            {
                FileLogHelper.WriteLog($"{baseModel.ThreadName}:Module initial fail：{ex.ToString()}.", baseModel.ThreadName);
            }
        }
        #endregion

        #region Start
        public void Start()
        {
            if (!IsInit) Init();
            if (!IsInit) return;
            isStop = false;
            Thread Thread_Run = new System.Threading.Thread(new System.Threading.ThreadStart(RunMethod));
            Thread_Run.Name = baseModel.ThreadName;
            Thread_Run.Start();

        }

        private void RunMethod()
        {
            FileLogHelper.WriteLog($"Begin Thread:{baseModel.ThreadName}.", baseModel.ThreadName);
            while (true)
            {
                if (!IsInit) Init();
                if (!IsStop)
                {
                    if (!IsPause)
                    {
                        //执行主任务
                        _applicationService.ThreadMethod(baseModel, serviceConfig, delegate () { this.DoWork(); });
                    }
                    //待处理工作流
                    _applicationService.CompleteModuleJob(baseModel);
                    //更新当前状态
                    var currentStatus = _applicationService.GetCurrentStatus(IsStop, IsPause);
                    _applicationService.SetServiceModuleStatus(baseModel, serviceConfig, currentStatus);
                    //休眠
                    System.Threading.Thread.Sleep(baseModel.LoopTime);
                }
                else
                {
                    //更新当前状态
                    _applicationService.SetServiceModuleStatus(baseModel, serviceConfig, _applicationService.GetCurrentStatus(IsStop, IsPause));
                    //清空下次执行时间
                    _applicationService.ClearNextRunTime(serviceConfig);
                    //跳出循环
                    break;
                }
            }
        }

        private void DoWork()
        {
            /***********下载产品库存数据***************/
            string _msg = DownProductInventoryDataFromSAP();
            /***************************************/

            //重置错误时间
            baseModel.CurrentErrorTimes = 0;
            //计算下次执行时间
            _applicationService.CalculationNextTime(serviceConfig);
            //保存结果
            _applicationService.DBLogInformation(serviceConfig.ServiceID, $"{_msg}<br/>Next Time:{serviceConfig.NextRunTime.ToString("yyyy-MM-dd HH:mm:ss")}.");
        }

        /// <summary>
        /// 下载产品库存数据
        /// </summary>
        private string DownProductInventoryDataFromSAP()
        {
            List<string> _msgList = new List<string>();
            FileLogHelper.WriteLog($"Start to download sparepart inventory from SAP.", baseModel.ThreadName);
            //保存产品
            var _result = _sAPService.DownSparePartInventory();
            //记录结果
            _msgList.Add($"->File download success:[{string.Join(",", _result.SuccessFiles)}],fail:[{string.Join(",", _result.FailFiles)}]");
            _msgList.Add($"->Total Record:{_result.ResultData.Count()},Success Record:{_result.ResultData.Where(p => p.Result).Count()},Fail Record:{_result.ResultData.Where(p => !p.Result).Count()}.");
            return string.Join("<br/>", _msgList);
        }
        #endregion

        #region interface
        public void Stop()
        {
            isStop = true;
            FileLogHelper.WriteLog($"Stop Thread:{baseModel.ThreadName}.", baseModel.ThreadName);
        }

        public void Pause()
        {
            isPause = true;
            FileLogHelper.WriteLog($"Pause Thread:{baseModel.ThreadName}.", baseModel.ThreadName);
        }

        public void Continue()
        {
            isInit = false;
            isPause = false;
            FileLogHelper.WriteLog($"Continue Thread:{baseModel.ThreadName}.", baseModel.ThreadName);
        }

        public void CurrentJob(Int64 id)
        {
            baseModel.CurrentJobID = id;
        }
        #endregion
    }
}
