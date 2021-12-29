﻿using Newtonsoft.Json;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Business.Web.Basic
{
    public class ConfigService : AppConfigCore, IConfigService
    {
        private appEntities _appDB;
        public ConfigService(appEntities appEntities) : base(appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Update(ConfigUpdateRequest request)
        {
            try
            {
                //读取配置信息
                List<SysConfig> objSysConfigs = _appDB.SysConfig.ToList();

                //保存语言
                var _languageArray = JsonHelper.JsonDeserialize<List<string>>(request.LanguagePack);
                if (_languageArray.Count == 0)
                {
                    throw new Exception("请至少配置一种语言");
                }

                var _Config_lp = objSysConfigs.Where(p => p.ConfigKey == LANGUAGE_PACK_KEY).SingleOrDefault();
                if (_Config_lp != null)
                {
                    _Config_lp.ConfigValue = string.Join("|", _languageArray);
                }

                //邮件配置
                if (string.IsNullOrEmpty(request.MailStmp) || request.MailPort == 0 || string.IsNullOrEmpty(request.MailUserName) || string.IsNullOrEmpty(request.MailPassword))
                {
                    throw new Exception("邮件服务器信息配置不完整");
                }

                var _Configec = objSysConfigs.Where(p => p.ConfigKey == EMAIL_CONFIG_KEY).SingleOrDefault();
                if (_Configec != null)
                {
                    EmailModel objEmailModel = new EmailModel()
                    {
                        ServerHost = request.MailStmp,
                        Port = request.MailPort,
                        MailUsername = request.MailUserName,
                        MailPassword = request.MailPassword
                    };
                    _Configec.ConfigValue = JsonHelper.JsonSerialize(objEmailModel, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                }

                //短信配置
                if (string.IsNullOrEmpty(request.SMSHost) || string.IsNullOrEmpty(request.SMSAccount) || string.IsNullOrEmpty(request.SMSAuthToken) || string.IsNullOrEmpty(request.SMSSender))
                {
                    throw new Exception("短信服务器信息配置不完整");
                }

                var _Configsm = objSysConfigs.Where(p => p.ConfigKey == SMS_CONFIG_KEY).SingleOrDefault();
                if (_Configsm != null)
                {
                    SMSModel objSMSModel = new SMSModel()
                    {
                        ServerHost = request.SMSHost,
                        AccountSid = request.SMSAccount,
                        AuthToken = request.SMSAuthToken,
                        Sender = request.SMSSender,
                        SendPhoneNumber = string.Empty
                    };
                    _Configsm.ConfigValue = JsonHelper.JsonSerialize(objSMSModel, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                }

                //皮肤配置
                var _Configss = objSysConfigs.Where(p => p.ConfigKey == SKIN_STYLE_KEY).SingleOrDefault();
                if (_Configss != null)
                {
                    _Configss.ConfigValue = request.SkinStyle;
                }

                _appDB.SaveChanges();

                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = "配置信息保存成功"
                };

            }
            catch (Exception ex)
            {
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }
    }
}
