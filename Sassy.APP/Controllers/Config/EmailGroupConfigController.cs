using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class EmailGroupConfigController : BaseController
    {
        private IEmailGroupConfigService _emailGroupConfigService;
        private appEntities _appDB;
        public EmailGroupConfigController(IBaseService baseService, IEmailGroupConfigService emailGroupConfigService, appEntities appEntities) : base(baseService)
        {
            _emailGroupConfigService = emailGroupConfigService;
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
                int _emailGroupID = VariableHelper.SaferequestInt(id);

                SendMailGroup objSendMailGroup = _appDB.SendMailGroup.Where(p => p.ID == _emailGroupID).SingleOrDefault();
                if (objSendMailGroup != null)
                {
                    List<string> _attrs = objSendMailGroup.MailAddresses.Split(',').ToList();

                    //返回数据
                    return Json(new
                    {
                        model = new
                        {
                            id = objSendMailGroup.ID,
                            groupName = objSendMailGroup.GroupName,
                            mailAddresses = from item in _attrs
                                            select new
                                            {
                                                index = _attrs.IndexOf(item),
                                                value = item
                                            },
                            remark = objSendMailGroup.Remark
                        }
                    }); ;
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
        public JsonResult Index_Message(EmailGroupConfigSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _emailGroupConfigService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.ID,
                           s1 = dy.GroupName,
                           s2 = dy.MailAddresses,
                           s3 = dy.Remark
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
        public JsonResult Add_Message(EmailGroupConfigAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _emailGroupConfigService.Add(request);
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
            int _emailGroupID = VariableHelper.SaferequestInt(id);
            SendMailGroup objSendMailGroup = _appDB.SendMailGroup.Where(p => p.ID == _emailGroupID).SingleOrDefault();
            if (objSendMailGroup != null)
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
        public JsonResult Edit_Message(EmailGroupConfigEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _emailGroupConfigService.Edit(request);
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

            var _res = _emailGroupConfigService.Delete(_ids);
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