using Microsoft.AspNetCore.Mvc.Filters;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class UserPowerAuthorize : BaseAuthorize
{
    private IBaseService _baseService;
    private appEntities _appDB;
    public UserPowerAuthorize(IBaseService baseService, IAntiforgeryService antiforgeryService, appEntities appEntities) : base(baseService, antiforgeryService)
    {
        _baseService = baseService;
        _appDB = appEntities;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //加载语言包
        var _languagePack = _baseService.CurrentLanguagePack;

        UserSessionModel objUserSessionModel = _baseService.CurrentLoginUser;
        //控制器名称
        string _controller = filterContext.RouteData.Values["controller"].ToString();
        var _actionName = filterContext.RouteData.Values["action"].ToString();
        //Action类型
        Type _actionType = filterContext.Controller.GetType().GetMethod(_actionName).ReturnType;
        //Action值
        //1.以自定义AuthorizePropertyAttribute传递的参数值为准
        //2.如果没有传递参数,则默则以路由地址中的Action前缀为准
        string _action = string.Empty;
        bool _isAntiforgeryToken = false;
        var _customAttributes = filterContext.Controller.GetType().GetMethod(_actionName).GetCustomAttribute(typeof(AuthorizePropertyAttribute));
        if (_customAttributes != null)
        {
            _action = ((AuthorizePropertyAttribute)_customAttributes).Action;
            _isAntiforgeryToken = ((AuthorizePropertyAttribute)_customAttributes).IsAntiforgeryToken;
        }

        //如果没有设置Action值
        if (string.IsNullOrEmpty(_action))
        {
            string _actionValue = filterContext.RouteData.Values["action"].ToString();
            if (_actionValue.IndexOf("_") > -1)
            {
                //如果是数据处理页面,则取下划线前面的功能标识
                _action = _actionValue.Split('_')[0];
            }
            else
            {
                _action = _actionValue;
            }
        }

        try
        {
            if (objUserSessionModel != null)
            {
                //1.首次登入没有修改密码
                //2.90天内没有修改密码
                if (objUserSessionModel.UserStatus == (int)UserStatus.ExpiredPwd)
                {
                    this.GoEditPassword(_actionType, filterContext);
                }
                else
                {
                    //开启CSRF攻击防御
                    if (_isAntiforgeryToken)
                    {
                        this.VerificationToken();
                    }

                    //当前权限
                    List<UserSessionModel.UserPower> objUserPowers = objUserSessionModel.UserPowers;
                    //获取权限id
                    SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.FuncSign.ToLower() == _controller.ToLower()).SingleOrDefault();
                    if (objSysFunction != null)
                    {
                        var _O = objUserPowers.Where(p => p.FunctionID == objSysFunction.Funcid).FirstOrDefault();
                        if (_O != null)
                        {
                            //比较操作权限
                            if (!_O.FunctionPower.Contains(_action.ToLower()))
                            {
                                throw new Exception(_languagePack["common_alert_no_permission"]);
                            }
                        }
                        else
                        {
                            throw new Exception(_languagePack["common_alert_no_permission"]);
                        }
                    }
                    else
                    {
                        throw new Exception(_languagePack["common_alert_no_permission"]);
                    }
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

