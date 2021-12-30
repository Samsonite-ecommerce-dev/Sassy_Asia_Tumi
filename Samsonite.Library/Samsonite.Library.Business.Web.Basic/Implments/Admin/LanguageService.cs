using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Samsonite.Library.Business.Web.Basic
{
    public class LanguageService : ILanguageService
    {
        private IBaseService _baseService;
        private IFunctionGroupService _functionGroupService;
        private IFunctionService _functionService;
        private IAppLogService _appLogService;
        private IHttpContextAccessor _httpContextAccessor;
        private appEntities _appDB;
        public LanguageService(IBaseService baseService, IFunctionGroupService functionGroupService, IFunctionService functionService, IAppLogService appLogService, IHttpContextAccessor httpContextAccessor, appEntities appEntities)
        {
            _baseService = baseService;
            _functionGroupService = functionGroupService;
            _functionService = functionService;
            _appLogService = appLogService;
            _httpContextAccessor = httpContextAccessor;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<View_LanguagePack> GetQuery(LanguageSearchRequest request)
        {
            QueryResponse<View_LanguagePack> _result = new QueryResponse<View_LanguagePack>();
            var _list = _appDB.View_LanguagePack.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.PackKey.Contains(request.Keyword) || p.PackValue.Contains(request.Keyword));
            }

            if (request.TypeID != 0)
            {
                _list = _list.Where(p => p.LanguageTypeID == request.TypeID);
            }

            if (request.FunctionID != 0)
            {
                _list = _list.Where(p => p.FunctionID == request.FunctionID);
            }

            if (request.IsDelete == 1)
            {
                _list = _list.Where(p => p.IsDelete);
            }
            else
            {
                _list = _list.Where(p => !p.IsDelete);
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.FunctionID).ThenBy(p => p.SeqNumber).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(LanguageAddRequest request)
        {
            //加载语言包
            var _LanguagePackKey = _baseService.CurrentLanguagePack;

            int _seqNumberID = 0;
            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    if (request.FunctionID == 0)
                    {
                        throw new Exception(_LanguagePackKey["language_edit_message_no_category"]);
                    }

                    //要插队的排序号
                    var _packKeyArray = JsonSerializer.Deserialize<List<LanguagePackAttr>>(request.packKeys);
                    if (_packKeyArray.Count > 0)
                    {
                        List<LanguagePackKey> _removeLanguagePackKeys = new List<LanguagePackKey>();
                        foreach (var item in _packKeyArray)
                        {
                            if (string.IsNullOrEmpty(item.PackKey))
                            {
                                throw new Exception(_LanguagePackKey["language_edit_message_no_key"]);
                            }
                            else
                            {
                                LanguagePackKey objLanguagePackKey = _appDB.LanguagePackKey.Where(p => p.PackKey == item.PackKey).SingleOrDefault();
                                if (objLanguagePackKey != null)
                                {
                                    throw new Exception(_LanguagePackKey["language_edit_message_repeat_category"]);
                                }
                            }

                            if (request.KeyID == 0)
                            {
                                if (_seqNumberID == 0)
                                {
                                    _seqNumberID = (_appDB.LanguagePackKey.Where(p => p.FunctionID == request.FunctionID).Any()) ? _appDB.LanguagePackKey.Where(p => p.FunctionID == request.FunctionID).Max(p => p.SeqNumber) + 1 : 1;
                                }
                                else
                                {
                                    _seqNumberID++;
                                }
                            }
                            else
                            {
                                if (_seqNumberID == 0)
                                {
                                    LanguagePackKey objLanguagePackKey1 = _appDB.LanguagePackKey.Where(p => p.ID == request.KeyID && p.FunctionID == request.FunctionID).SingleOrDefault();
                                    if (objLanguagePackKey1 != null)
                                    {
                                        _seqNumberID = objLanguagePackKey1.SeqNumber + 1;
                                        //需要改变排序号的数据集合
                                        _removeLanguagePackKeys = _appDB.LanguagePackKey.Where(p => p.FunctionID == request.FunctionID && p.SeqNumber > objLanguagePackKey1.SeqNumber).ToList();
                                    }
                                }
                                else
                                {
                                    _seqNumberID++;
                                }
                            }

                            //添加Key
                            LanguagePackKey objData = new LanguagePackKey()
                            {
                                FunctionID = request.FunctionID,
                                PackKey = item.PackKey,
                                SeqNumber = _seqNumberID,
                                IsDelete = false
                            };
                            _appDB.LanguagePackKey.Add(objData);
                            _appDB.SaveChanges();

                            List<LanguagePackValue> objDatas = new List<LanguagePackValue>();
                            //添加关联语言包
                            foreach (var lg in item.Languages)
                            {
                                objDatas.Add(new LanguagePackValue()
                                {
                                    KeyID = objData.ID,
                                    LanguageTypeID = lg.LanguageTypeID,
                                    PackValue = lg.LanguageValue
                                });
                            }
                            _appDB.LanguagePackValue.AddRange(objDatas);
                        }
                        //如果是插队添加
                        if (request.KeyID > 0)
                        {
                            if (_removeLanguagePackKeys.Count > 0)
                            {
                                foreach (var item in _removeLanguagePackKeys)
                                {
                                    item.SeqNumber = item.SeqNumber + _packKeyArray.Count;
                                }
                            }
                        }
                        _appDB.SaveChanges();
                    }
                    else
                    {
                        throw new Exception(_LanguagePackKey["language_edit_message_atleast_one"]);
                    }
                    Trans.Commit();
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _LanguagePackKey["common_data_save_success"]
                    };
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    return new PostResponse()
                    {
                        Result = false,
                        Message = ex.Message
                    };
                }
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Edit(LanguageEditRequest request)
        {
            //加载语言包
            var _LanguagePackKey = _baseService.CurrentLanguagePack;

            using (var Trans = _appDB.Database.BeginTransaction())
            {
                try
                {
                    var _languagePackValueAttrs = new List<LanguagePackValueAttr>();
                    LanguagePackKey objData = _appDB.LanguagePackKey.Where(p => p.ID == request.ID).SingleOrDefault();
                    if (objData != null)
                    {
                        objData.PackKey = request.LanguageKey;
                        if (!string.IsNullOrEmpty(request.LanguageValue))
                        {
                            _languagePackValueAttrs = JsonSerializer.Deserialize<List<LanguagePackValueAttr>>(request.LanguageValue);
                            //删除旧信息
                            _appDB.Database.ExecuteSqlRaw("delete from LanguagePackValue where KeyID={0}", objData.ID);
                            foreach (var item in _languagePackValueAttrs)
                            {
                                _appDB.LanguagePackValue.Add(new LanguagePackValue()
                                {
                                    KeyID = objData.ID,
                                    LanguageTypeID = item.LanguageTypeID,
                                    PackValue = item.LanguageValue
                                });
                            }
                        }
                        _appDB.SaveChanges();
                        //编辑日志
                        _appLogService.UpdateLog<LanguagePackKey>(objData, objData.ID.ToString());
                    }
                    Trans.Commit();
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _LanguagePackKey["common_data_save_success"]
                    };
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    return new PostResponse()
                    {
                        Result = false,
                        Message = ex.Message
                    };
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Delete(long[] ids)
        {
            //加载语言包
            var _LanguagePackKey = _baseService.CurrentLanguagePack;

            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception(_LanguagePackKey["common_data_need_one"]);
                }

                LanguagePackKey objLanguagePackKey = new LanguagePackKey();
                foreach (var id in ids)
                {
                    objLanguagePackKey = _appDB.LanguagePackKey.Where(p => p.ID == id).SingleOrDefault();
                    if (objLanguagePackKey != null)
                    {
                        objLanguagePackKey.IsDelete = true;
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, _LanguagePackKey["common_data_no_exsit"]));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("LanguagePackKey", string.Join(",", ids));
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = _LanguagePackKey["common_data_delete_success"]
                };

            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Restore(long[] ids)
        {
            //加载语言包
            var _LanguagePackKey = _baseService.CurrentLanguagePack;

            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception(_LanguagePackKey["common_data_need_one"]);
                }

                LanguagePackKey objLanguagePackKey = new LanguagePackKey();
                foreach (var id in ids)
                {
                    objLanguagePackKey = _appDB.LanguagePackKey.Where(p => p.ID == id).SingleOrDefault();
                    if (objLanguagePackKey != null)
                    {
                        objLanguagePackKey.IsDelete = false;
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, _LanguagePackKey["common_data_no_exsit"]));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.RestoreLog("LanguagePackKey", string.Join(",", ids));
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = _LanguagePackKey["common_data_recover_success"]
                };

            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Sort(LanguageSortRequest request)
        {
            //加载语言包
            var _LanguagePackKey = _baseService.CurrentLanguagePack;

            try
            {
                int _NewSeqNumber = 0;
                LanguagePackKey objLanguagePackKey = _appDB.LanguagePackKey.Where(p => p.ID == request.ID).SingleOrDefault();
                if (objLanguagePackKey != null)
                {
                    if (request.Type.ToUpper() == "U")
                    {
                        LanguagePackKey objLanguagePackKey_U = _appDB.LanguagePackKey.Where(p => p.FunctionID == objLanguagePackKey.FunctionID && p.SeqNumber < objLanguagePackKey.SeqNumber).OrderByDescending(p => p.SeqNumber).FirstOrDefault();
                        if (objLanguagePackKey_U != null)
                        {
                            _NewSeqNumber = objLanguagePackKey_U.SeqNumber;
                            //交换排序号
                            objLanguagePackKey_U.SeqNumber = objLanguagePackKey.SeqNumber;
                            objLanguagePackKey.SeqNumber = _NewSeqNumber;
                        }
                        else
                        {
                            throw new Exception(_LanguagePackKey["language_sort_message_on_top"]);
                        }
                    }
                    else if (request.Type.ToUpper() == "D")
                    {
                        LanguagePackKey objLanguagePackKey_D = _appDB.LanguagePackKey.Where(p => p.FunctionID == objLanguagePackKey.FunctionID && p.SeqNumber > objLanguagePackKey.SeqNumber).OrderBy(p => p.SeqNumber).FirstOrDefault();
                        if (objLanguagePackKey_D != null)
                        {
                            _NewSeqNumber = objLanguagePackKey_D.SeqNumber;
                            //交换排序号
                            objLanguagePackKey_D.SeqNumber = objLanguagePackKey.SeqNumber;
                            objLanguagePackKey.SeqNumber = _NewSeqNumber;
                        }
                        else
                        {
                            throw new Exception(_LanguagePackKey["language_sort_message_on_bottom"]);
                        }
                    }
                    else
                    {
                        throw new Exception(_LanguagePackKey["common_parameter_error"]);
                    }
                    _appDB.SaveChanges();
                }
                else
                {
                    throw new Exception("common_data_load_false");
                }
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = _LanguagePackKey["common_data_save_success"]
                };

            }
            catch (Exception ex)
            {
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 根据分类查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<LanguagePackKey> GetQueryByKey(LanguageSearchByKeyRequest request)
        {
            QueryResponse<LanguagePackKey> _result = new QueryResponse<LanguagePackKey>();
            int _top = 20;
            var _list = _appDB.LanguagePackKey.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Key))
            {
                _list = _list.Where(p => p.PackKey.Contains(request.Key));
            }

            //搜索条件
            if (request.FunctionID != 0)
            {
                _list = _list.Where(p => p.FunctionID == request.FunctionID);
            }

            //返回数据
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.FunctionID).ThenBy(p => p.SeqNumber).Take(_top).ToList();
            return _result;
        }

        /// <summary>
        /// 功能组合菜单
        /// </summary>
        /// <returns></returns>
        public List<DefineGroupSelectOption> GetLanguageGroupObject()
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            List<DefineGroupSelectOption> _result = new List<DefineGroupSelectOption>();
            List<SysFunctionGroup> objSysFunctionGroups = _functionGroupService.GetFunctionGroupObject();
            List<SysFunction> objSysFunctions = _functionService.GetFunctionObject();
            var defineSelectOptions = new List<DefineSelectOption>();
            //公共
            _result.Add(new DefineGroupSelectOption()
            {
                Label = "Public",
                Options = new List<DefineSelectOption>()
                {
                    new DefineSelectOption()
                   {
                        Label = "Home",
                        Value = -999
                   },new DefineSelectOption()
                   {
                        Label = "Login",
                        Value = -998
                   },
                    new DefineSelectOption()
                   {
                        Label = "Upload",
                        Value = -997
                   },
                    new DefineSelectOption()
                   {
                        Label = "Menu",
                        Value = -1
                   },
                    new DefineSelectOption()
                   {
                        Label = "Common",
                        Value = 999
                   }
                }
            });
            foreach (var _sfg in objSysFunctionGroups)
            {
                defineSelectOptions = new List<DefineSelectOption>();
                foreach (var _sf in objSysFunctions.Where(p => p.Groupid == _sfg.Groupid))
                {
                    defineSelectOptions.Add(new DefineSelectOption()
                    {
                        Label = _languagePack[string.Format("menu_function_{0}", _sf.Funcid)],
                        Value = _sf.Funcid
                    });
                }
                _result.Add(new DefineGroupSelectOption()
                {
                    Label = _languagePack[string.Format("menu_group_{0}", _sfg.Groupid)],
                    Options = defineSelectOptions
                });
            }
            return _result;
        }
    }
}
