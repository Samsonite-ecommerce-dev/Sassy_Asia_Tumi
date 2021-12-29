using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Web.Basic;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class FunctionGroupController : BaseController
    {
        private IFunctionGroupService _functionGroupService;
        private appEntities _appDB;
        public FunctionGroupController(IBaseService baseService, IFunctionGroupService functionGroupService, appEntities appEntities) : base(baseService)
        {
            _functionGroupService = functionGroupService;
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
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
            else if (type == "edit")
            {
                //过滤参数
                int _groupID = VariableHelper.SaferequestInt(id);
                SysFunctionGroup objSysFunctionGroup = _appDB.SysFunctionGroup.Where(p => p.Groupid == _groupID).SingleOrDefault();
                if (objSysFunctionGroup != null)
                {
                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objSysFunctionGroup.Groupid,
                            groupName = objSysFunctionGroup.GroupName,
                            groupIcon = objSysFunctionGroup.GroupIcon,
                            groupMemo = objSysFunctionGroup.GroupMemo
                        }
                    });
                }
                else
                {
                    return Json(new { });
                }
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
        public JsonResult Index_Message(FunctionGroupSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _functionGroupService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.Groupid,
                           s1 = dy.GroupName,
                           s2 = ((!string.IsNullOrEmpty(dy.GroupIcon)) ? string.Format("<i class=\"fa {0}\"></i>", dy.GroupIcon) : string.Empty),
                           s3 = dy.Rootid
                       }
            };
            return Json(_result);
        }
        #endregion

        #region 添加
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Add_Message(FunctionGroupAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _functionGroupService.Add(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 编辑
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Edit(int id)
        {
            //过滤参数
            int _groupID = VariableHelper.SaferequestInt(id);
            SysFunctionGroup objSysFunctionGroup = _appDB.SysFunctionGroup.Where(p => p.Groupid == _groupID).SingleOrDefault();
            if (objSysFunctionGroup != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error", new { Type = (int)ErrorType.NoMessage });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Edit_Message(FunctionGroupEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _functionGroupService.Edit(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Delete_Message(string ids)
        {
            //过滤参数
            int[] _ids = VariableHelper.SaferequestIntArray(ids);

            var _res = _functionGroupService.Delete(_ids);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion
    }
}