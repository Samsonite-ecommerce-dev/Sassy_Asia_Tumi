using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Samsonite.Library.API.Utils;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.WebApi;
using Samsonite.Library.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Samsonite.Library.API
{
    public class CustomAuthorizeFilter : ActionFilterAttribute
    {
        private IApiService _apiService;
        private AuthorizeHelper _authorizeHelper;
        private UtilsHelper _utilsHelper;
        private appEntities _appDB;
        private logEntities _logDB;
        private List<AuthorizeUser> _authorizeUsers;
        private string _requestD = string.Empty;
        public CustomAuthorizeFilter(IApiService apiService, appEntities appEntities, logEntities logEntities)
        {
            _apiService = apiService;
            _authorizeHelper = new AuthorizeHelper(_apiService, appEntities);
            _utilsHelper = new UtilsHelper();
            _appDB = appEntities;
            _logDB = logEntities;
            //初始化账号信息
            _authorizeUsers = _authorizeHelper.GetApiAccounts();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            WebApiAccessLog webApiAccessLog = new WebApiAccessLog();
            var _controllerName = context.RouteData.Values["controller"].ToString();
            var _actionName = context.RouteData.Values["action"].ToString();
            var _postBody = string.Empty;
            _requestD = _utilsHelper.GreateRequestID();
            try
            {
                var _result = _authorizeHelper.VisitValid(context, _authorizeUsers);
                ////测试用-------------
                //_result.Result = true;
                ////-------------------
                //访问信息
                webApiAccessLog.LogType = _apiService.GetAPIType(_controllerName);
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
                context.Result = new JsonResult(new ApiResponse
                {
                    RequestID = _requestD,
                    Code = (int)ApiResultCode.Fail,
                    Message = ex.Message
                });
            }
            //添加日志
            _logDB.Add(webApiAccessLog);
            _logDB.SaveChanges();
            //文件日志
            if (GlobalConfig.IsApiDebugLog)
            {
                string[] _logs = new string[] { $"Client Ip:{webApiAccessLog.Ip}", $"Request Uri:{webApiAccessLog.Url}", $"Request Body:{_postBody}" };
                _utilsHelper.WriteLogger(_controllerName, _actionName, _requestD, _logs);
            }

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
            if (GlobalConfig.IsApiDebugLog)
            {
                _utilsHelper.WriteLogger(_controllerName, _actionName, _requestD, new string[] { $"ApiResult: {_contextResultLog}", "********************************************************************************", "\r" });
            }

            base.OnActionExecuted(context);
        }
    }
}
