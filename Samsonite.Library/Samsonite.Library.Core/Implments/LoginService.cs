using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Core
{
    public class LoginService : AppUserCore, ILoginService
    {
        private IAppLogService _appLogService;
        private IHttpContextAccessor _httpContextAccessor;
        private appEntities _appDB;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private IResponseCookies _cookie_res => _httpContextAccessor.HttpContext.Response.Cookies;
        private GlobalConfigModel _globalConfig;
        public LoginService(IAppConfigService appConfigService, IAppLanguageService appLanguageService, IAppLogService appLogService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, appEntities appEntities) : base(appConfigService, appLanguageService, httpContextAccessor, memoryCache, appEntities)
        {
            _appLogService = appLogService;
            _httpContextAccessor = httpContextAccessor;
            _appDB = appEntities;
            _globalConfig = appConfigService.GetConfigCache().GlobalConfig;
        }

        /// <summary>
        /// 账号登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isRemember">默认保存1天，记住密码保存30天</param>
        /// <returns></returns>
        public PostResponse UserLogin(string userName, string password, bool isRemember)
        {
            PostResponse _result = new PostResponse();
            string _privateKey = string.Empty;
            string _ip = HttpHelper.GetRequestIP(_httpContextAccessor.HttpContext);
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();
            try
            {
                if (string.IsNullOrEmpty(userName)) throw new Exception(_languagePack["login_index_message_no_username"]);
                if (string.IsNullOrEmpty(password)) throw new Exception(_languagePack["login_index_message_no_password"]);

                UserInfo objUserInfo = _appDB.UserInfo.SingleOrDefault<UserInfo>(p => p.UserName == userName);
                if (objUserInfo != null)
                {
                    if (objUserInfo.Status == (int)UserStatus.Locked)
                    {
                        throw new Exception(_languagePack["login_index_message_username_lock"]);
                    }
                    else
                    {
                        _privateKey = objUserInfo.PrivateKey;
                        if (objUserInfo.Pwd.ToLower() == EncryptPassword(password, _privateKey).ToLower())
                        {
                            //查看密码是否已经过期
                            bool IsExpired = false;
                            if (objUserInfo.LastPwdEditTime == null)
                            {
                                IsExpired = true;
                            }
                            else
                            {
                                if (objUserInfo.LastPwdEditTime.Value.AddDays(_globalConfig.PwdValidityTime) < DateTime.Now)
                                {
                                    IsExpired = true;
                                }
                            }
                            //如果密码已经过期
                            if (IsExpired)
                            {
                                objUserInfo.Status = (int)UserStatus.ExpiredPwd;
                            }
                            //重置连续密码错误次数
                            objUserInfo.PwdErrorNum = 0;

                            //读取权重集合
                            var userWeights = (from ur in _appDB.UserRoles.Where(p => p.Userid == objUserInfo.Userid)
                                               join sr in _appDB.SysRole on ur.Roleid equals sr.Roleid
                                               select sr.RoleWeight).ToList();

                            //写入登录信息
                            UserSessionModel objUserSessionModel = new UserSessionModel();
                            objUserSessionModel.Userid = (int)objUserInfo.Userid;
                            objUserSessionModel.UserName = objUserInfo.UserName;
                            objUserSessionModel.Passwd = objUserInfo.Pwd;
                            objUserSessionModel.UserType = objUserInfo.Type;
                            objUserSessionModel.RoleWeight = userWeights.Any() ? userWeights.Min() : 99;
                            objUserSessionModel.UserStatus = objUserInfo.Status;
                            objUserSessionModel.UserPowers = this.GetUserFunctions(objUserSessionModel.Userid);
                            objUserSessionModel.DefaultLanguage = objUserInfo.DefaultLanguage;
                            //上机日志表
                            _appLogService.LoginLog(new WebAppLoginLog()
                            {
                                LoginStatus = true,
                                LoginType = 0,
                                Account = objUserInfo.UserName,
                                Password = EncryptPassword(password, _privateKey),
                                UserID = objUserInfo.Userid,
                                IP = _ip,
                                Remark = string.Empty,
                                AddTime = DateTime.Now
                            }); ;
                            //写入Session
                            this.SetUserSession(objUserSessionModel);
                            //写入Cookie
                            if (isRemember)
                            {
                                this.SetUserCookie(objUserSessionModel, _globalConfig.LoginCookieSaveTime);
                            }
                            //返回信息
                            _result.Result = true;
                            _result.Message = string.Empty;
                        }
                        else
                        {
                            //累计连续密码错误次数
                            objUserInfo.PwdErrorNum += 1;

                            //保存日志
                            _appLogService.LoginLog(new WebAppLoginLog()
                            {
                                LoginStatus = false,
                                Account = userName,
                                Password = EncryptPassword(password, _privateKey),
                                UserID = 0,
                                IP = _ip,
                                Remark = _languagePack["login_index_message_err"],
                                AddTime = DateTime.Now
                            });

                            //如果超过限制错误次数
                            int _maxErrorLockNum = _globalConfig.PwdErrorLockNum;
                            if (objUserInfo.PwdErrorNum >= _maxErrorLockNum)
                            {
                                objUserInfo.Status = (int)UserStatus.Locked;

                                //返回信息
                                _result.Result = false;
                                _result.Message = string.Format(_languagePack["login_index_message_err_lock"], _maxErrorLockNum);
                            }
                            else
                            {
                                //返回信息
                                _result.Result = false;
                                _result.Message = _languagePack["login_index_message_err"];
                            }
                        }
                        _appDB.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception(_languagePack["login_index_message_username_not_exist"]);
                }
            }
            catch (Exception ex)
            {
                //保存日志
                _appLogService.LoginLog(new WebAppLoginLog()
                {
                    LoginStatus = false,
                    Account = userName,
                    Password = EncryptPassword(password, _privateKey),
                    UserID = 0,
                    IP = _ip,
                    Remark = ex.Message,
                    AddTime = DateTime.Now
                });

                //返回信息
                _result.Result = false;
                _result.Message = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 用户退出
        /// </summary>
        public void UserLoginOut()
        {
            //清空session
            _session.Remove($"{_globalConfig.SessionKey}_LoginMessage");
            _session.Clear();
            //清空cookie
            _cookie_res.Delete($"{_globalConfig.CookieKey}_LoginMessage_Uname");
            _cookie_res.Delete($"{_globalConfig.CookieKey}_LoginMessage_Upass");
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse ForgetPassword(ForgetPasswordRequest request)
        {
            //加载语言包
            var _languagePack = this.GetCurrentLanguagePack();

            PostResponse _result = new PostResponse();
            try
            {
                if (string.IsNullOrEmpty(request.UserName)) throw new Exception(_languagePack["login_forget_message_no_username"]);
                if (string.IsNullOrEmpty(request.Email)) throw new Exception(_languagePack["login_forget_message_no_email"]);

                UserInfo objUserInfo = _appDB.UserInfo.SingleOrDefault<UserInfo>(p => p.UserName == request.UserName);
                if (objUserInfo != null)
                {
                    if (objUserInfo.Email.ToLower() == request.Email.ToLower())
                    {
                        //创建新密码
                        string _newPassword = this.CreateRandomPassword();
                        //发送邮件

                        //更新密码
                        objUserInfo.Pwd = EncryptPassword(_newPassword, objUserInfo.PrivateKey);
                        objUserInfo.LastPwdEditTime = DateTime.Now;
                        _appDB.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("您输入的账号和邮箱不匹配");
                    }
                }
                else
                {
                    throw new Exception("用户不存在");
                }

                //返回信息
                _result.Result = true;
                _result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                //返回信息
                _result.Result = false;
                _result.Message = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 创建随机密码
        /// </summary>
        /// <returns></returns>
        public string CreateRandomPassword()
        {
            string _result = string.Empty;
            string[] _array = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string[] _spArray = new string[] { "+", "_" };
            Random rnd = new Random();
            for (int i = 0; i < 7; i++)
            {
                rnd = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")) + i);
                _result += _array[rnd.Next(0, 35)];
            }
            _result = $"{_result.Substring(0, 4)}{_spArray[rnd.Next(0, 2)]}{_result.Substring(4)}";
            return _result;
        }

        /// <summary>
        /// 创建密钥
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string CreatePrivateKey(int length)
        {
            string _result = string.Empty;
            string[] _array = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                rnd = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")) + i);
                _result += _array[rnd.Next(0, 61)];
            }
            return _result;
        }

        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="password"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string EncryptPassword(string password, string key)
        {
            string _result = string.Empty;
            //先加盐
            _result = password + key;
            //再HMAC_SHA256加密
            _result = EncryptHelper.HMAC_SHA256(_result, key);
            return _result;
        }
    }
}