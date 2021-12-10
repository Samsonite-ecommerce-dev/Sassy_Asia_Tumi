using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Web;

public class BaseAuthorize : ActionFilterAttribute
{
    private IBaseService _baseService;
    private IAntiforgeryService _antiforgeryService;
    private Dictionary<string, string> _languagePack;
    public BaseAuthorize(IBaseService baseService, IAntiforgeryService antiforgeryService)
    {
        _baseService = baseService;
        _antiforgeryService = antiforgeryService;
        //加载语言包
        _languagePack = _baseService.CurrentLanguagePack();
    }

    /// <summary>
    /// 跳转到登入页面
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="filterContext"></param>
    protected void GoLogin(Type actionType, ActionExecutingContext filterContext)
    {
        if (actionType == typeof(JsonResult))
        {
            filterContext.Result = new JsonResult(new { result = false, msg = _languagePack["common_alert_no_login"] });
        }
        else if (actionType == typeof(ContentResult))
        {
            filterContext.Result = new ContentResult();
        }
        else if (actionType == typeof(FileResult))
        {
            filterContext.Result = null;
        }
        //框架页专用
        else if (actionType == typeof(IActionResult))
        {
            filterContext.Result = new RedirectResult("~/Login/Index");
        }
        else
        {
            filterContext.Result = new RedirectResult("~/Error/Index?Type=" + (int)ErrorType.LoginTimeOut);
        }
    }

    /// <summary>
    /// 跳转到修改密码页面
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="filterContext"></param>
    protected void GoEditPassword(Type actionType, ActionExecutingContext filterContext)
    {
        if (actionType == typeof(JsonResult))
        {
            filterContext.Result = new JsonResult(new { result = false, msg = _languagePack["common_alert_first_editpassword"] });
        }
        else if (actionType == typeof(ContentResult))
        {
            filterContext.Result = new ContentResult();
        }
        else if (actionType == typeof(FileResult))
        {
            filterContext.Result = null;
        }
        else
        {
            filterContext.Result = new RedirectResult("~/Home/EditPassword");
        }
    }

    /// <summary>
    /// 跳转到错误页面
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="filterContext"></param>
    /// <param name="msg"></param>
    protected void GoError(Type actionType, ActionExecutingContext filterContext, string msg)
    {
        if (actionType == typeof(JsonResult))
        {
            filterContext.Result = new JsonResult(new { result = false, msg = msg });
        }
        else if (actionType == typeof(ContentResult))
        {
            filterContext.Result = new ContentResult();
        }
        else if (actionType == typeof(FileResult))
        {
            filterContext.Result = null;
        }
        else
        {
            filterContext.Result = new RedirectResult("~/Error/Index?Type=" + (int)ErrorType.Other + "&Message=" + HttpUtility.UrlEncode(msg));
        }
    }

    /// <summary>
    /// 跨站请求伪造(CSRF)防御验证
    /// </summary>
    protected void VerificationToken()
    {
        try
        {
            var _validResult = _antiforgeryService.ValidateRequest();
            if (!_validResult)
            {
                throw new Exception(_languagePack["common_alert_no_law"]);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

