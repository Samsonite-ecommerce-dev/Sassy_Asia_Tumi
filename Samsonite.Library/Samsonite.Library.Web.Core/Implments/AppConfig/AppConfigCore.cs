using Microsoft.Extensions.Configuration;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Samsonite.Library.Web.Core
{
    public class AppConfigCore
    {
        private appEntities _appDB;
        public AppConfigCore(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 全局配置
        /// </summary>
        public GlobalConfigModel GetGlobalConfig()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //创建配置根对象
            var configurationRoot = builder.Build();
            //获取配置根目录
            var rootSection = configurationRoot.GetSection("SiteConfig");

            return new GlobalConfigModel()
            {
                HttpURL = rootSection.GetSection("httpURL").Value,
                SessionKey = $"{rootSection.GetSection("basePrefix").Value}_SESSION_SAMSONITE_OMS",
                CookieKey = $"{rootSection.GetSection("basePrefix").Value}_COOKIE_SAMSONITE_OMS",
                CacheKey = $"{rootSection.GetSection("basePrefix").Value}_CacheKey_SAMSONITE_OMS",
                UploadFilePath = "/UploadFile/Temporary",
                UploadCachePath = "/UploadFile/CacheFile",
                ImagePath = $"{rootSection.GetSection("imagePath").Value}",
                LoginCookieSaveTime = 1,
                PwdPastNum = 5,
                PwdValidityTime = 90,
                PwdErrorLockNum = 5,
                SitePhysicalPath = rootSection.GetSection("sitePhysicalPATH").Value
            };
        }

        #region 配置关键字
        /// <summary>
        /// 语言包配置
        /// </summary>
        public const string LANGUAGE_PACK_KEY = "LANGUAGE_PACK";

        /// <summary>
        /// 邮件服务器配置
        /// </summary>
        public const string EMAIL_CONFIG_KEY = "EMAIL_CONFIG";

        /// <summary>
        /// 短信配置
        /// </summary>
        public const string SMS_CONFIG_KEY = "SMS_CONFIG";

        /// <summary>
        /// 皮肤
        /// </summary>
        public const string SKIN_STYLE_KEY = "SKIN_STYLE";
        #endregion 

        /// <summary>
        /// 读取系统配置信息
        /// </summary>
        /// <returns></returns>
        public SysConfigModel GetSysConfig()
        {
            SysConfigModel _result = new SysConfigModel();
            //读取所有配置信息
            List<SysConfig> objSysConfigs = _appDB.SysConfig.ToList();
            //语言包配置
            _result.LanguagePacks = GetLanguagePackConfig(objSysConfigs);
            //邮件配置
            _result.EmailConfig = GetEmailConfig(objSysConfigs);
            //短信配置
            _result.SMSConfig = GetSMSConfig(objSysConfigs);
            //样式配置
            _result.SkinStyle = GetSkinStyleConfig(objSysConfigs);
            return _result;
        }

        /// <summary>
        /// 获取语言包配置
        /// </summary>
        /// <param name="sysConfigList"></param>
        /// <returns></returns>
        private List<int> GetLanguagePackConfig(List<SysConfig> sysConfigList)
        {
            List<int> _result = new List<int>();
            SysConfig objSysConfig = sysConfigList.Where(p => p.ConfigKey.ToUpper() == LANGUAGE_PACK_KEY).SingleOrDefault();
            if (objSysConfig != null)
            {
                //如果值为空,那么Split之后仍然会生成一个数组
                if (!string.IsNullOrEmpty(objSysConfig.ConfigValue))
                {
                    //语言集合
                    var objLanguagePacks = _appDB.LanguageType.ToList();

                    string[] _value = objSysConfig.ConfigValue.Split('|');
                    foreach (string _str in _value)
                    {
                        var _o = objLanguagePacks.Where(p => p.LanguageCode == _str).SingleOrDefault();
                        if (_o != null)
                        {
                            _result.Add(_o.ID);
                        }
                    }
                }
            }
            return _result;
        }

        /// <summary>
        /// 邮件配置
        /// </summary>
        /// <param name="sysConfigList"></param>
        private EmailModel GetEmailConfig(List<SysConfig> sysConfigList)
        {
            EmailModel _result = new EmailModel();
            SysConfig objSysConfig = sysConfigList.Where(p => p.ConfigKey.ToUpper() == EMAIL_CONFIG_KEY).SingleOrDefault();

            if (objSysConfig != null)
            {
                _result = JsonSerializer.Deserialize<EmailModel>(objSysConfig.ConfigValue);
            }
            return _result;
        }

        /// <summary>
        /// 短信配置
        /// </summary>
        /// <param name="sysConfigList"></param>
        private SMSModel GetSMSConfig(List<SysConfig> sysConfigList)
        {
            SMSModel _result = new SMSModel();
            SysConfig objSysConfig = sysConfigList.Where(p => p.ConfigKey.ToUpper() == SMS_CONFIG_KEY).SingleOrDefault();

            if (objSysConfig != null)
            {
                _result = JsonSerializer.Deserialize<SMSModel>(objSysConfig.ConfigValue);
            }
            return _result;
        }

        /// <summary>
        /// 皮肤配置
        /// </summary>
        /// <param name="sysConfigList"></param>
        private string GetSkinStyleConfig(List<SysConfig> sysConfigList)
        {
            string _result = string.Empty;
            SysConfig objSysConfig = sysConfigList.Where(p => p.ConfigKey.ToUpper() == SKIN_STYLE_KEY).SingleOrDefault();
            if (objSysConfig != null)
            {
                _result = objSysConfig.ConfigValue;
            }
            return _result;
        }
    }
}
