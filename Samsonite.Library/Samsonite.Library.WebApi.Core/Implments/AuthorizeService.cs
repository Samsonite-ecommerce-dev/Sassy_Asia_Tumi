using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.WebApi.Core.Models;
using Samsonite.Library.WebApi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.WebApi.Core
{
    public class AuthorizeService : IAuthorizeService
    {
        private IMenuService _menuService;
        private UtilsHelper _utilsHelper;
        private appEntities _appDB;
        public AuthorizeService(IMenuService menuService, appEntities appEntities)
        {
            _menuService = menuService;
            _utilsHelper = new UtilsHelper();
            _appDB = appEntities;
        }

        /// <summary>
        /// 账号权限列表
        /// </summary>
        /// <returns></returns>
        public List<AuthorizeUser> GetApiAccounts()
        {
            List<AuthorizeUser> _result = new List<AuthorizeUser>();
            //账号列表
            var _webApiAccounts = _appDB.WebApiAccount.Where(p => p.IsUsed).ToList();
            //关联权限
            var _webApiRoles = _appDB.WebApiRoles.ToList();

            foreach (var item in _webApiAccounts)
            {
                _result.Add(new AuthorizeUser()
                {
                    Id = item.ID,
                    AppID = item.Appid,
                    Token = item.Token,
                    Ips = item.Ips,
                    Roles = _webApiRoles.Where(p => p.AccountID == item.ID).Select(o => o.InterfaceID).ToList()
                });
            }
            return _result;
        }

        /// <summary>
        /// 登入账号权限判断
        /// </summary>
        /// <param name="authorizeValidRequest"></param>
        /// <returns></returns>
        public AuthorizeValidResponse VisitValid(AuthorizeValidRequest authorizeValidRequest)
        {
            AuthorizeValidResponse _result = new AuthorizeValidResponse();
            AuthorizeParam _paramsRequest = new AuthorizeParam()
            {
                Url = authorizeValidRequest.RequestUrl,
                Ip = authorizeValidRequest.RequestIp,
                PostBody = authorizeValidRequest.PostBody
            };

            try
            {
                //基础参数
                var _params = authorizeValidRequest.RequestParam;
                _paramsRequest.Userid = (_params.ContainsKey("userid")) ? VariableHelper.SaferequestStr(_params["userid"]) : "";
                _paramsRequest.Version = (_params.ContainsKey("version")) ? VariableHelper.SaferequestStr(_params["version"]) : "";
                _paramsRequest.Format = (_params.ContainsKey("format")) ? VariableHelper.SaferequestStr(_params["format"]) : "";
                //默认MD5方式加密
                _paramsRequest.Method = (_params.ContainsKey("method")) ? VariableHelper.SaferequestStr(_params["method"]) : GlobalConfig.SIGN_METHOD_MD5;
                _paramsRequest.Timestamp = (_params.ContainsKey("timestamp")) ? VariableHelper.SaferequestStr(_params["timestamp"]) : "";
                _paramsRequest.Sign = (_params.ContainsKey("sign")) ? VariableHelper.SaferequestStr(_params["sign"]) : "";

                //账号
                if (string.IsNullOrEmpty(_paramsRequest.Userid))
                {
                    throw new Exception("Userid is mandatory!");
                }
                //版本
                if (string.IsNullOrEmpty(_paramsRequest.Version))
                {
                    throw new Exception("Version is mandatory!");
                }
                //格式
                if (!string.IsNullOrEmpty(_paramsRequest.Format))
                {
                    if (_paramsRequest.Format.ToLower() != "json") throw new Exception("Invalid Request Format");
                }
                else
                {
                    throw new Exception("Format is mandatory!");
                }
                //时间戳
                if (!string.IsNullOrEmpty(_paramsRequest.Timestamp))
                {
                    try
                    {
                        TimeHelper.UnixTimestampToDateTime(VariableHelper.SaferequestInt64(_paramsRequest.Timestamp));
                    }
                    catch
                    {
                        throw new Exception("Invalid Timestamp format");
                    }

                    //时间差为正负10分钟
                    long diffTimes = 10 * 60;
                    long _ts = VariableHelper.SaferequestInt64(_paramsRequest.Timestamp);
                    long _now = TimeHelper.DateTimeToUnixTimestamp(DateTime.Now);
                    if ((_ts > _now + diffTimes) || _ts < _now - diffTimes)
                    {
                        throw new Exception("Timestamp has expired!");
                    }
                }
                else
                {
                    throw new Exception("Timestamp is mandatory!");
                }
                //签名
                if (string.IsNullOrEmpty(_paramsRequest.Sign))
                {
                    throw new Exception("Sign is mandatory!");
                }

                var objAuthorizeUser = authorizeValidRequest.AuthorizeUsers.Where(p => p.AppID == _paramsRequest.Userid).SingleOrDefault();
                if (objAuthorizeUser != null)
                {
                    string[] ip_areas = _paramsRequest.Ip.Split('.');
                    //ip段限制,比如192.168.*.*
                    string ip_area = string.Empty;
                    for (int t = 0; t < ip_areas.Length; t++)
                    {
                        if (t == 0)
                        {
                            ip_area += ip_areas[0];
                        }
                        else if (t == 1)
                        {
                            ip_area += "." + ip_areas[1];
                        }
                    }
                    ip_area = ip_area + ".*.*";
                    //是否允许IP
                    if (objAuthorizeUser.Ips.Contains(_paramsRequest.Ip) || objAuthorizeUser.Ips.Contains(ip_area))
                    {
                        //去除签名字段
                        _params.Remove("sign");
                        //验证签名
                        string _appSign = _utilsHelper.CreateSign(_params, objAuthorizeUser.Token, _paramsRequest.Method);
                        if (_appSign == _paramsRequest.Sign)
                        {
                            //权限列表
                            var objInterfaces = _menuService.InterfaceOptions();
                            //查询当前action的ID
                            var objGroup = objInterfaces.Where(p => p.ControllerName.ToUpper() == authorizeValidRequest.ControllerName.ToUpper()).SingleOrDefault();
                            if (objGroup != null)
                            {
                                var objInterface = objGroup.Interfaces.Where(p => p.ActionName.ToUpper() == authorizeValidRequest.ActionName.ToUpper()).SingleOrDefault();
                                if (objInterface != null)
                                {
                                    if (objAuthorizeUser.Roles.Contains(objInterface.ID))
                                    {
                                        //返回信息
                                        _result = new AuthorizeValidResponse()
                                        {
                                            Result = true,
                                            Message = string.Empty,
                                            Params = _paramsRequest
                                        };
                                    }
                                    else
                                    {
                                        throw new Exception("Sorry,You have no access to perform this action!");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Sorry,You have no access to perform this action!");
                                }
                            }
                            else
                            {
                                throw new Exception("Sorry,You have no access to perform this action!");
                            }
                        }
                        else
                        {
                            throw new Exception("Incorrect Signature!");
                        }
                    }
                    else
                    {
                        throw new Exception("Access Denied!");
                    }
                }
                else
                {
                    throw new Exception("User ID does not exist!");
                }
            }
            catch (Exception ex)
            {
                //返回信息
                _result = new AuthorizeValidResponse()
                {
                    Result = false,
                    Message = ex.Message,
                    Params = _paramsRequest
                };
            }
            return _result;
        }
    }
}
