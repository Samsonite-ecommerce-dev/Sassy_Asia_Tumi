using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business.Custom;
using Samsonite.Library.Business.Custom.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class SparePartQueryController : BaseController
    {
        private ISparePartQueryService _sparePartQueryService;
        private appEntities _appDB;
        public SparePartQueryController(IBaseService baseService, ISparePartQueryService sparePartQueryService, appEntities appEntities) : base(baseService)
        {
            _sparePartQueryService = sparePartQueryService;
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            var _statusList = _sparePartQueryService.GetStatusOption().Select(p => new { label = p, value = p }).ToList();

            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers(),
                    //状态列表
                    statusList = _statusList
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                long _sparePartID = VariableHelper.SaferequestInt64(id);
                SparePart objSparePart = _appDB.SparePart.Where(p => p.SparePartID == _sparePartID).SingleOrDefault();
                if (objSparePart != null)
                {
                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objSparePart.SparePartID,
                            sparepartDesc = objSparePart.SparePartDescription,
                            imageUrl = objSparePart.ImageUrl
                        }
                    });
                }
                else
                {
                    return Json(new { });
                }
            }
            else if (type == "detail")
            {
                //过滤参数
                long _sparePartID = VariableHelper.SaferequestInt64(id);
                SparePart objSparePart = _appDB.SparePart.Where(p => p.SparePartID == _sparePartID).SingleOrDefault();
                if (objSparePart != null)
                {
                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objSparePart.SparePartID,
                            sparepartDesc = objSparePart.SparePartDescription,
                            imageUrl = _sparePartQueryService.GetImageHtml(objSparePart.ImageUrl, 225),
                            price = $"{VariableHelper.FormateMoney(objSparePart.BasicPrice)} {objSparePart.Currency}",
                            unit = objSparePart.UnitofMeasure,
                            inventory = objSparePart.AvailableStock,
                            inventoryUpdateDate = objSparePart.InventoryUpdateDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                            status = objSparePart.Status
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
        public JsonResult Index_Message(SparePartQuerySearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _sparePartQueryService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.SparePartID,
                           s1 = $"<a href=\"javascript:appVue.sparepartDetail('{dy.SparePartID}')\" class=\"href-blue-line\">" + dy.SparePartID + "</a>",
                           s2 = dy.SparePartDescription,
                           s3 = _sparePartQueryService.GetImageHtml(dy.ImageUrl, 75),
                           s4 = dy.GroupName,
                           s5 = $"{VariableHelper.FormateMoney(dy.BasicPrice)} {dy.Currency}",
                           s6 = dy.UnitofMeasure,
                           s7 = dy.AvailableStock,
                           s8 = dy.InventoryUpdateDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                           s9 = dy.Status
                       }
            };
            return Json(_result);
        }
        #endregion

        #region 编辑
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Edit(int id)
        {
            //过滤参数
            long _sparePartID = VariableHelper.SaferequestInt64(id);
            SparePart objSparePart = _appDB.SparePart.Where(p => p.SparePartID == _sparePartID).SingleOrDefault();
            if (objSparePart != null)
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
        public JsonResult Edit_Message(SparePartQueryEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _sparePartQueryService.Edit(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 详情
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Detail(int id)
        {
            //过滤参数
            long _sparePartID = VariableHelper.SaferequestInt64(id);
            SparePart objSparePart = _appDB.SparePart.Where(p => p.SparePartID == _sparePartID).SingleOrDefault();
            if (objSparePart != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error", new { Type = (int)ErrorType.NoMessage });
            }
        }
        #endregion
    }
}