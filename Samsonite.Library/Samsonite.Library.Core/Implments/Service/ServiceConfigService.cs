using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Core
{
    public class ServiceConfigService : IServiceConfigService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public ServiceConfigService(IAppLogService appLogService, appEntities appEntities)
        {
            _appDB = appEntities;
            _appLogService = appLogService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<ServiceModuleInfo> GetQuery(ServiceConfigSearchRequest request)
        {
            QueryResponse<ServiceModuleInfo> _result = new QueryResponse<ServiceModuleInfo>();
            var _list = _appDB.ServiceModuleInfo.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.ModuleTitle.Contains(request.Keyword));
            }

            if (request.Status > 0)
            {
                _list = _list.Where(p => p.Status == (request.Status - 1));
            }

            if (request.IsRun > 0)
            {
                if (request.IsRun == 1)
                {
                    _list = _list.Where(p => p.IsRun);
                }
                else
                {
                    _list = _list.Where(p => !p.IsRun);
                }
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.SortID).ThenBy(p => p.SortID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(ServiceConfigAddRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ModuleTitle))
                {
                    throw new Exception("服务名称不能为空");
                }

                if (string.IsNullOrEmpty(request.ModuleMark))
                {
                    throw new Exception("服务标识不能为空");
                }
                else
                {
                    ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleMark == request.ModuleMark).SingleOrDefault();
                    if (objServiceModuleInfo != null)
                    {
                        throw new Exception("服务标识已经存在，请勿重复");
                    }
                }

                if (string.IsNullOrEmpty(request.ModuleAssembly))
                {
                    throw new Exception("Assembly不能为空");
                }

                if (string.IsNullOrEmpty(request.ModuleType))
                {
                    throw new Exception("Class不能为空");
                }

                if (string.IsNullOrEmpty(request.ModuleBLL))
                {
                    throw new Exception("BLL不能为空");
                }

                int _sortID = (_appDB.ServiceModuleInfo.Any()) ? _appDB.ServiceModuleInfo.Max(p => p.SortID) + 1 : 1;

                ServiceModuleInfo objData = new ServiceModuleInfo()
                {
                    ModuleTitle = request.ModuleTitle,
                    ModuleWorkflowID = request.ModuleWorkflowID,
                    ModuleMark = request.ModuleMark,
                    ModuleAssembly = request.ModuleAssembly,
                    ModuleType = request.ModuleType,
                    ModuleBLL = request.ModuleBLL,
                    FixType = request.FixType,
                    FixTime = request.FixTime,
                    IsRun = request.IsRun,
                    Remark = request.Remark,
                    SortID = _sortID,
                    Status = (int)ServiceStatus.Stop,
                    NextRunTime = null,
                    CreateTime = DateTime.Now
                };
                _appDB.ServiceModuleInfo.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.InsertLog<ServiceModuleInfo>(objData, objData.ModuleID.ToString());
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = "数据保存成功"
                };
            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Edit(ServiceConfigEditRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ModuleTitle))
                {
                    throw new Exception("服务名称不能为空");
                }

                if (string.IsNullOrEmpty(request.ModuleMark))
                {
                    throw new Exception("服务标识不能为空");
                }
                else
                {
                    ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleMark == request.ModuleMark && p.ModuleID != request.ID).SingleOrDefault();
                    if (objServiceModuleInfo != null)
                    {
                        throw new Exception("服务标识已经存在，请勿重复");
                    }
                }

                if (string.IsNullOrEmpty(request.ModuleAssembly))
                {
                    throw new Exception("Assembly不能为空");
                }

                if (string.IsNullOrEmpty(request.ModuleType))
                {
                    throw new Exception("Class不能为空");
                }

                if (string.IsNullOrEmpty(request.ModuleBLL))
                {
                    throw new Exception("BLL不能为空");
                }

                ServiceModuleInfo objData = _appDB.ServiceModuleInfo.Where(p => p.ModuleID == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.ModuleTitle = request.ModuleTitle;
                    objData.ModuleWorkflowID = request.ModuleWorkflowID;
                    objData.ModuleMark = request.ModuleMark;
                    objData.ModuleAssembly = request.ModuleAssembly;
                    objData.ModuleType = request.ModuleType;
                    objData.ModuleBLL = request.ModuleBLL;
                    objData.FixType = request.FixType;
                    objData.FixTime = request.FixTime;
                    objData.IsRun = request.IsRun;
                    objData.SortID = request.Sort;
                    objData.Remark = request.Remark;
                    _appDB.SaveChanges();

                    //添加日志
                    _appLogService.UpdateLog<ServiceModuleInfo>(objData, objData.ModuleID.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = "数据保存成功"
                    };
                }
                else
                {
                    throw new Exception("数据读取失败");
                }
            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Oper(ServiceConfigOperRequest request)
        {
            try
            {
                //限制每个服务当前只能有插入一条未处理完成的工作流
                var objServiceModuleJob = _appDB.ServiceModuleJob.Where(p => p.ModuleID == request.ID && (new List<int>() { (int)JobStatus.Wait, (int)JobStatus.Processing }).Contains(p.Status)).FirstOrDefault();
                if (objServiceModuleJob != null)
                {
                    throw new Exception("当前存在未处理完的工作流,请稍后在试.");
                }

                ServiceModuleInfo objServiceModuleInfo = _appDB.ServiceModuleInfo.Where(p => p.ModuleID == request.ID).SingleOrDefault();
                if (objServiceModuleInfo != null)
                {
                    switch (request.OperType)
                    {
                        case (int)JobType.Start:
                            if (objServiceModuleInfo.Status == (int)ServiceStatus.Stop)
                            {
                                _appDB.ServiceModuleJob.Add(new ServiceModuleJob()
                                {
                                    ModuleID = objServiceModuleInfo.ModuleID,
                                    OperType = (int)JobType.Start,
                                    Status = (int)JobStatus.Wait,
                                    StatusMessage = string.Empty,
                                    AddTime = DateTime.Now
                                });
                                _appDB.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("只有当服务流程为停止时才能进行启动操作.");
                            }
                            break;
                        case (int)JobType.Pause:
                            if (objServiceModuleInfo.Status == (int)ServiceStatus.Runing)
                            {
                                _appDB.ServiceModuleJob.Add(new ServiceModuleJob()
                                {
                                    ModuleID = objServiceModuleInfo.ModuleID,
                                    OperType = (int)JobType.Pause,
                                    Status = (int)JobStatus.Wait,
                                    StatusMessage = string.Empty,
                                    AddTime = DateTime.Now
                                });
                                _appDB.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("只有当服务流程为运行中时才能进行暂停操作");
                            }
                            break;
                        case (int)JobType.Continue:
                            if (objServiceModuleInfo.Status == (int)ServiceStatus.Pause)
                            {
                                _appDB.ServiceModuleJob.Add(new ServiceModuleJob()
                                {
                                    ModuleID = objServiceModuleInfo.ModuleID,
                                    OperType = (int)JobType.Continue,
                                    Status = (int)JobStatus.Wait,
                                    StatusMessage = string.Empty,
                                    AddTime = DateTime.Now
                                });
                                _appDB.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("只有当服务流程为暂停中时才能进行继续操作");
                            }
                            break;
                        default:
                            throw new Exception("未知指令.");
                    }

                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = "指令保存成功,请耐心等待执行..."
                    };
                }
                else
                {
                    throw new Exception("数据读取失败");
                }
            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse OperDelete(long[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception("请至少选择一条要操作的数据");
                }

                ServiceModuleJob objServiceModuleJob = new ServiceModuleJob();
                foreach (var id in ids)
                {
                    objServiceModuleJob = _appDB.ServiceModuleJob.Where(p => p.ID == id).SingleOrDefault();
                    if (objServiceModuleJob != null)
                    {
                        if (objServiceModuleJob.Status == (int)JobStatus.Wait)
                        {
                            _appDB.ServiceModuleJob.Remove(objServiceModuleJob);
                        }
                        else
                        {
                            throw new Exception(string.Format("{0}:{1}", id, "信息已经处理完成,无法进行删除"));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, "信息不存在或已被删除"));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("ServiceModuleJob", string.Join(",", ids));
                //返回信息
                return new PostResponse
                {
                    Result = true,
                    Message = "数据删除成功"
                };
            }
            catch (Exception ex)
            {
                return new PostResponse
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 返回服务集合
        /// </summary>
        /// <returns></returns>
        public List<ServiceModuleInfo> GetModuleObject()
        {
            return _appDB.ServiceModuleInfo.OrderBy(p => p.SortID).ToList();
        }
    }
}
