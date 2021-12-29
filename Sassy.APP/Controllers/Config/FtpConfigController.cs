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
    public class FtpConfigController : BaseController
    {
        private IFtpConfigService _ftpConfigService;
        private appEntities _appDB;
        public FtpConfigController(IBaseService baseService, IFtpConfigService ftpConfigService, appEntities appEntities) : base(baseService)
        {
            _ftpConfigService = ftpConfigService;
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
                int _ftpID = VariableHelper.SaferequestInt(id);

                FTPInfo objFTPInfo = _appDB.FTPInfo.Where(p => p.ID == _ftpID).SingleOrDefault();
                if (objFTPInfo != null)
                {
                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objFTPInfo.ID,
                            ftpName = objFTPInfo.FTPName,
                            ftpIdentify = objFTPInfo.FTPIdentify,
                            ip = objFTPInfo.IP,
                            port = objFTPInfo.Port,
                            userName = objFTPInfo.UserName,
                            password = objFTPInfo.Password,
                            filePath = objFTPInfo.FilePath,
                            sort = objFTPInfo.SortID,
                            remark = objFTPInfo.Remark
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
        public JsonResult Index_Message(FtpConfigSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _ftpConfigService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.ID,
                           s1 = dy.FTPName,
                           s2 = dy.FTPIdentify,
                           s3 = dy.IP,
                           s4 = dy.Port,
                           s5 = dy.UserName,
                           s6 = dy.Password,
                           s7 = dy.FilePath,
                           s8 = dy.SortID,
                           s9 = dy.Remark,
                           s10 = dy.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
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
        public JsonResult Add_Message(FtpConfigAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _ftpConfigService.Add(request);
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
            int _ftpID = VariableHelper.SaferequestInt(id);

            FTPInfo objFTPInfo = _appDB.FTPInfo.Where(p => p.ID == _ftpID).SingleOrDefault();
            if (objFTPInfo != null)
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
        public JsonResult Edit_Message(FtpConfigEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _ftpConfigService.Edit(request);
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