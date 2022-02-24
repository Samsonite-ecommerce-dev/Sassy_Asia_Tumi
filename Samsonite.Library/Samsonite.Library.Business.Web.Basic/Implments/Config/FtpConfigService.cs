using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Linq;

namespace Samsonite.Library.Business.Web.Basic
{
    public class FtpConfigService : IFtpConfigService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public FtpConfigService(IAppLogService appLogService, appEntities appEntities)
        {
            _appDB = appEntities;
            _appLogService = appLogService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<FTPInfo> GetQuery(FtpConfigSearchRequest request)
        {
            QueryResponse<FTPInfo> _result = new QueryResponse<FTPInfo>();
            var _list = _appDB.FTPInfo.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.FTPName.Contains(request.Keyword));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.SortID).ThenBy(p => p.ID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(FtpConfigAddRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FTPIdentify))
                {
                    throw new Exception("标识不能为空");
                }

                if (string.IsNullOrEmpty(request.FTPName))
                {
                    throw new Exception("名称不能为空");
                }
                else
                {
                    FTPInfo objFTPInfo = _appDB.FTPInfo.Where(p => p.FTPName == request.FTPName).SingleOrDefault();
                    if (objFTPInfo != null)
                    {
                        throw new Exception("名称已经存在，请勿重复");
                    }
                }

                if (string.IsNullOrEmpty(request.IP))
                {
                    throw new Exception("FTP主机不能为空");
                }

                if (string.IsNullOrEmpty(request.UserName))
                {
                    throw new Exception("FTP用户不能为空");
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new Exception("FTP密码不能为空");
                }

                //默认22端口
                if (request.Port == 0) request.Port = 22;
                //默认根目录
                if (string.IsNullOrEmpty(request.FilePath)) request.FilePath = "/";

                int _sortID = (_appDB.FTPInfo.Any()) ? _appDB.FTPInfo.Max(p => p.SortID) + 1 : 1;

                FTPInfo objData = new FTPInfo()
                {
                    FTPName = request.FTPName,
                    IP = request.IP,
                    Port = request.Port,
                    UserName = request.UserName,
                    Password = request.Password,
                    FilePath = request.FilePath,
                    Remark = request.Remark,
                    SortID = _sortID,
                    CreateTime = DateTime.Now
                };
                _appDB.FTPInfo.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.InsertLog<FTPInfo>(objData, objData.ID.ToString());
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
        public PostResponse Edit(FtpConfigEditRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FTPIdentify))
                {
                    throw new Exception("标识不能为空");
                }

                if (string.IsNullOrEmpty(request.FTPName))
                {
                    throw new Exception("名称不能为空");
                }
                else
                {
                    FTPInfo objFTPInfo = _appDB.FTPInfo.Where(p => p.FTPName == request.FTPName && p.ID != request.ID).SingleOrDefault();
                    if (objFTPInfo != null)
                    {
                        throw new Exception("名称已经存在，请勿重复");
                    }
                }

                if (string.IsNullOrEmpty(request.IP))
                {
                    throw new Exception("FTP主机不能为空");
                }

                if (string.IsNullOrEmpty(request.UserName))
                {
                    throw new Exception("FTP用户不能为空");
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new Exception("FTP密码不能为空");
                }

                //默认22端口
                if (request.Port == 0) request.Port = 22;
                //默认根目录
                if (string.IsNullOrEmpty(request.FilePath)) request.FilePath = "/";

                FTPInfo objData = _appDB.FTPInfo.Where(p => p.ID == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.FTPIdentify = request.FTPIdentify;
                    objData.FTPName = request.FTPName;
                    objData.IP = request.IP;
                    objData.Port = request.Port;
                    objData.UserName = request.UserName;
                    objData.Password = request.Password;
                    objData.FilePath = request.FilePath;
                    objData.Remark = request.Remark;
                    objData.SortID = request.Sort;
                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<FTPInfo>(objData, objData.ID.ToString());
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
        /// 根据标识获取FTP信息
        /// </summary>
        /// <param name="ftpIdentify"></param>
        /// <returns></returns>
        public FTPInfo GetFtpInfo(string ftpIdentify)
        {
            return _appDB.FTPInfo.Where(p => p.FTPIdentify == ftpIdentify).SingleOrDefault();
        }
    }
}
