using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Samsonite.Library.Business;
using Samsonite.Library.Core;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Service.Core;
using Samsonite.Library.Service.Core.Enum;
using Samsonite.Library.Service.Core.Implments;
using Samsonite.Library.Service.Core.Interface;
using Samsonite.Library.Service.Core.Model;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Samsonite.Library.Service.WorkService
{
    public class Worker : BackgroundService
    {
        private List<ModuleModel> usedModule_List = new List<ModuleModel>();
        private bool isInit = false;
        private IApplicationService _applicationService;
        private ISAPService _sAPService;
        private readonly appEntities _appDB;
        private readonly logEntities _logDB;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            //��ȡ�����ļ�
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false, true);
            var configurationRoot = builder.Build();
            var connectionStrings = configurationRoot.GetSection("ConnectionStrings");
            //�����������
            var services = new ServiceCollection();
            services.AddDbContext<appEntities>(o => o.UseSqlServer(connectionStrings.GetSection("appConnection").Value), ServiceLifetime.Transient);
            services.AddDbContext<logEntities>(o => o.UseSqlServer(connectionStrings.GetSection("logConnection").Value), ServiceLifetime.Transient);
            //ע�ắ��
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IFtpService, FtpService>();
            services.AddScoped<ISAPService, SAPService>();
            //��������
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            //��ʼ������
            _applicationService = serviceProvider.GetService<IApplicationService>();
            _sAPService = serviceProvider.GetService<ISAPService>();
            _logger = logger;
            _appDB = serviceProvider.GetService<appEntities>();
            _logDB = serviceProvider.GetService<logEntities>();

            //��ʼ����Ϣ
            InitializeInformation();
        }

        private void InitializeInformation()
        {
            //��ʼ����Ҫ�����ķ����б�
            ServiceInit();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                JobThread();

                await Task.Delay(Config.JobIntervalTime, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            this.WriteLogger(LogLevel.Information, "Start the service..", "Service");
            //������Ҫ���еķ���
            if (isInit)
            {
                if (usedModule_List.Count > 0)
                {
                    foreach (var _o in usedModule_List)
                    {
                        _o.ModuleInstance.Start();
                    }
                }
            }
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.WriteLogger(LogLevel.Warning, "Stop the service..", "Service");
            if (isInit)
            {
                if (usedModule_List.Count > 0)
                {
                    foreach (var _o in usedModule_List)
                    {
                        _o.ModuleInstance.Stop();
                    }
                }
            }
            await base.StopAsync(cancellationToken);
        }

        #region ����
        /// <summary>
        /// ��ʼ��
        /// </summary>
        private void ServiceInit()
        {
            try
            {
                this.WriteLogger(LogLevel.Information, "Loading the list of services which is used.", "Service");
                //��ȡ��Ҫ�����ķ����б�
                var objServiceModuleInfo_List = _appDB.ServiceModuleInfo.Where(p => p.IsRun).ToList();
                if (objServiceModuleInfo_List.Count > 0)
                {
                    foreach (ServiceModuleInfo objServiceModuleInfo in objServiceModuleInfo_List)
                    {
                        var o = this.CreateInstance(objServiceModuleInfo);
                        usedModule_List.Add(new ModuleModel()
                        {
                            ModuleID = objServiceModuleInfo.ModuleID,
                            ModuleInstance = o
                        });
                        this.WriteLogger(LogLevel.Information, $"->Run {objServiceModuleInfo.ModuleMark}.{objServiceModuleInfo.ModuleTitle}", "Service");
                    }
                    isInit = true;
                }
                else
                {
                    throw new Exception("There is no module need to be run.");
                }
            }
            catch (Exception ex)
            {
                isInit = false;
                this.WriteLogger(LogLevel.Error, ex.ToString(), "Service");
                throw ex;
            }
        }

        /// <summary>
        /// ��ش���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JobThread()
        {
            List<ServiceModuleJob> objWait_List = new List<ServiceModuleJob>();
            try
            {
                //������ʹ����й������б�
                List<ServiceModuleJob> objServiceModuleJob_List = _appDB.ServiceModuleJob.Where(p => (new List<int>() { (int)JobStatus.Wait, (int)JobStatus.Processing }).Contains(p.Status)).OrderBy(p => p.ID).ToList();
                //ÿ�������ն���,ÿ��ִֻ��һ��������,ȡ��������ǰ���һ��������
                List<IGrouping<int, ServiceModuleJob>> tmpModuleIDs = objServiceModuleJob_List.GroupBy(p => p.ModuleID).ToList();
                foreach (var tmpID in tmpModuleIDs)
                {
                    var tmp = tmpID.FirstOrDefault();
                    if (tmp != null)
                    {
                        objWait_List.Add(tmp);
                    }
                }
                if (objWait_List.Count > 0)
                {
                    this.WriteLogger(LogLevel.Information, $"Loading the list of services job which is waiting.Total Job:{objServiceModuleJob_List.Count}", "Service");
                }
                //ѭ������������
                foreach (var item in objWait_List)
                {
                    if (item.Status == (int)JobStatus.Wait)
                    {
                        try
                        {
                            //��ȡ��ǰ����״̬
                            ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleID == item.ModuleID).SingleOrDefault();
                            if (objServiceModuleInfo != null)
                            {
                                //�鿴��ǰ�����Ƿ���������
                                if (objServiceModuleInfo.IsRun)
                                {
                                    item.Status = (int)JobStatus.Processing;
                                    switch (item.OperType)
                                    {
                                        case (int)JobType.Start:
                                            if (objServiceModuleInfo.Status == (int)ServiceStatus.Stop)
                                            {
                                                //�鿴�ڴ����Ƿ���ڸ÷������
                                                var _o = usedModule_List.Where(p => p.ModuleID == objServiceModuleInfo.ModuleID).SingleOrDefault();
                                                if (_o != null)
                                                {
                                                    //�����������û���������ķ��񲻿��ܴ������ڴ���
                                                    throw new Exception("Repeat start.");
                                                }
                                                else
                                                {
                                                    //�����·������
                                                    var N_o = this.CreateInstance(objServiceModuleInfo);
                                                    //�ö�����뵽�ڴ�
                                                    usedModule_List.Add(new ModuleModel()
                                                    {
                                                        ModuleID = objServiceModuleInfo.ModuleID,
                                                        ModuleInstance = N_o
                                                    });
                                                    FileLogHelper.WriteLog($"->Run {objServiceModuleInfo.ModuleMark}.{objServiceModuleInfo.ModuleTitle}", "Service_Job");
                                                    //��ʼ����
                                                    N_o.CurrentJob(item.ID);
                                                    N_o.Start();
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Operations can only be performed when the module status is Stop.");
                                            }
                                            break;
                                        case (int)JobType.Pause:
                                            if (objServiceModuleInfo.Status == (int)ServiceStatus.Runing)
                                            {
                                                var _o = usedModule_List.Where(p => p.ModuleID == objServiceModuleInfo.ModuleID).SingleOrDefault();
                                                if (_o != null)
                                                {
                                                    _o.ModuleInstance.CurrentJob(item.ID);
                                                    _o.ModuleInstance.Pause();
                                                }
                                                else
                                                {
                                                    throw new Exception("The object of service dose not exsist in RAM.");
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Operations can only be performed when the module status is Runing.");
                                            }
                                            break;
                                        case (int)JobType.Continue:
                                            if (objServiceModuleInfo.Status == (int)ServiceStatus.Pause)
                                            {
                                                var _o = usedModule_List.Where(p => p.ModuleID == objServiceModuleInfo.ModuleID).SingleOrDefault();
                                                if (_o != null)
                                                {
                                                    _o.ModuleInstance.CurrentJob(item.ID);
                                                    _o.ModuleInstance.Continue();
                                                }
                                                else
                                                {
                                                    throw new Exception("The object of service dose not exsist in RAM.");
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Operations can only be performed when the module status is Pause.");
                                            }
                                            break;
                                        default:
                                            throw new Exception("Unkown command.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Disabled Service.");
                                }
                            }
                            else
                            {
                                throw new Exception("The service dose not exsist.");
                            }
                        }
                        catch (Exception ex)
                        {
                            item.Status = (int)JobStatus.Fail;
                            item.StatusMessage = ex.Message;
                        }
                        _appDB.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLogger(LogLevel.Error, ex.ToString(), "Service_Job");
            }
        }

        /// <summary>
        /// ����ӳ�����
        /// </summary>
        /// <param name="objServiceModuleInfo"></param>
        /// <returns></returns>
        private IModule CreateInstance(ServiceModuleInfo objServiceModuleInfo)
        {
            return (IModule)Assembly.Load(objServiceModuleInfo.ModuleAssembly).CreateInstance($"{objServiceModuleInfo.ModuleAssembly}.{objServiceModuleInfo.ModuleType}", false, BindingFlags.Default, null, new object[] { _applicationService, _sAPService, _appDB }, null, null);
        }

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        /// <param name="fileName"></param>
        private void WriteLogger(LogLevel logLevel, string msg, string fileName)
        {
            string _fileName = $"{Config.ThreadPrefix}_{fileName}";
            //����̨��־
            if (Config.IsConsoleLogger)
            {
                switch (logLevel)
                {
                    case LogLevel.Information:
                        _logger.LogInformation($"[{_fileName}]{msg}");
                        break;
                    case LogLevel.Warning:
                        _logger.LogWarning($"[{_fileName}]{msg}");
                        break;
                    case LogLevel.Error:
                        _logger.LogError($"[{_fileName}]{msg}");
                        break;
                    case LogLevel.Debug:
                        _logger.LogDebug($"[{_fileName}]{msg}");
                        break;
                }

            }
            //�ļ���־
            FileLogHelper.WriteLog(msg, _fileName);
        }
        #endregion
    }
}
