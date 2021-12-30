using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Samsonite.Library.Business.Web.Basic
{
    public class ApiConfigService : IApiConfigService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public ApiConfigService(IAppLogService appLogService, appEntities appEntities)
        {
            _appLogService = appLogService;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<WebApiAccount> GetQuery(ApiConfigSearchRequest request)
        {
            QueryResponse<WebApiAccount> _result = new QueryResponse<WebApiAccount>();
            var _list = _appDB.WebApiAccount.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.Appid.Contains(request.Keyword));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.ID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(ApiConfigAddRequest request)
        {
            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(request.AppID))
                    {
                        throw new Exception("账号名称不能为空");
                    }
                    else
                    {
                        WebApiAccount objWebApiAccount = _appDB.WebApiAccount.Where(p => p.Appid == request.AppID).SingleOrDefault();
                        if (objWebApiAccount != null)
                        {
                            throw new Exception("账号名称已经存在，请勿重复");
                        }
                    }

                    if (string.IsNullOrEmpty(request.Token))
                    {
                        throw new Exception("Token不能为空");
                    }
                    else
                    {
                        WebApiAccount objWebApiAccount = _appDB.WebApiAccount.Where(p => p.Token == request.Token).SingleOrDefault();
                        if (objWebApiAccount != null)
                        {
                            throw new Exception("Token已经存在，请勿重复");
                        }
                    }

                    List<IpsAttr> _ipsAttrs = JsonSerializer.Deserialize<List<IpsAttr>>(request.Ips);
                    if (_ipsAttrs.Count > 0)
                    {
                        foreach (var item in _ipsAttrs)
                        {
                            if (string.IsNullOrEmpty(item.Value))
                            {
                                throw new Exception("IP地址不能为空");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("请至少填写一个要限制的IP");
                    }

                    List<int> _interfaceAttrs = JsonSerializer.Deserialize<List<int>>(request.Interfaces);
                    if (_interfaceAttrs.Count == 0)
                    {
                        throw new Exception("请至少填写一个API权限");
                    }

                    WebApiAccount objData = new WebApiAccount()
                    {
                        Appid = request.AppID,
                        Token = request.Token,
                        CompanyName = string.Empty,
                        Ips = JsonSerializer.Serialize(_ipsAttrs.Select(p => p.Value)),
                        Remark = request.Remark,
                        IsUsed = request.IsUsed
                    };
                    _appDB.WebApiAccount.Add(objData);
                    _appDB.SaveChanges();
                    //添加接口权限
                    WebApiRoles objWebApiRoleses = new WebApiRoles();
                    foreach (var item in _interfaceAttrs)
                    {
                        objWebApiRoleses = new WebApiRoles()
                        {
                            AccountID = objData.ID,
                            InterfaceID = item
                        };
                        _appDB.WebApiRoles.Add(objWebApiRoleses);
                    }
                    _appDB.SaveChanges();
                    Trans.Commit();
                    //添加日志
                    _appLogService.InsertLog<WebApiAccount>(objData, objData.ID.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = "数据保存成功"
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
        public PostResponse Edit(ApiConfigEditRequest request)
        {
            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(request.AppID))
                    {
                        throw new Exception("账号名称不能为空");
                    }
                    else
                    {
                        WebApiAccount objWebApiAccount = _appDB.WebApiAccount.Where(p => p.Appid == request.AppID && p.ID != request.ID).SingleOrDefault();
                        if (objWebApiAccount != null)
                        {
                            throw new Exception("账号名称已经存在，请勿重复");
                        }
                    }

                    if (string.IsNullOrEmpty(request.Token))
                    {
                        throw new Exception("Token不能为空");
                    }
                    else
                    {
                        WebApiAccount objWebApiAccount = _appDB.WebApiAccount.Where(p => p.Token == request.Token && p.ID != request.ID).SingleOrDefault();
                        if (objWebApiAccount != null)
                        {
                            throw new Exception("Token已经存在，请勿重复");
                        }
                    }

                    List<IpsAttr> _ipsAttrs = JsonSerializer.Deserialize<List<IpsAttr>>(request.Ips);
                    if (_ipsAttrs.Count > 0)
                    {
                        foreach (var item in _ipsAttrs)
                        {
                            if (string.IsNullOrEmpty(item.Value))
                            {
                                throw new Exception("IP地址不能为空");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("请至少填写一个要限制的IP");
                    }

                    List<int> _interfaceAttrs = JsonSerializer.Deserialize<List<int>>(request.Interfaces);
                    if (_interfaceAttrs.Count == 0)
                    {
                        throw new Exception("请至少填写一个API权限");
                    }

                    WebApiAccount objData = _appDB.WebApiAccount.Where(p => p.ID == request.ID).SingleOrDefault();
                    if (objData != null)
                    {
                        objData.Appid = request.AppID;
                        objData.Token = request.Token;
                        objData.Ips = JsonSerializer.Serialize(_ipsAttrs.Select(p => p.Value));
                        objData.IsUsed = request.IsUsed;
                        objData.Remark = request.Remark;
                        _appDB.SaveChanges();
                        //删除原角色组
                        _appDB.Database.ExecuteSqlRaw("delete from WebApiRoles where AccountID={0}", objData.ID);
                        //添加接口权限
                        WebApiRoles objWebApiRoleses = new WebApiRoles();
                        foreach (var item in _interfaceAttrs)
                        {
                            objWebApiRoleses = new WebApiRoles()
                            {
                                AccountID = objData.ID,
                                InterfaceID = item
                            };
                            _appDB.WebApiRoles.Add(objWebApiRoleses);
                        }
                        _appDB.SaveChanges();
                        Trans.Commit();
                        //添加日志
                        _appLogService.UpdateLog<WebApiAccount>(objData, objData.ID.ToString());
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
    }
}
