using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Business;
using Samsonite.Library.Business.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class UploadSparePartController : BaseController
    {
        private IUploadSparePartService _uploadSparePartService;
        private IWebHostEnvironment _webHostEnvironment;
        private appEntities _appDB;
        public UploadSparePartController(IBaseService baseService, IUploadSparePartService uploadSparePartService, IWebHostEnvironment webHostEnvironment, appEntities appEntities) : base(baseService)
        {
            _uploadSparePartService = uploadSparePartService;
            _webHostEnvironment = webHostEnvironment;
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
                    userAuthorization = this.FunctionPowers()
                });
            }
            else
            {
                return Json(new { });
            }
        }
        #endregion

        #region 导入Excel
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        public JsonResult Index_Message(UploadSparePartImportRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //读取信息
            var _list = _uploadSparePartService.Import(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           s1 = dy.SparePartId,
                           s2 = dy.SparePartDesc,
                           s3 = string.Join(",", dy.Skus.Select(p => p.SKU))
                       }
            };
            return Json(_result);
        }

        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public ActionResult Index_SaveUpload(UploadSparePartImportRequest request)
        {
            var _languagePack = this.GetLanguagePack;
            //过滤参数
            ValidateHelper.Validate(request);

            //读取信息
            var _errorList = _uploadSparePartService.ImportSave(request);

            var _result = new
            {
                result = true,
                msg = (_errorList.Count == 0) ? _languagePack["common_data_save_success"] : _languagePack["common_partial_data_save_success"],
                total = _errorList.Count,
                rows = from dy in _errorList
                       select new
                       {
                           s1 = dy.SparePartId,
                           s2 = dy.SparePartDesc,
                           s3 = string.Join(",", dy.Skus.Select(p => p.SKU)),
                           s4 = dy.Result ? "<label class=\"el-icon-check text-primary\"></label>" : "<label class=\"el-icon-close text-danger\"></label>",
                           s5 = dy.ResultMsg
                       }
            };
            return Json(_result);
        }
        #endregion
    }
}