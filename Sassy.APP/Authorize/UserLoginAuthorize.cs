using Microsoft.AspNetCore.Mvc.Filters;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using System;
using System.Reflection;

public class UserLoginAuthorize : BaseAuthorize
{
    private IBaseService _baseService;
    public UserLoginAuthorize(IBaseService baseService,IAntiforgeryService  antiforgeryService) : base(baseService, antiforgeryService)
    {
        _baseService = baseService;
    }

    //默认页面跳转模式
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var _actionName = filterContext.RouteData.Values["action"].ToString();
        //Action类型
        Type _actionType = filterContext.Controller.GetType().GetMethod(_actionName).ReturnType;
        bool _isAntiforgeryToken = false;
        var _customAttributes = filterContext.Controller.GetType().GetMethod(_actionName).GetCustomAttribute(typeof(AuthorizePropertyAttribute));
        if (_customAttributes != null)
        {
            _isAntiforgeryToken = ((AuthorizePropertyAttribute)_customAttributes).IsAntiforgeryToken;
        }

        try
        {
            UserSessionModel objUserSession = _baseService.CurrentLoginUser;
            if (objUserSession != null)
            {
                //开启CSRF攻击防御
                if (_isAntiforgeryToken)
                {
                    this.VerificationToken();
                }
            }
            else
            {
                this.GoLogin(_actionType, filterContext);
            }
        }
        catch (Exception ex)
        {
            this.GoError(_actionType, filterContext, ex.Message);
        }

        base.OnActionExecuting(filterContext);
    }
}

