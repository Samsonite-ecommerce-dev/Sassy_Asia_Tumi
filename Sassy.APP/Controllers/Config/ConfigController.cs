using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Web.Basic;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class ConfigController : BaseController
    {
        private IConfigService _configService;
        private IAppConfigService _appConfigService;
        private IAppLanguageService _appLanguageService;
        public ConfigController(IBaseService baseService, IConfigService configService, IAppConfigService appConfigService, IAppLanguageService appLanguageService) : base(baseService)
        {
            _configService = configService;
            _appConfigService = appConfigService;
            _appLanguageService = appLanguageService;
        }

        #region 配置
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public JsonResult Index_Info()
        {
            //读取配置信息
            var objAppConfig = _appConfigService.GetConfig();

            //返回数据
            return Json(new
            {
                //菜单栏
                navMenu = this.MenuBar(),
                //功能权限
                userAuthorization = this.FunctionPowers(),
                //语言包集合
                languageList = _appLanguageService.LanguageTypeOption().Select(p => new
                {
                    label = p.LanguageName,
                    value = p.LanguageCode
                }).ToList(),
                model = new
                {
                    languagePack = _appLanguageService.LanguageTypeOption().Where(p => objAppConfig.SysConfig.LanguagePacks.Contains(p.ID)).Select(o => o.LanguageCode).ToList(),
                    mailStmp = objAppConfig.SysConfig.EmailConfig.ServerHost,
                    mailPort = objAppConfig.SysConfig.EmailConfig.Port,
                    mailUserName = objAppConfig.SysConfig.EmailConfig.MailUsername,
                    mailPassword = objAppConfig.SysConfig.EmailConfig.MailPassword,
                    smsHost = objAppConfig.SysConfig.SMSConfig.ServerHost,
                    smsAccount = objAppConfig.SysConfig.SMSConfig.AccountSid,
                    smsAuthToken = objAppConfig.SysConfig.SMSConfig.AuthToken,
                    smsSender = objAppConfig.SysConfig.SMSConfig.Sender,
                    skinStyle = objAppConfig.SysConfig.SkinStyle.ToString()
                }
            });
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public JsonResult Index_Message(ConfigUpdateRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _configService.Update(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 重置系统配置缓存
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Reset_Message()
        {
            try
            {
                _appConfigService.ResetConfigCache();
                //返回信息
                return Json(new
                {
                    result = true,
                    msg = "系统配置更新成功"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false,
                    msg = ex.Message
                });
            }
        }
        #endregion
    }
}