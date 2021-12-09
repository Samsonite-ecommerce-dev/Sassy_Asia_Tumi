using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Basic;
using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class SystemLogController : BaseController
    {
        private ISystemLogService _systemLogService;
        private IUsersService _usersService;
        public SystemLogController(IBaseService baseService, ISystemLogService systemLogService, IUsersService usersService) : base(baseService)
        {
            _systemLogService = systemLogService;
            _usersService = usersService;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type)
        {
            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers()
                });
            }
            else
            {
                return Json(new { });
            }
        }
        #endregion

        #region 查询
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public JsonResult Index_Message(SystemLogSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _systemLogService.GetQuery(request);
            //账号列表
            List<UserInfo> objUsers = _usersService.GetUsers(_list.Items.Select(p => p.UserID).ToList());
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.LogID,
                           s1 = (objUsers.Where(p => p.Userid == dy.UserID).SingleOrDefault() != null) ? objUsers.Where(p => p.Userid == dy.UserID).SingleOrDefault().RealName : string.Empty,
                           s2 = dy.UserIP,
                           s3 = dy.LogMessage,
                           s4 = dy.AddTime.ToString("yyyy-MM-dd HH:mm:ss")
                       }
            };
            return Json(_result);
        }
        #endregion
    }
}