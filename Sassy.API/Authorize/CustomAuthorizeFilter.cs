using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Samsonite.Library.Core.WebApi;
using Samsonite.Library.Core.WebApi.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Sassy.API
{
    public class CustomAuthorizeFilter : ActionFilterAttribute
    {
        private IApiBaseService _apiBaseService;
        private IAuthorizeService _authorizeService;
        private logEntities _logDB;
        private List<AuthorizeUser> _authorizeUsers;
        private string _requestD = string.Empty;
        public CustomAuthorizeFilter(IApiBaseService apiBaseService, IAuthorizeService authorizeService, logEntities logEntities)
        {
            _apiBaseService = apiBaseService;
            _authorizeService = authorizeService;
            _logDB = logEntities;
            //初始化账号信息
            _authorizeUsers = _authorizeService.GetApiAccounts();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            WebApiAccessLog webApiAccessLog = new WebApiAccessLog();
            //控制器
            var _controllerName = context.RouteData.Values["controller"].ToString();
            var _actionName = context.RouteData.Values["action"].ToString();
            //action对象
            string _actionRoute = string.Empty;
            var _objAction = context.Controller.GetType().GetMethod(_actionName);
            var _customAttributes = _objAction.GetCustomAttribute(typeof(RouteAttribute));
            if (_customAttributes != null)
            {
                _actionRoute = ((RouteAttribute)_customAttributes).Template;
            }
            //访问地址
            string _requestUrl = $"{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";
            //post的body中的内容
            var _postBody = string.Empty;
            //访问者IP
            string _requestIP = HttpHelper.GetRequestIP(context.HttpContext);
            //基础参数
            var _requestParams = context.HttpContext.Request.Query.ToDictionary(p => p.Key, p => p.Value.ToString());
            //context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            //using (var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8))
            //{
            //    _paramsRequest.PostBody = reader.ReadToEndAsync().Result;
            //}
            _requestD = _apiBaseService.GreateRequestID();

            //文件日志
            if (_apiBaseService.IsApiDebugLog())
            {
                string[] _logs = new string[] { $"Client Ip:{_requestIP}", $"Request Uri:{_requestUrl}", $"Request Body:{_postBody}" };
                _apiBaseService.WriteLogger(_controllerName, _actionName, _requestD, _logs);
            }

            try
            {
                var _result = _authorizeService.VisitValid(new AuthorizeValidRequest()
                {
                    ControllerName = _controllerName,
                    ActionName = _actionName,
                    ActionRoute = _actionRoute,
                    RequestUrl = _requestUrl,
                    PostBody = _postBody,
                    RequestIp = _requestIP,
                    RequestParam = _requestParams,
                    AuthorizeUsers = _authorizeUsers
                });
                //注:测试时跳过验证使用代码********************************************************************************//
                //_result.Result = true;
                //*********************************************************************************************************//
                //访问信息
                webApiAccessLog.LogType = _apiBaseService.GetAPIType(_controllerName);
                webApiAccessLog.Url = _result.Params.Url;
                webApiAccessLog.RequestID = _requestD;
                webApiAccessLog.UserID = _result.Params.Userid;
                webApiAccessLog.Ip = _result.Params.Ip;
                webApiAccessLog.CreateTime = DateTime.Now;
                //Body参数
                _postBody = _result.Params.PostBody;
                //返回信息
                if (_result.Result)
                {
                    webApiAccessLog.State = true;
                    webApiAccessLog.Remark = _result.Message;
                }
                else
                {
                    throw new Exception(_result.Message);
                }
            }
            catch (Exception ex)
            {
                //日志
                webApiAccessLog.State = false;
                webApiAccessLog.Remark = ex.Message;

                //返回错误信息
                var _contextResult = new JsonResult(new ApiResponse
                {
                    RequestID = _requestD,
                    Code = (int)ApiResultCode.SystemError,
                    Message = ex.Message
                });
                context.Result = _contextResult;

                //文件日志
                if (_apiBaseService.IsApiDebugLog())
                {
                    //Json转化返回信息
                    var _contextResultLog = JsonSerializer.Serialize(_contextResult, new JsonSerializerOptions
                    {
                        IgnoreNullValues = true
                    });
                    _apiBaseService.WriteLogger(_controllerName, _actionName, _requestD, new string[] { $"ApiResult: {_contextResultLog}", "********************************************************************************", "\r" });
                }
            }
            //添加日志
            _logDB.Add(webApiAccessLog);
            _logDB.SaveChanges();

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var _controllerName = context.RouteData.Values["controller"].ToString();
            var _actionName = context.RouteData.Values["action"].ToString();
            //日志内容信息
            string _contextResultLog = string.Empty;
            if (context.Result != null)
            {
                var _result = (ObjectResult)context.Result;
                var _statusCode = _result.StatusCode;
                //获取由IAction返回的信息:_result.Value
                if (_result.Value.GetType().Equals((new ApiResponse()).GetType()))
                {
                    //添加requestID
                    ((ApiResponse)_result.Value).RequestID = _requestD;
                }
                else if (_result.Value.GetType().Equals((new ApiGetResponse()).GetType()))
                {
                    //添加requestID
                    ((ApiGetResponse)_result.Value).RequestID = _requestD;
                }
                else if (_result.Value.GetType().Equals((new ApiPostResponse()).GetType()))
                {
                    //添加requestID
                    ((ApiPostResponse)_result.Value).RequestID = _requestD;
                }
                else if (_result.Value.GetType().Equals((new ApiPageResponse()).GetType()))
                {
                    //添加requestID
                    ((ApiPageResponse)_result.Value).RequestID = _requestD;
                }

                //Json转化返回信息
                _contextResultLog = JsonSerializer.Serialize(_result.Value, new JsonSerializerOptions
                {
                    IgnoreNullValues = true
                });
            }

            //文件日志
            if (_apiBaseService.IsApiDebugLog())
            {
                _apiBaseService.WriteLogger(_controllerName, _actionName, _requestD, new string[] { $"ApiResult: {_contextResultLog}", "********************************************************************************", "\r" });
            }

            base.OnActionExecuted(context);
        }
    }
}
