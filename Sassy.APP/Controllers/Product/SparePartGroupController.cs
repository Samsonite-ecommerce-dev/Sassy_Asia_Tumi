using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Web.Custom;
using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Sassy.APP.Helper;
using System.Linq;

namespace Sassy.APP.Controllers
{
    public class SparePartGroupController : BaseController
    {
        private ISparePartGroupService _sparePartGroupService;
        private SparePartHelper _sparePartHelper;
        private appEntities _appDB;
        public SparePartGroupController(IBaseService baseService, ISparePartGroupService sparePartGroupService, appEntities appEntities) : base(baseService)
        {
            _sparePartGroupService = sparePartGroupService;
            _sparePartHelper = new SparePartHelper();
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            var _groupTypeList = _sparePartHelper.SparePartGroupTypeObject().Select(p => new { label = p.Label, value = p.Value }).ToList();

            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers(),
                    //分组类型
                    groupTypeList = _groupTypeList
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                long _groupID = VariableHelper.SaferequestInt64(id);
                GroupInfo objGroupInfo = _appDB.GroupInfo.Where(p => p.GroupID == _groupID).SingleOrDefault();
                if (objGroupInfo != null)
                {
                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objGroupInfo.GroupID,
                            groupDescription = objGroupInfo.GroupDescription,
                            groupText = objGroupInfo.GroupText
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
        public JsonResult Index_Message(SparePartGroupSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _sparePartGroupService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.GroupID,
                           s1 = dy.GroupID,
                           s2 = dy.GroupDescription,
                           s3 = _sparePartHelper.GetSparePartGroupTypeDisplay(dy.GroupType, true),
                           s4 = dy.GroupText
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
        public JsonResult Add_Message(SparePartGroupAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _sparePartGroupService.Add(request);
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
            long _groupID = VariableHelper.SaferequestInt64(id);
            GroupInfo objGroupInfo = _appDB.GroupInfo.Where(p => p.GroupID == _groupID).SingleOrDefault();
            if (objGroupInfo != null)
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
        public JsonResult Edit_Message(SparePartGroupEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _sparePartGroupService.Edit(request);
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