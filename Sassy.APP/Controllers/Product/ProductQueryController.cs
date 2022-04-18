using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Custom;
using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Sassy.APP.Controllers
{
    public class ProductQueryController : BaseController
    {
        private ISparePartQueryService _sparePartQueryService;
        private IProductQueryService _productQueryService;
        private appEntities _appDB;
        public ProductQueryController(IBaseService baseService, ISparePartQueryService sparePartQueryService, IProductQueryService productQueryService, appEntities appEntities) : base(baseService)
        {
            _sparePartQueryService = sparePartQueryService;
            _productQueryService = productQueryService;
            _appDB = appEntities;
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
                    userAuthorization = this.FunctionPowers(),
                    //查询类型
                    searchTypeList = new List<DefineSelectOption>()
                    {
                        new DefineSelectOption{
                            Label = "Mat+Grid",
                            Value = 0
                        },
                        new DefineSelectOption
                        {
                            Label = "SKU",
                            Value = 1
                        }
                    }
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
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Line_Message(ProductQuerySearchLineRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _productQueryService.GetLineQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           s1 = dy.LineID,
                           s2 = dy.LineDescription
                       }
            };
            return Json(_result);
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Size_Message(ProductQuerySearchSizeRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _productQueryService.GetSizeQuery(request);
            var _result = new
            {
                rows = from dy in _list
                       select new
                       {
                           s1 = dy.SizeID,
                           s2 = dy.SizeDescription,
                       }
            };
            return Json(_result);
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Group_Message(ProductQuerySearchGroupRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _productQueryService.GetGroupQuery(request);
            var _result = new
            {
                rows = from dy in _list
                       select new
                       {
                           s1 = dy.GroupID,
                           s2 = dy.GroupDescription,
                       }
            };
            return Json(_result);
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public JsonResult Index_Message(ProductQuerySearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _productQueryService.GetQuery(request);
            var _productIDs = _list.Items.Select(p => p.MaterialId + "-" + p.Gridval).ToList();
            //产品集合
            var _products = _appDB.Product.Where(p => _productIDs.Contains(p.MaterialId + "-" + p.Gridval)).AsNoTracking().ToList();
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.RelatedID,
                           s1 = $"<a href=\"javascript:appVue.sparepartDetail('{dy.SparePartID}')\" class=\"href-blue-line\">" + dy.SparePartID + "</a>",
                           s2 = dy.SparePartDescription,
                           s3 = _sparePartQueryService.GetImageHtml(dy.ImageUrl, 75),
                           s4 = dy.MaterialId,
                           s5 = dy.Gridval,
                           s6 = _products.Where(p => p.MaterialId == dy.MaterialId && p.Gridval == dy.Gridval).SingleOrDefault()?.SKU,
                           s7 = _products.Where(p => p.MaterialId == dy.MaterialId && p.Gridval == dy.Gridval).SingleOrDefault()?.EAN,
                           s8 = dy.VersionID,
                           s9 = $"{VariableHelper.FormateMoney(dy.BasicPrice)} {dy.Currency}",
                           s10 = dy.UnitofMeasure,
                           s11 = dy.Status,
                           s12 = dy.AvailableStock
                       }
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

            var _res = _productQueryService.Delete(_ids);
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