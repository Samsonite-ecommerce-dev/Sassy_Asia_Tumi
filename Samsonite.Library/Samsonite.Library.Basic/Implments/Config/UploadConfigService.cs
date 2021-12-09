using Samsonite.Library.Basic.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Basic
{
    public class UploadConfigService : IUploadConfigService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public UploadConfigService(IAppLogService appLogService, appEntities appEntities)
        {
            _appDB = appEntities;
            _appLogService = appLogService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<SysUploadModel> GetQuery(UploadConfigSearchRequest request)
        {
            QueryResponse<SysUploadModel> _result = new QueryResponse<SysUploadModel>();
            var _list = _appDB.SysUploadModel.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.UploadName.Contains(request.Keyword) || p.ModelMark.Contains(request.Keyword));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.ID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(UploadConfigAddRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UploadName))
                {
                    throw new Exception("名称不能为空");
                }

                if (string.IsNullOrEmpty(request.ModelMark))
                {
                    throw new Exception("标识不能为空");
                }
                else
                {
                    SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ModelMark == request.ModelMark).SingleOrDefault();
                    if (objSysUploadModel != null)
                    {
                        throw new Exception("标识已经存在，请勿重复");
                    }
                }

                if (request.MaxFileCount <= 0)
                {
                    throw new Exception("文件上传数限制必须大于零");
                }

                if (request.MaxFileSize <= 0)
                {
                    throw new Exception("文件大小限制必须大于零");
                }

                if (string.IsNullOrEmpty(request.AllowFile))
                {
                    throw new Exception("文件后缀名限制不能为空");
                }

                if (string.IsNullOrEmpty(request.SaveCatalog))
                {
                    throw new Exception("保存文件夹名称不能为空");
                }

                //文件单位转化成k
                request.MaxFileSize = request.MaxFileSize * 1024;
                SysUploadModel objData = new SysUploadModel()
                {
                    UploadName = request.UploadName,
                    ModelMark = request.ModelMark,
                    FileType = request.FileType,
                    MaxFileSize = request.MaxFileSize,
                    MaxFileCount = request.MaxFileCount,
                    AllowFile = request.AllowFile.Replace(",", "|"),
                    SaveCatalog = request.SaveCatalog,
                    SaveStyle = request.SaveStyle,
                    IsRename = request.IsRename,
                    CreateTime = DateTime.Now
                };
                _appDB.SysUploadModel.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.InsertLog<SysUploadModel>(objData, objData.ID.ToString());
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = "数据保存成功"
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
        public PostResponse Edit(UploadConfigEditRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UploadName))
                {
                    throw new Exception("名称不能为空");
                }

                if (string.IsNullOrEmpty(request.ModelMark))
                {
                    throw new Exception("标识不能为空");
                }
                else
                {
                    SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ModelMark == request.ModelMark && p.ID != request.ID).SingleOrDefault();
                    if (objSysUploadModel != null)
                    {
                        throw new Exception("标识已经存在，请勿重复");
                    }
                }

                if (request.MaxFileCount <= 0)
                {
                    throw new Exception("文件上传数限制必须大于零");
                }

                if (request.MaxFileSize <= 0)
                {
                    throw new Exception("文件大小限制必须大于零");
                }

                if (string.IsNullOrEmpty(request.AllowFile))
                {
                    throw new Exception("文件后缀名限制不能为空");
                }

                if (string.IsNullOrEmpty(request.SaveCatalog))
                {
                    throw new Exception("保存文件夹名称不能为空");
                }

                //文件单位转化成k
                request.MaxFileSize = request.MaxFileSize * 1024;
                SysUploadModel objData = _appDB.SysUploadModel.Where(p => p.ID == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.UploadName = request.UploadName;
                    objData.ModelMark = request.ModelMark;
                    objData.FileType = request.FileType;
                    objData.MaxFileSize = request.MaxFileSize;
                    objData.MaxFileCount = request.MaxFileCount;
                    objData.AllowFile = request.AllowFile.Replace(",", "|");
                    objData.SaveCatalog = request.SaveCatalog;
                    objData.SaveStyle = request.SaveStyle;
                    objData.IsRename = request.IsRename;
                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<SysUploadModel>(objData, objData.ID.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = "数据保存成功"
                    };
                }
                else
                {
                    throw new Exception("数据读取失败");
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
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Delete(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception("请至少选择一条要操作的数据");
                }

                SysUploadModel objSysUploadModel = new SysUploadModel();
                foreach (var id in ids)
                {
                    objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ID == id).SingleOrDefault();
                    if (objSysUploadModel != null)
                    {
                        _appDB.SysUploadModel.Remove(objSysUploadModel);
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, "信息不存在或已被删除"));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("SysUploadModel", string.Join(",", ids));
                //返回信息
                return new PostResponse
                {
                    Result = true,
                    Message = "数据删除成功"
                };
            }
            catch (Exception ex)
            {
                return new PostResponse
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 上传类型列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> UploadTypeObject()
        {
            Dictionary<int, string> _result = new Dictionary<int, string>();
            _result.Add((int)UploadConfigType.Picture, "图片");
            _result.Add((int)UploadConfigType.File, "文件");
            return _result;
        }
    }
}
