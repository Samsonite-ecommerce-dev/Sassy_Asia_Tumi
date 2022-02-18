using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Samsonite.Library.Core.Web
{
    public class UploadService : IUploadService
    {
        private IBaseService _baseService;
        private IAppConfigService _appConfigService;
        private IHostEnvironment _hostEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        private appEntities _appDB;
        public UploadService(IBaseService baseService, IAppConfigService appConfigService, IHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, appEntities appEntities)
        {
            _baseService = baseService;
            _appConfigService = appConfigService;
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询文件列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<UploadFileCollection> GetQuery(UploadSearchRequest request)
        {
            QueryResponse<UploadFileCollection> _result = new QueryResponse<UploadFileCollection>();

            List<UploadFileCollection> objFiles = new List<UploadFileCollection>();
            string _directoryPath = _appConfigService.GetConfig().GlobalConfig.UploadFilePath;
            string _filepath = request.Filepath;
            string _path = string.Empty;

            //默认根目录
            if (string.IsNullOrEmpty(_filepath))
            {
                _path = $"{_directoryPath}/";
            }
            else
            {
                //过滤../参数
                if (_filepath.IndexOf("..") > -1)
                    _filepath = _filepath.Replace(".", "");
                if (_filepath.IndexOf("/") == 0)
                    _filepath = _filepath.Substring(1);

                _path = $"{_directoryPath}/{_filepath}/";
            }
            //如果不存在默认为根目录
            string _physicalPath = $"{_hostEnvironment.ContentRootPath}/wwwroot/{_path}";
            if (!Directory.Exists(_physicalPath))
            {
                _path = $"{_directoryPath}/";
            }
            //读取文件
            DirectoryInfo _dir = new DirectoryInfo(_physicalPath);
            //目录列表
            foreach (var _d in _dir.GetDirectories().OrderByDescending(p => p.LastWriteTime))
            {
                objFiles.Add(new UploadFileCollection()
                {
                    FileName = $"<i class=\"fa fa-folder-open text-warning\"></i><a href=\"javascript:appUpload.searchNextPath('{(!string.IsNullOrEmpty(_filepath) ? _filepath + "/" + _d.Name : _d.Name)}')\" class=\"href-blue\">{_d.Name}</a>",
                    FileExt = "folder",
                    FileSize = "",
                    EditTime = _d.LastWriteTime
                });
            }
            //文件列表
            foreach (var _f in _dir.GetFiles().OrderByDescending(p => p.LastWriteTime))
            {
                objFiles.Add(new UploadFileCollection()
                {
                    FileName = $"<a href=\"javascript:appUpload.selectFile('{_path}{_f.Name}')\" class=\"href-blue\">{this.GetFileName(_path, _f)}</a>",
                    FileExt = _f.Extension.Replace(".", ""),
                    FileSize = VariableHelper.FormatSize(_f.Length),
                    EditTime = _f.LastWriteTime
                });
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = objFiles.Count();
            _result.Items = objFiles.OrderByDescending(p => p.EditTime).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 格式化文件名称
        /// </summary>
        /// <param name="objFilePath"></param>
        /// <param name="objFileInfo"></param>
        /// <returns></returns>
        private string GetFileName(string objFilePath, FileInfo objFileInfo)
        {
            string _result = string.Empty;
            string _ext = objFileInfo.Extension.ToUpper();
            if (_ext == ".JPG" || _ext == ".JPEG" || _ext == ".GIF" || _ext == ".PNG" || _ext == ".BMP")
            {
                _result = $"<img src=\"{objFilePath}{objFileInfo.Name}\" style=\"width:50px;height:50px;border:1px silver solid;border-radius:3px;margin:1px;padding:1px;\" />{ objFileInfo.Name}";
            }
            else
            {
                _result = objFileInfo.Name;
            }
            return _result;
        }

        /// <summary>
        /// 通用保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UploadSaveResponse SaveFile(UploadSaveRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            List<string> _fileList = new List<string>();
            string _directoryPath = _appConfigService.GetConfig().GlobalConfig.UploadFilePath;
            string _filePath = string.Empty;

            try
            {
                SysUploadModel objSysUploadModel = _appDB.SysUploadModel.Where(p => p.ModelMark == request.Model).SingleOrDefault();
                if (objSysUploadModel != null)
                {
                    if (!string.IsNullOrEmpty(request.Catalog))
                    {
                        objSysUploadModel.SaveCatalog = request.Catalog;
                    }

                    if (_httpContextAccessor.HttpContext.Request.Form.Files.Count == 0)
                    {
                        throw new Exception("Please select a file to upload!");
                    }

                    long _fileSize = 0;
                    string _oFileName = string.Empty;
                    string _oFileExt = string.Empty;
                    string _nFileName = string.Empty;
                    if (_httpContextAccessor.HttpContext.Request.Form.Files.Count <= objSysUploadModel.MaxFileCount)
                    {
                        if (objSysUploadModel.SaveStyle == "fileorder")
                        {
                            _filePath = $"/{ objSysUploadModel.SaveCatalog}";
                        }
                        else
                        {
                            _filePath = $"/{ objSysUploadModel.SaveCatalog}/{DateTime.Now.ToString("yyyy-MM-dd")}";
                        }
                        _directoryPath = $"{_directoryPath}/{_filePath}";
                        string _physicalDirectoryPath = $"{_hostEnvironment.ContentRootPath}/wwwroot/{_directoryPath}";
                        //创建目录
                        if (!Directory.Exists(_physicalDirectoryPath))
                            Directory.CreateDirectory(_physicalDirectoryPath);
                        //循环上传文件
                        foreach (var file in _httpContextAccessor.HttpContext.Request.Form.Files)
                        {
                            _fileSize = file.Length;
                            if (_fileSize > 0)
                            {
                                _oFileName = file.FileName;
                                if (_fileSize <= objSysUploadModel.MaxFileSize)
                                {
                                    _oFileExt = Path.GetExtension(_oFileName).Substring(1);
                                    if (("|" + objSysUploadModel.AllowFile + "|").ToUpper().IndexOf("|" + _oFileExt.ToUpper() + "|") > -1)
                                    {
                                        //是否重命名
                                        if (objSysUploadModel.IsRename)
                                        {
                                            _nFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{_fileSize.ToString()}.{_oFileExt}";
                                        }
                                        else
                                        {
                                            _nFileName = _oFileName.Substring(_oFileName.LastIndexOf("\\") + 1);
                                        }
                                        //添加到文件集合
                                        _fileList.Add($"{_directoryPath}/{ _nFileName}");
                                        //保存文件
                                        using (var stream = new FileStream($"{_physicalDirectoryPath}/{_nFileName}", FileMode.Create))
                                        {
                                            file.CopyTo(stream);
                                            stream.Flush();
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"The format of ({_oFileExt}) is not allowed to upload!");
                                    }
                                }
                                else
                                {
                                    throw new Exception($"The size of files must be less than {objSysUploadModel.MaxFileSize}!");
                                }
                            }
                            else
                            {
                                throw new Exception($"The size of files must be biger than 0!");
                            }
                        }

                        //返回信息
                        return new UploadSaveResponse()
                        {
                            FileName = string.Join("|", _fileList),
                            FilePath = _filePath,
                            Result = true,
                            Message = string.Empty
                        };
                    }
                    else
                    {
                        throw new Exception($"The quantity of files must be less than {objSysUploadModel.MaxFileCount}!");
                    }
                }
                else
                {
                    throw new Exception(_languagePack["common_data_load_false"]);
                }
            }
            catch (Exception ex)
            {
                //返回信息
                return new UploadSaveResponse()
                {
                    FileName = string.Empty,
                    FilePath = string.Empty,
                    Result = false,
                    Message = ex.Message
                };
            }
        }
    }
}
