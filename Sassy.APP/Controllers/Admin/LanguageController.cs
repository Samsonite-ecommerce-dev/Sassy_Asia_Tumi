using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Controllers
{
    public class LanguageController : BaseController
    {
        private IBaseService _baseService;
        private ILanguageService _languageService;
        private IAppLanguageService _appLanguageService;
        private appEntities _appDB;
        public LanguageController(IBaseService baseService, ILanguageService languageService, IAppLanguageService appLanguageService, appEntities appEntities) : base(baseService)
        {
            _baseService = baseService;
            _languageService = languageService;
            _appLanguageService = appLanguageService;
            _appDB = appEntities;
        }

        #region 初始化
        [HttpGet]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(Action = "index")]
        public JsonResult Initialize_Info(string type, int id)
        {
            var _languagePack = this.GetLanguagePack;
            var _languageFunctionList = _languageService.GetLanguageGroupObject();
            var _languageList = _appLanguageService.LanguageTypeOption().Where(p => _baseService.CurrentApplicationConfig().SysConfig.LanguagePacks.Contains(p.ID)).Select(o => new DefineSelectOption() { Label = o.LanguageName, Value = o.ID });

            if (type == "index")
            {
                //返回数据
                return Json(new
                {
                    //菜单栏
                    navMenu = this.MenuBar(),
                    //功能权限
                    userAuthorization = this.FunctionPowers(),
                    //功能列表
                    languageFunctionList = _languageFunctionList,
                    //语言包配置列表
                    languageList = _languageList,
                    statusList = new List<DefineSelectOption>()
                    {
                        new DefineSelectOption{
                            Label = _languagePack["common_actived"],
                            Value = 0
                        },
                        new DefineSelectOption
                        {
                            Label = _languagePack["common_deleted"],
                            Value = 1
                        }
                    }
                });
            }
            else if (type == "add")
            {
                return Json(new
                {
                    //功能列表
                    languageFunctionList = _languageFunctionList,
                    //语言包配置列表
                    languageList = _languageList
                });
            }
            else if (type == "edit")
            {
                //过滤参数
                int _keyID = VariableHelper.SaferequestInt(id);
                LanguagePackKey objLanguagePackKey = _appDB.LanguagePackKey.Where(p => p.ID == _keyID).SingleOrDefault();
                if (objLanguagePackKey != null)
                {
                    //语言包值
                    List<LanguagePackValue> languagePackValues = _appDB.LanguagePackValue.Where(p => p.KeyID == objLanguagePackKey.ID).ToList();
                    //语言包配置列表
                    var languages = _appLanguageService.LanguageTypeOption().Where(p => _baseService.CurrentApplicationConfig().SysConfig.LanguagePacks.Contains(p.ID)).ToList();

                    return Json(new
                    {
                        //功能列表
                        languageFunctionList = _languageFunctionList,
                        model = new
                        {
                            id = objLanguagePackKey.ID,
                            functionId = objLanguagePackKey.FunctionID,
                            languageKey = objLanguagePackKey.PackKey,
                            languageValue = from ls in languages
                                            join lpk in languagePackValues on ls.ID equals lpk.LanguageTypeID
                                            select new
                                            {
                                                id = lpk.LanguageTypeID,
                                                label = ls.LanguageName,
                                                value = lpk.PackValue
                                            }
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
        public JsonResult Index_Message(LanguageSearchRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _languageService.GetQuery(request);
            var _result = new
            {
                total = _list.TotalRecord,
                rows = from dy in _list.Items
                       select new
                       {
                           ck = dy.KeyID,
                           s1 = dy.PackKey,
                           s2 = dy.LanguageName,
                           s3 = dy.PackValue,
                           s4 = dy.SeqNumber,
                           s5 = string.Format("<a href=\"javascript: void(0)\" onclick=\"elementExtend.Grid.CommonOper(appVue,'" + Url.Action("Sort_Message", "Language") + "',{{id:{0},type:'U'}})\"><icon class=\"el-icon-top text-success fontSize-18\"></icon></a><a href=\"javascript: void(0)\" onclick=\"elementExtend.Grid.CommonOper(appVue,'" + Url.Action("Sort_Message", "Language") + "',{{id:{0},type:'D'}})\"><icon class=\"el-icon-bottom text-success fontSize-18\"></icon></a>", dy.KeyID),
                           s6 = (!dy.IsDelete) ? "<label class=\"el-icon-check text-primary\"></label>" : "<label class=\"el-icon-close text-danger\"></label>",
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
        public JsonResult Add_Message(LanguageAddRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _languageService.Add(request);
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
            int _keyID = VariableHelper.SaferequestInt(id);
            LanguagePackKey objLanguagePackKey = _appDB.LanguagePackKey.Where(p => p.ID == _keyID).SingleOrDefault();
            if (objLanguagePackKey != null)
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
        public JsonResult Edit_Message(LanguageEditRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _languageService.Edit(request);
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
            long[] _ids = VariableHelper.SaferequestInt64Array(ids);

            var _res = _languageService.Delete(_ids);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 恢复
        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Restore_Message(string ids)
        {
            //过滤参数
            long[] _ids = VariableHelper.SaferequestInt64Array(ids);

            var _res = _languageService.Restore(_ids);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 排序
        [HttpPost]
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Sort_Message(LanguageSortRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            var _res = _languageService.Sort(request);
            var _result = new
            {
                result = _res.Result,
                msg = _res.Message
            };
            return Json(_result);
        }
        #endregion

        #region 重置语言包缓存
        [ServiceFilter(typeof(UserPowerAuthorize))]
        [AuthorizePropertyAttribute(IsAntiforgeryToken = true)]
        public JsonResult Reset_Message()
        {
            //加载语言包
            var _languagePack = this.GetLanguagePack;

            try
            {
                _appLanguageService.ResetLanguagePacks();
                //返回信息
                return Json(new
                {
                    result = true,
                    msg = _languagePack["common_data_save_success"]
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false,
                    msg = ex.Message
                });
            }
        }
        #endregion

        #region 查询语言包关键字
        /// <summary>
        /// 查询语言包关键字
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ServiceFilter(typeof(UserLoginAuthorize))]
        public JsonResult LanguagePack_Message(LanguageSearchByKeyRequest request)
        {
            //过滤参数
            ValidateHelper.Validate(request);

            //查询
            var _list = _languageService.GetQueryByKey(request);
            var _result = new
            {
                rows = from dy in _list.Items
                       select new
                       {
                           label = dy.PackKey,
                           value = dy.ID
                       }
            };
            return Json(_result);
        }
        #endregion
    }
}
