using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Business.Web.Custom
{
    public class SparePartGroupService : ISparePartGroupService
    {
        private IBaseService _baseService;
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public SparePartGroupService(IBaseService baseService, IAppLogService appLogService, appEntities appEntities)
        {
            _baseService = baseService;
            _appLogService = appLogService;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<GroupInfo> GetQuery(SparePartGroupSearchRequest request)
        {
            QueryResponse<GroupInfo> _result = new QueryResponse<GroupInfo>();
            var _list = _appDB.GroupInfo.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.GroupDescription.Contains(request.Keyword));
            }

            if (request.GroupType > 0)
            {
                _list = _list.Where(p => p.GroupType == request.GroupType);
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.GroupID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(SparePartGroupAddRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                var groupInfo = _appDB.GroupInfo.Where(p => p.GroupDescription == request.GroupDescription).FirstOrDefault();
                if (groupInfo != null)
                {
                    throw new Exception(_languagePack["sparepartgroup_edit_error_exsit_description"]);
                }

                //自定义的Group的ID为了防止和SAP的冲突使用10000后的数字
                int _groupID = (_appDB.GroupInfo.Any()) ? _appDB.GroupInfo.Max(p => p.GroupID) + 1 : 1;
                if (_groupID < 10000)
                    _groupID = 10001;

                GroupInfo objData = new GroupInfo()
                {
                    GroupID = _groupID,
                    GroupDescription = request.GroupDescription,
                    GroupType = (int)GroupType.Custom,
                    GroupText = request.GroupText,
                    AddDate = DateTime.Now,
                    EditDate = DateTime.Now
                };
                _appDB.GroupInfo.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.UpdateLog<GroupInfo>(objData, objData.GroupID.ToString());
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = _languagePack["common_data_save_success"]
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
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Edit(SparePartGroupEditRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                var groupInfo = _appDB.GroupInfo.Where(p => p.GroupDescription == request.GroupDescription && p.GroupID != request.ID).FirstOrDefault();
                if (groupInfo != null)
                {
                    throw new Exception(_languagePack["sparepartgroup_edit_error_exsit_description"]);
                }

                GroupInfo objData = _appDB.GroupInfo.Where(p => p.GroupID == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.GroupDescription = request.GroupDescription;
                    objData.GroupText = request.GroupText;

                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<GroupInfo>(objData, objData.GroupID.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _languagePack["common_data_save_success"]
                    };
                }
                else
                {
                    throw new Exception(_languagePack["common_data_load_false"]);
                }
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
        /// 返回分组集合
        /// </summary>
        /// <returns></returns>
        public List<GroupInfo> GetFunctionGroupObject()
        {
            return _appDB.GroupInfo.OrderBy(p => p.GroupID).ToList();
        }
    }
}
