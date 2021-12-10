using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Basic;
using Samsonite.Library.Business.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class FunctionController : BaseController
    {
        private IFunctionGroupService _functionGroupService;
        private IFunctionService _functionService;
        private appEntities _appDB;
        public FunctionController(IBaseService baseService, IFunctionGroupService functionGroupService, IFunctionService functionService, appEntities appEntities) : base(baseService)
        {
            _functionGroupService = functionGroupService;
            _functionService = functionService;
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            var _groupList = _functionGroupService.GetFunctionGroupObject().Select(p => new { label = p.GroupName, value = p.Groupid }).ToList();

            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers(),
                    //栏目列表
                    groupList = _groupList
                });
            }
            else if (type == "add")
            {
                //返回数据
                return Json(new
                {
                    //栏目列表
                    groupList = _groupList
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                int _funcID = VariableHelper.SaferequestInt(id);
                SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.Funcid == _funcID).SingleOrDefault();
                if (objSysFunction != null)
                {
                    //权限
                    List<DefineUserPower> _funcPowerAttrs = JsonHelper.JsonDeserialize<List<DefineUserPower>>(objSysFunction.FuncPower);

                    int _index = 0;
                    //返回数据
                    return Json(new
                    {
                        //栏目列表
                        groupList = _groupList,
                        model = new
                        {
                            id = objSysFunction.Funcid,
                            funcName = objSysFunction.FuncName,
                            groupId = objSysFunction.Groupid,
                            funcType = objSysFunction.FuncType.ToString(),
                            funcSign = objSysFunction.FuncSign,
                            funcUrl = objSysFunction.FuncUrl,
                            funcPowers = from item in _funcPowerAttrs
                                         select new
                                         {
                                             index = _index++,
                                             name = item.Name,
                                             value = item.Value
                                         },
                            funcTarget = objSysFunction.FuncTarget,
                            isShow = objSysFunction.IsShow,
                            funcMemo = objSysFunction.FuncMemo
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
        public JsonResult Index_Message(FunctionSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _functionService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.Funcid,
                           s1 = dy.FuncName,
                           s2 = dy.GroupName,
                           s3 = (dy.FuncType == 1) ? "菜单" : "功能",
                           s4 = dy.FuncSign,
                           s5 = dy.FuncUrl,
                           s6 = (dy.IsShow) ? "显示" : "<span class=\"text-danger\">隐藏</span>"
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
        public JsonResult Add_Message(FunctionAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _functionService.Add(request);
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
            int _funcID = VariableHelper.SaferequestInt(id);
            SysFunction objSysFunction = _appDB.SysFunction.Where(p => p.Funcid == _funcID).SingleOrDefault();
            if (objSysFunction != null)
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
        public JsonResult Edit_Message(FunctionEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _functionService.Edit(request);
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

            var _res = _functionService.Delete(_ids);
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