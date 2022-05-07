using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Sassy.APP.Controllers
{
    public class UploadController : BaseController
    {
        public IBaseService _baseService;
        private IUploadService _uploadService;
        private appEntities _appDB;
        public UploadController(IBaseService baseService, IUploadService uploadService, appEntities appEntities) : base(baseService)
        {
            _baseService = baseService;
            _uploadService = uploadService;
            _appDB = appEntities;
        }

        #region 查询
        [HttpGet]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult Initialize_Info(string model, string catalog)
        {
            string _model = VariableHelper.SaferequestStr(model);
            string _catalog = VariableHelper.SaferequestStr(catalog);
            SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ModelMark == _model).SingleOrDefault();
            if (objSysUploadModel != null)
            {
                if (!string.IsNullOrEmpty(_catalog))
                {
                    objSysUploadModel.SaveCatalog = _catalog;
                }
                var allowFileExts = new List<string>();
                var allowFileExtArray = objSysUploadModel.AllowFile.Split('|');
                foreach (var item in allowFileExtArray)
                {
                    allowFileExts.Add($".{item}");
                }


                //返回数据
                return Json(new
                {
                    model = objSysUploadModel.ModelMark,
                    catalog = _catalog,
                    listType = (objSysUploadModel.FileType == 2) ? "text" : "picture",
                    maxFileCount = objSysUploadModel.MaxFileCount,
                    maxFileSize = VariableHelper.FormatSize(objSysUploadModel.MaxFileSize),
                    allowFile = string.Join(",", allowFileExts)
                });
            }
            else
            {
                return Json(new { });
            }
        }

        [ServiceFilter(typeof(UserLoginAuthorize))]
        public ActionResult Index()
        {
            string _model = VariableHelper.SaferequestStr(Request.Query["model"].FirstOrDefault());
            string _catalog = VariableHelper.SaferequestStr(Request.Query["catalog"].FirstOrDefault());
            SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ModelMark == _model).SingleOrDefault();
            if (objSysUploadModel != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error", new { Type = (int)ErrorType.NoMessage });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult Index_Message(UploadSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate<UploadSearchRequest>(request);

            //查询
            var _list = _uploadService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           s1 = dy.FileName,
                           s2 = dy.FileExt,
                           s3 = dy.FileSize,
                           s4 = dy.EditTime.ToString("yyyy-MM-dd HH:mm")
                       }
            };
            return Json(_result);
        }
        #endregion

        #region 保存文件
        [HttpPost]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public ActionResult Save_Message(UploadSaveRequest request)
        {
            //过滤参数
            ValidateHelper.Validate<UploadSaveRequest>(request);

            var _res = _uploadService.SaveFileAsync(request);
            var _result = new
            {
                filename = _res.Result.FileName,
                filepath = _res.Result.FilePath,
                result = _res.Result.Result,
                msg = _res.Result.Message
            };
            return Json(_result);
        }
        #endregion
    }
}