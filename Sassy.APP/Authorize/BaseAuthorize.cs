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
    /// <param name="objActionType"></param>
    /// <param name="objFilterContext"></param>
    protected void GoLogin(Type objActionType, ActionExecutingContext objFilterContext)
    {
        if (objActionType == typeof(JsonResult))
        {
            objFilterContext.Result = new JsonResult(new { result = false, msg = _languagePack["common_alert_no_login"] });
        }
        else if (objActionType == typeof(ContentResult))
        {
            objFilterContext.Result = new ContentResult();
        }
        else if (objActionType == typeof(FileResult))
        {
            objFilterContext.Result = null;
        }
        //框架页专用
        else if (objActionType == typeof(IActionResult))
        {
            objFilterContext.Result = new RedirectResult("~/Login/Index");
        }
        else
        {
            objFilterContext.Result = new RedirectResult("~/Error/Index?Type=" + (int)ErrorType.LoginTimeOut);
        }
    }

    /// <summary>
    /// 跳转到修改密码页面
    /// </summary>
    /// <param name="objActionType"></param>
    /// <param name="objActionExecutingContext"></param>
    protected void GoEditPassword(Type objActionType, ActionExecutingContext objFilterContext)
    {
        if (objActionType == typeof(JsonResult))
        {
            objFilterContext.Result = new JsonResult(new { result = false, msg = _languagePack["common_alert_first_editpassword"] });
        }
        else if (objActionType == typeof(ContentResult))
        {
            objFilterContext.Result = new ContentResult();
        }
        else if (objActionType == typeof(FileResult))
        {
            objFilterContext.Result = null;
        }
        else
        {
            objFilterContext.Result = new RedirectResult("~/Home/EditPassword");
        }
    }

    /// <summary>
    /// 跳转到错误页面
    /// </summary>
    /// <param name="objActionType"></param>
    /// <param name="objobjFilterContext"></param>
    /// <param name="objMsg"></param>
    protected void GoError(Type objActionType, ActionExecutingContext objFilterContext, string objMsg)
    {
        if (objActionType == typeof(JsonResult))
        {
            objFilterContext.Result = new JsonResult(new { result = false, msg = objMsg });
        }
        else if (objActionType == typeof(ContentResult))
        {
            objFilterContext.Result = new ContentResult();
        }
        else if (objActionType == typeof(FileResult))
        {
            objFilterContext.Result = null;
        }
        else
        {
            objFilterContext.Result = new RedirectResult("~/Error/Index?Type=" + (int)ErrorType.Other + "&Message=" + HttpUtility.UrlEncode(objMsg));
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

