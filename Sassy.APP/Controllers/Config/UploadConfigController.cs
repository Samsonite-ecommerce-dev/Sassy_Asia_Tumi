using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Basic;
using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class UploadConfigController : BaseController
    {
        private IUploadConfigService _uploadConfigService;
        private appEntities _appDB;
        public UploadConfigController(IBaseService baseService, IUploadConfigService uploadConfigService, appEntities appEntities) : base(baseService)
        {
            _uploadConfigService = uploadConfigService;
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            var _uploadTypeList = _uploadConfigService.UploadTypeObject().Select(p => new
            {
                label = p.Value,
                value = p.Key
            });

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
            else if (type == "add")
            {
                //返回数据
                return Json(new
                {
                    uploadTypeList = _uploadTypeList
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                int _uploadID = VariableHelper.SaferequestInt(id);

                SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ID == _uploadID).SingleOrDefault();
                if (objSysUploadModel != null)
                {
                    //返回数据
                    return Json(new
                    {
                        uploadTypeList = _uploadTypeList,
                        model = new
                        {
                            id = objSysUploadModel.ID,
                            uploadName = objSysUploadModel.UploadName,
                            modelMark = objSysUploadModel.ModelMark,
                            fileType = objSysUploadModel.FileType,
                            maxFileCount = objSysUploadModel.MaxFileCount,
                            maxFileSize = (Math.Floor((double)objSysUploadModel.MaxFileSize / 1024)).ToString(),
                            allowFile = objSysUploadModel.AllowFile,
                            saveStyle = objSysUploadModel.SaveStyle,
                            saveCatalog = objSysUploadModel.SaveCatalog,
                            isRename = objSysUploadModel.IsRename
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
        public JsonResult Index_Message(UploadConfigSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _uploadConfigService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.ID,
                           s1 = dy.UploadName,
                           s2 = dy.ModelMark,
                           s3 = (dy.FileType == (int)UploadConfigType.File) ? "文件" : "图片",
                           s4 = dy.MaxFileCount,
                           s5 = VariableHelper.FormatSize(dy.MaxFileSize),
                           s6 = dy.AllowFile.Replace("|", ","),
                           s7 = (dy.SaveStyle.ToLower() == "dateorder") ? "按时间" : "按文件夹",
                           s8 = dy.SaveCatalog,
                           s9 = (dy.IsRename) ? "<label class=\"el-icon-check text-primary\"></label>" : "<label class=\"el-icon-close text-danger\"></label>"
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
        public JsonResult Add_Message(UploadConfigAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _uploadConfigService.Add(request);
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
            int _uploadID = VariableHelper.SaferequestInt(id);

            SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ID == _uploadID).SingleOrDefault();
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
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Edit_Message(UploadConfigEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _uploadConfigService.Edit(request);
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

            var _res = _uploadConfigService.Delete(_ids);
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