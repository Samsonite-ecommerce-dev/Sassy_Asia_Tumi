using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Business.Web.Basic
{
    public class FunctionGroupService : IFunctionGroupService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public FunctionGroupService(IAppLogService appLogService, appEntities appEntities)
        {
            _appLogService = appLogService;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<SysFunctionGroup> GetQuery(FunctionGroupSearchRequest request)
        {
            QueryResponse<SysFunctionGroup> _result = new QueryResponse<SysFunctionGroup>();
            var _list = _appDB.SysFunctionGroup.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.GroupName.Contains(request.Keyword));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.Rootid).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(FunctionGroupAddRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.GroupName))
                {
                    throw new Exception("栏目名称不能为空");
                }

                SysFunctionGroup objSysFunctionGroup = _appDB.SysFunctionGroup.Where(p => p.GroupName == request.GroupName).SingleOrDefault();
                if (objSysFunctionGroup != null)
                {
                    throw new Exception("栏目已经存在，请勿重复");
                }

                int _rootID = (_appDB.SysFunctionGroup.Any()) ? _appDB.SysFunctionGroup.Max(p => p.Rootid) + 1 : 1;

                SysFunctionGroup objData = new SysFunctionGroup()
                {
                    GroupName = request.GroupName,
                    GroupIcon = request.GroupIcon,
                    Rootid = _rootID,
                    Parentid = 0,
                    SeqNumber = 0,
                    GroupMemo = request.GroupMemo,
                    CreateTime = DateTime.Now
                };
                _appDB.SysFunctionGroup.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.InsertLog<SysFunctionGroup>(objData, objData.Groupid.ToString());
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
        public PostResponse Edit(FunctionGroupEditRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.GroupName))
                {
                    throw new Exception("栏目名称不能为空");
                }

                SysFunctionGroup objSysFunctionGroup = _appDB.SysFunctionGroup.Where(p => p.GroupName == request.GroupName && p.Groupid != request.ID).SingleOrDefault();
                if (objSysFunctionGroup != null)
                {
                    throw new Exception("栏目已经存在，请勿重复");
                }

                SysFunctionGroup objData = _appDB.SysFunctionGroup.Where(p => p.Groupid == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.GroupName = request.GroupName;
                    objData.GroupIcon = request.GroupIcon;
                    objData.GroupMemo = request.GroupMemo;
                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<SysFunctionGroup>(objData, objData.Groupid.ToString());
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
            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    if (ids.Count() == 0)
                    {
                        throw new Exception("请至少选择一条要操作的数据");
                    }

                    SysFunctionGroup objSysFunctionGroup = new SysFunctionGroup();
                    foreach (var id in ids)
                    {
                        objSysFunctionGroup = _appDB.SysFunctionGroup.Where(p => p.Groupid == id).SingleOrDefault();
                        if (objSysFunctionGroup != null)
                        {
                            _appDB.Database.ExecuteSqlRaw("delete from SysFunction where GroupID={0}", id);
                            _appDB.SysFunctionGroup.Remove(objSysFunctionGroup);
                        }
                        else
                        {
                            throw new Exception(string.Format("{0}:{1}", id, "信息不存在或已被删除"));
                        }
                    }
                    _appDB.SaveChanges();
                    Trans.Commit();
                    //添加日志
                    _appLogService.DeleteLog("SysFunctionGroup,SysFunction", string.Join(",", ids));
                    //返回信息
                    return new PostResponse
                    {
                        Result = true,
                        Message = "数据删除成功"
                    };
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    return new PostResponse
                    {
                        Result = false,
                        Message = ex.Message
                    };
                }
            }
        }

        /// <summary>
        /// 返回栏目集合
        /// </summary>
        /// <returns></returns>
        public List<SysFunctionGroup> GetFunctionGroupObject()
        {
            return _appDB.SysFunctionGroup.Where(p => p.Parentid == 0).OrderBy(p => p.Rootid).ToList();
        }
    }
}
