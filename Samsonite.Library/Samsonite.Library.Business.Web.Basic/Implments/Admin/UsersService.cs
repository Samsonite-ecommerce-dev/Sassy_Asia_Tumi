using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Samsonite.Library.Business.Web.Basic
{
    public class UsersService : IUsersService
    {
        private IBaseService _baseService;
        private ILoginService _loginService;
        private IAppLogService _appLogService;
        private appEntities _appDB;
        private logEntities _logDB;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private IHttpContextAccessor _httpContextAccessor;
        private AppConfigModel _currentApplicationConfig;
        public UsersService(IBaseService baseService, ILoginService loginService, IAppLogService appLogService, appEntities appEntities, logEntities logEntities, IHttpContextAccessor httpContextAccessor)
        {
            _baseService = baseService;
            _loginService = loginService;
            _appLogService = appLogService;
            _appDB = appEntities;
            _logDB = logEntities;
            _httpContextAccessor = httpContextAccessor;
            _currentApplicationConfig = _baseService.CurrentApplicationConfig;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<UserInfo> GetQuery(UsersSearchRequest request)
        {
            QueryResponse<UserInfo> _result = new QueryResponse<UserInfo>();
            var _list = _appDB.UserInfo.AsQueryable();
            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.UserName.Contains(request.Keyword) || p.RealName.Contains(request.Keyword));
            }

            if (request.UserType > 0)
            {
                _list = _list.Where(p => p.Type == request.UserType);
            }

            if (request.Status == 1)
            {
                _list = _list.Where(p => p.Status == (int)UserStatus.Locked);
            }
            else
            {
                _list = _list.Where(p => (new List<int>() { (int)UserStatus.Normal, (int)UserStatus.ExpiredPwd }).Contains(p.Status));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderByDescending(p => p.Userid).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(UsersAddRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    var _rolesArray = JsonSerializer.Deserialize<List<int>>(request.Roles);

                    if (string.IsNullOrEmpty(request.UserName))
                    {
                        throw new Exception(_languagePack["users_edit_message_no_email"]);
                    }
                    else
                    {
                        if (!CheckHelper.CheckEmail(request.UserName))
                        {
                            throw new Exception(_languagePack["users_edit_message_email_error"]);
                        }
                    }

                    UserInfo objUserInfo = _appDB.UserInfo.Where(p => p.UserName == request.UserName).SingleOrDefault();
                    if (objUserInfo != null)
                    {
                        throw new Exception(_languagePack["users_edit_message_exist_username"]);
                    }

                    if (string.IsNullOrEmpty(request.Password))
                    {
                        throw new Exception(_languagePack["users_edit_message_no_password"]);
                    }

                    if (!CheckHelper.ValidPassword(request.Password))
                    {
                        throw new Exception(_languagePack["users_editpassword_message_password_error"]);
                    }

                    if (_rolesArray.Count == 0)
                    {
                        throw new Exception(_languagePack["users_edit_message_select_role"]);
                    }

                    string _privateKey = _loginService.CreatePrivateKey(12);
                    string _encryptPassword = _loginService.EncryptPassword(request.Password, _privateKey);
                    UserInfo objData = new UserInfo()
                    {
                        UserName = request.UserName,
                        RealName = request.RealName,
                        Pwd = _encryptPassword,
                        Email = request.UserName,
                        Remark = request.Memo,
                        DefaultLanguage = request.DefaultLanguage,
                        Status = (int)UserStatus.ExpiredPwd,
                        Type = request.UserType,
                        PrivateKey = _privateKey,
                        PwdErrorNum = 0,
                        LastPwdEditTime = null,
                        AddTime = DateTime.Now
                    };
                    _appDB.UserInfo.Add(objData);
                    _appDB.SaveChanges();
                    //添加相关角色组
                    UserRoles objUserRoles = new UserRoles();
                    //允许添加的权限组
                    List<SysRole> objSysRoleList = _appDB.SysRole.Where(p => _rolesArray.Contains(p.Roleid) && p.RoleWeight >= _baseService.CurrentLoginUser.RoleWeight).ToList();
                    foreach (var item in objSysRoleList)
                    {
                        objUserRoles = new UserRoles()
                        {
                            Userid = objData.Userid,
                            Roleid = item.Roleid,
                        };
                        _appDB.UserRoles.Add(objUserRoles);
                    }
                    _appDB.SaveChanges();
                    Trans.Commit();
                    //添加密码日志
                    _appLogService.PasswordLog(new WebAppPasswordLog()
                    {
                        Account = request.UserName,
                        Password = _encryptPassword,
                        UserID = objData.Userid,
                        IP = HttpHelper.GetRequestIP(_httpContextAccessor.HttpContext),
                        Remark = string.Empty,
                        AddTime = DateTime.Now
                    });
                    //添加日志
                    _appLogService.InsertLog<UserInfo>(objData, objData.Userid.ToString());
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
        public PostResponse Edit(UsersEditRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    var _rolesArray = JsonSerializer.Deserialize<List<int>>(request.Roles);

                    if (_rolesArray.Count == 0)
                    {
                        throw new Exception(_languagePack["users_edit_message_select_role"]);
                    }

                    UserInfo objData = _appDB.UserInfo.Where(p => p.Userid == request.ID).SingleOrDefault();
                    if (objData != null)
                    {
                        objData.RealName = request.RealName;
                        objData.DefaultLanguage = request.DefaultLanguage;
                        objData.Remark = request.Memo;
                        if (objData.Status == (int)UserStatus.ExpiredPwd)
                        {
                            objData.Status = (!request.Status) ? (int)UserStatus.Locked : (int)UserStatus.ExpiredPwd;
                        }
                        else
                        {
                            objData.Status = (!request.Status) ? (int)UserStatus.Locked : (int)UserStatus.Normal;
                        }

                        objData.Type = request.UserType;
                        _appDB.SaveChanges();
                        //删除原角色组
                        _appDB.Database.ExecuteSqlRaw("delete from UserRoles where Userid={0}", objData.Userid);
                        //添加相关角色组
                        UserRoles objUserRoles = new UserRoles();
                        //允许添加的权限组
                        List<SysRole> objSysRoleList = _appDB.SysRole.Where(p => _rolesArray.Contains(p.Roleid) && p.RoleWeight >= _baseService.CurrentLoginUser.RoleWeight).ToList();
                        foreach (var item in objSysRoleList)
                        {
                            objUserRoles = new UserRoles()
                            {
                                Userid = (int)objData.Userid,
                                Roleid = item.Roleid,
                            };
                            _appDB.UserRoles.Add(objUserRoles);
                        }
                        _appDB.SaveChanges();
                        Trans.Commit();
                        //添加日志
                        _appLogService.UpdateLog<UserInfo>(objData, objData.Userid.ToString());
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
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception(_languagePack["common_data_need_one"]);
                }

                UserInfo objUserInfo = new UserInfo();
                foreach (var id in ids)
                {
                    objUserInfo = _appDB.UserInfo.Where(p => p.Userid == id).SingleOrDefault();
                    if (objUserInfo != null)
                    {
                        objUserInfo.Status = (int)UserStatus.Locked;
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, _languagePack["common_data_no_exsit"]));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("UserInfo", string.Join(",", ids));
                //返回信息
                return new PostResponse
                {
                    Result = true,
                    Message = _languagePack["common_data_delete_success"]
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
        /// 恢复
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Restore(int[] ids)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception(_languagePack["common_data_need_one"]);
                }

                UserInfo objUserInfo = new UserInfo();
                foreach (var id in ids)
                {
                    objUserInfo = _appDB.UserInfo.Where(p => p.Userid == id).SingleOrDefault();
                    if (objUserInfo != null)
                    {
                        objUserInfo.Status = (int)UserStatus.Normal;
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, _languagePack["common_data_no_exsit"]));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.RestoreLog("UserInfo", string.Join(",", ids));
                //返回信息
                return new PostResponse
                {
                    Result = true,
                    Message = _languagePack["common_data_recover_success"]
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
        /// 编辑密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse EditPassword(UsersPasswordEditRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new Exception(_languagePack["users_editpassword_message_no_password"]);
                }

                if (!CheckHelper.ValidPassword(request.Password))
                {
                    throw new Exception(_languagePack["users_editpassword_message_password_error"]);
                }

                if (string.IsNullOrEmpty(request.SurePassword))
                {
                    throw new Exception(_languagePack["users_editpassword_message_no_reset_password"]);
                }

                if (request.Password != request.SurePassword)
                {
                    throw new Exception(_languagePack["users_editpassword_message_not_same"]);
                }

                UserInfo objData = _appDB.UserInfo.Where(p => p.Userid == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    string _encryptPassword = _loginService.EncryptPassword(request.Password, objData.PrivateKey);
                    //检查是否存在N次密码修改存在重复
                    List<string> objWebAppPasswordLogs = _logDB.WebAppPasswordLog.Where(p => p.UserID == objData.Userid).OrderByDescending(p => p.LogID).Select(p => p.Password).Take(_baseService.CurrentApplicationConfig.GlobalConfig.PwdPastNum).ToList();
                    if (objWebAppPasswordLogs.Contains(_encryptPassword))
                    {
                        throw new Exception(_languagePack["users_editpassword_message_password_repeat_error"]);
                    }

                    objData.Pwd = _encryptPassword;
                    objData.PwdErrorNum = 0;
                    objData.LastPwdEditTime = DateTime.Now;
                    _appDB.SaveChanges();
                    //添加密码日志
                    _appLogService.PasswordLog(new WebAppPasswordLog()
                    {
                        Account = objData.UserName,
                        Password = _encryptPassword,
                        UserID = objData.Userid,
                        IP = HttpHelper.GetRequestIP(_httpContextAccessor.HttpContext),
                        Remark = string.Empty,
                        AddTime = DateTime.Now
                    });
                    //添加日志
                    _appLogService.UpdateLog<UserInfo>(objData, objData.Userid.ToString());
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
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 根据ID获取会员
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserInfo GetUser(int userID)
        {
            return _appDB.UserInfo.Where(p => p.Userid == userID).SingleOrDefault();
        }

        /// <summary>
        /// 根据ID获取会员集合
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        public List<UserInfo> GetUsers(List<int> userIDs)
        {
            return _appDB.UserInfo.Where(p => userIDs.Contains(p.Userid)).ToList();
        }
    }
}
