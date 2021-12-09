using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Basic
{
    public class FunctionService : IFunctionService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public FunctionService(IAppLogService appLogService, appEntities appEntities)
        {
            _appLogService = appLogService;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<View_SysFunction> GetQuery(FunctionSearchRequest request)
        {
            QueryResponse<View_SysFunction> _result = new QueryResponse<View_SysFunction>();
            var _list = _appDB.View_SysFunction.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.FuncName.Contains(request.Keyword));
            }

            //搜索条件
            if (request.GroupID > 0)
            {
                _list = _list.Where(p => p.Groupid == request.GroupID);
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.Groupid).ThenBy(p => p.SeqNumber).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(FunctionAddRequest request)
        {
            try
            {
                var _funcPowersArray = JsonHelper.JsonDeserialize<List<FuncPowerAttr>>(request.FuncPowers);

                if (string.IsNullOrEmpty(request.FuncName))
                {
                    throw new Exception("功能名称不能为空");
                }

                if (request.GroupID == 0)
                {
                    throw new Exception("请选择栏目组");
                }

                if (string.IsNullOrEmpty(request.FuncSign))
                {
                    throw new Exception("请填写功能标识");
                }

                if (string.IsNullOrEmpty(request.FuncUrl))
                {
                    throw new Exception("请填写默认地址");
                }

                if (_funcPowersArray.Count == 0)
                {
                    throw new Exception("至少添加一条权限");
                }

                List<DefineUserPower> _defineUserPowers = new List<DefineUserPower>();
                foreach (var item in _funcPowersArray)
                {
                    _defineUserPowers.Add(new DefineUserPower() { Name = item.Name.Trim(), Value = item.Value.Trim() });
                }

                int _seqNumberID = (_appDB.SysFunction.Where(p => p.Groupid == request.GroupID).Any()) ? _appDB.SysFunction.Where(p => p.Groupid == request.GroupID).Max(p => p.SeqNumber) + 1 : 1;

                SysFunction objData = new SysFunction()
                {
                    FuncName = request.FuncName,
                    Groupid = request.GroupID,
                    SeqNumber = _seqNumberID,
                    FuncType = request.FuncType,
                    FuncSign = request.FuncSign,
                    FuncUrl = request.FuncUrl,
                    FuncPower = JsonHelper.JsonSerialize(_defineUserPowers),
                    FuncTarget = request.FuncTarget,
                    IsShow = request.IsShow,
                    FuncMemo = request.FuncMemo,
                    CreateTime = DateTime.Now
                };
                _appDB.SysFunction.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.InsertLog<SysFunction>(objData, objData.Funcid.ToString());
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
        public PostResponse Edit(FunctionEditRequest request)
        {
            try
            {
                var _funcPowersArray = JsonHelper.JsonDeserialize<List<FuncPowerAttr>>(request.FuncPowers);

                if (string.IsNullOrEmpty(request.FuncName))
                {
                    throw new Exception("功能名称不能为空");
                }

                if (request.GroupID == 0)
                {
                    throw new Exception("请选择栏目组");
                }

                if (string.IsNullOrEmpty(request.FuncSign))
                {
                    throw new Exception("请填写功能标识");
                }

                if (string.IsNullOrEmpty(request.FuncUrl))
                {
                    throw new Exception("请填写默认地址");
                }

                if (_funcPowersArray.Count == 0)
                {
                    throw new Exception("至少添加一条权限");
                }

                List<DefineUserPower> _defineUserPowers = new List<DefineUserPower>();
                foreach (var item in _funcPowersArray)
                {
                    _defineUserPowers.Add(new DefineUserPower() { Name = item.Name.Trim(), Value = item.Value.Trim() });
                }

                SysFunction objData = _appDB.SysFunction.Where(p => p.Funcid == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.FuncName = request.FuncName;
                    objData.Groupid = request.GroupID;
                    objData.FuncType = request.FuncType;
                    objData.FuncSign = request.FuncSign;
                    objData.FuncUrl = request.FuncUrl;
                    objData.FuncPower = JsonHelper.JsonSerialize(_defineUserPowers);
                    objData.FuncTarget = request.FuncTarget;
                    objData.IsShow = request.IsShow;
                    objData.FuncMemo = request.FuncMemo;
                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<SysFunction>(objData, objData.Funcid.ToString());
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
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Delete(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception("请至少选择一条要操作的数据");
                }

                SysFunction objSysFunction = new SysFunction();
                foreach (var id in ids)
                {
                    objSysFunction = _appDB.SysFunction.Where(p => p.Funcid == id).SingleOrDefault();
                    if (objSysFunction != null)
                    {
                        _appDB.SysFunction.Remove(objSysFunction);
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, "信息不存在或已被删除"));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("SysFunction", string.Join(",", ids));
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
        /// 返回功能集合
        /// </summary>
        /// <returns></returns>
        public List<SysFunction> GetFunctionObject()
        {
            return _appDB.SysFunction.OrderBy(p => p.Groupid).OrderBy(p => p.SeqNumber).ToList();
        }
    }
}
