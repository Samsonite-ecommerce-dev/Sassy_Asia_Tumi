using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Core
{
    public class RolesService : IRolesService
    {
        private IBaseService _baseService;
        private IAppLogService _appLogService;
        private IHttpContextAccessor _httpContextAccessor;
        private appEntities _appDB;
        public RolesService(IBaseService baseService, IAppLogService appLogService, IHttpContextAccessor httpContextAccessor, appEntities appEntities)
        {
            _baseService = baseService;
            _appLogService = appLogService;
            _httpContextAccessor = httpContextAccessor;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<SysRole> GetQuery(RolesSearchRequest request)
        {
            QueryResponse<SysRole> _result = new QueryResponse<SysRole>();
            var _list = _appDB.SysRole.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.RoleName.Contains(request.Keyword));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.SeqNumber).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(RolesAddRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack();

            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    List<RolesFunctionAttr> _rolesFunctionAttrs = new List<RolesFunctionAttr>();
                    foreach (var item in JsonHelper.JsonDeserialize<List<string>>(request.RoleFunctions))
                    {
                        if (item.IndexOf('|') > -1)
                        {
                            var tmp = item.Split('|');
                            _rolesFunctionAttrs.Add(new RolesFunctionAttr()
                            {
                                FunctionID = VariableHelper.SaferequestInt16(tmp[0]),
                                FunctionValue = tmp[1]
                            });
                        }
                    }

                    if (string.IsNullOrEmpty(request.RoleName))
                    {
                        throw new Exception(_languagePack["role_edit_message_no_rolename"]);
                    }

                    SysRole objSysRole = _appDB.SysRole.Where(p => p.RoleName == request.RoleName).SingleOrDefault();
                    if (objSysRole != null)
                    {
                        throw new Exception(_languagePack["role_edit_message_exist_rolename"]);
                    }

                    if (_rolesFunctionAttrs.Count == 0)
                    {
                        throw new Exception(_languagePack["role_edit_message_need_one_function"]);
                    }

                    List<object[]> _funcPermissions = new List<object[]>();
                    foreach (var _str in _rolesFunctionAttrs.GroupBy(p => p.FunctionID).Select(o => o.Key))
                    {
                        var tmpKeys = _rolesFunctionAttrs.Where(p => p.FunctionID == _str);
                        _funcPermissions.Add(new object[2] { _str, string.Join(",", tmpKeys.Select(p => p.FunctionValue)) });
                    }

                    int _seqNumberID = (_appDB.SysRole.Any()) ? _appDB.SysRole.Max(p => p.SeqNumber) + 1 : 1;

                    SysRole objData = new SysRole()
                    {
                        RoleName = request.RoleName,
                        RoleWeight = request.RoleWeight,
                        SeqNumber = _seqNumberID,
                        RoleMemo = request.RoleMemo,
                        CreateTime = DateTime.Now
                    };
                    _appDB.SysRole.Add(objData);
                    _appDB.SaveChanges();
                    //添加相关功能
                    SysRoleFunction objSysRoleFunction = new SysRoleFunction();
                    foreach (object[] _O in _funcPermissions)
                    {
                        objSysRoleFunction = new SysRoleFunction()
                        {
                            Funid = VariableHelper.SaferequestInt(_O[0]),
                            Roleid = objData.Roleid,
                            Powers = _O[1].ToString().ToLower()
                        };
                        _appDB.SysRoleFunction.Add(objSysRoleFunction);
                    }
                    _appDB.SaveChanges();
                    Trans.Commit();
                    //添加日志
                    _appLogService.InsertLog<SysRole>(objData, objData.Roleid.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _languagePack["common_data_save_success"]
                    };
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    //返回信息
                    return new PostResponse()
                    {
                        Result = false,
                        Message = ex.Message
                    };
                }
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Edit(RolesEditRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack();

            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    List<RolesFunctionAttr> _rolesFunctionAttrs = new List<RolesFunctionAttr>();
                    foreach (var item in JsonHelper.JsonDeserialize<List<string>>(request.RoleFunctions))
                    {
                        if (item.IndexOf('|') > -1)
                        {
                            var tmp = item.Split('|');
                            _rolesFunctionAttrs.Add(new RolesFunctionAttr()
                            {
                                FunctionID = VariableHelper.SaferequestInt16(tmp[0]),
                                FunctionValue = tmp[1]
                            });
                        }
                    }

                    if (string.IsNullOrEmpty(request.RoleName))
                    {
                        throw new Exception(_languagePack["role_edit_message_no_rolename"]);
                    }

                    SysRole objSysRole = _appDB.SysRole.Where(p => p.RoleName == request.RoleName && p.Roleid != request.ID).SingleOrDefault();
                    if (objSysRole != null)
                    {
                        throw new Exception(_languagePack["role_edit_message_exist_rolename"]);
                    }

                    if (_rolesFunctionAttrs.Count == 0)
                    {
                        throw new Exception(_languagePack["role_edit_message_need_one_function"]);
                    }

                    List<object[]> _funcPermissions = new List<object[]>();
                    foreach (var _str in _rolesFunctionAttrs.GroupBy(p => p.FunctionID).Select(o => o.Key))
                    {
                        var tmpKeys = _rolesFunctionAttrs.Where(p => p.FunctionID == _str);
                        _funcPermissions.Add(new object[2] { _str, string.Join(",", tmpKeys.Select(p => p.FunctionValue)) });
                    }

                    SysRole objData = _appDB.SysRole.Where(p => p.Roleid == request.ID).SingleOrDefault();
                    if (objData != null)
                    {
                        objData.RoleName = request.RoleName;
                        objData.RoleWeight = request.RoleWeight;
                        objData.SeqNumber = request.SeqNumber;
                        objData.RoleMemo = request.RoleMemo;
                        _appDB.SaveChanges();
                        //删除原功能
                        _appDB.Database.ExecuteSqlRaw("delete from SysRoleFunction where Roleid={0}", request.ID);
                        //添加相关功能
                        SysRoleFunction objSysRoleFunction = new SysRoleFunction();
                        foreach (object[] _O in _funcPermissions)
                        {
                            objSysRoleFunction = new SysRoleFunction()
                            {
                                Funid = VariableHelper.SaferequestInt(_O[0]),
                                Roleid = objData.Roleid,
                                Powers = _O[1].ToString().ToLower()
                            };
                            _appDB.SysRoleFunction.Add(objSysRoleFunction);
                        }
                        _appDB.SaveChanges();
                        Trans.Commit();
                        //添加日志
                        _appLogService.UpdateLog<SysRole>(objData, objData.Roleid.ToString());
                        //返回信息
                        return new PostResponse()
                        {
                            Result = true,
                            Message = _languagePack["common_data_save_success"]
                        };
                    }
                    else
                    {
                        throw new Exception(_languagePack["common_data_load_false"]);
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    //返回信息
                    return new PostResponse()
                    {
                        Result = false,
                        Message = ex.Message
                    };
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Delete(int[] ids)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack();

            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    if (ids.Count() == 0)
                    {
                        throw new Exception(_languagePack["common_data_need_one"]);
                    }

                    SysRole objSysRole = new SysRole();
                    foreach (var id in ids)
                    {
                        objSysRole = _appDB.SysRole.Where(p => p.Roleid == id).SingleOrDefault();
                        if (objSysRole != null)
                        {
                            _appDB.Database.ExecuteSqlRaw("delete from SysRoleFunction where Roleid ={0}", id);
                            _appDB.SysRole.Remove(objSysRole);
                        }
                        else
                        {
                            throw new Exception(string.Format("{0}:{1}", id, _languagePack["common_data_no_exsit"]));
                        }
                    }
                    _appDB.SaveChanges();
                    Trans.Commit();
                    //添加日志
                    _appLogService.DeleteLog("SysRole,SysRoleFunction", string.Join(",", ids));
                    //返回信息
                    return new PostResponse
                    {
                        Result = true,
                        Message = _languagePack["common_data_delete_success"]
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
        /// 返回角色组集合
        /// </summary>
        /// <param name="roleWeight"></param>
        /// <returns></returns>
        public List<SysRole> GetRoleObject(int roleWeight)
        {
            return _appDB.SysRole.Where(p => p.RoleWeight >= roleWeight).OrderBy(p => p.SeqNumber).ToList();
        }
    }
}
