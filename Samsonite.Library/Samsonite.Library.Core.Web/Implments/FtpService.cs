using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Samsonite.Library.Core.Web
{
    public class FtpService : IFtpService
    {
        private appEntities _appDB;
        public FtpService(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 获取Ftp信息
        /// </summary>
        /// <param name="ftpIdentify"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public FtpDto GetFtp(string ftpIdentify, bool isDelete = false)
        {
            FTPInfo fTPInfo = _appDB.FTPInfo.Where(p => p.FTPIdentify == ftpIdentify).SingleOrDefault();
            if (fTPInfo != null)
            {
                return new FtpDto()
                {
                    FtpName = fTPInfo.FTPName,
                    FtpServerIp = fTPInfo.IP,
                    Port = fTPInfo.Port,
                    UserId = fTPInfo.UserName,
                    Password = fTPInfo.Password,
                    RemoteFilePath = fTPInfo.FilePath,
                    IsDeleteOriginalFile = isDelete
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 从FTP下载文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public FTPResult DownFileFromFTP(SapFTPDto config)
        {
            FtpDto ftpDto = config.Ftp;
            //FTP文件目录
            SFTPHelper sftpHelper = new SFTPHelper(ftpDto.FtpServerIp, ftpDto.UserId, ftpDto.Password);
            //本地保存文件目录
            string _localSavePath = $"{AppDomain.CurrentDomain.BaseDirectory + config.LocalSavePath}/{DateTime.Now.ToString("yyyy-MM")}/{DateTime.Now.ToString("yyyyMMdd")}";
            //下载文件
            return this.GetFilesFromSFtp(sftpHelper, config.RemoteFilePath, _localSavePath, config.FileExt, ftpDto.IsDeleteOriginalFile);
        }

        #region SFTP
        /// <summary>
        /// 从SFTP上读取特定格式文件
        /// </summary>
        /// <param name="sFTPHelper">FTP对象</param>
        /// <param name="remoteFilePath">FTP文件目录路径</param>
        /// <param name="localSavePath">本地文件目录路径</param>
        /// <param name="fileExt">文件后缀名</param>
        /// <param name="isDelete">是否删除ftp上文件</param>
        /// <returns></returns>
        public FTPResult GetFilesFromSFtp(SFTPHelper sFTPHelper, string remoteFilePath, string localSavePath, string fileExt, bool isDelete = true)
        {
            FTPResult _result = new FTPResult();
            _result.SuccessFile = new List<string>();
            _result.FailFile = new List<string>();
            //检测文件路径是否存在
            if (!Directory.Exists(localSavePath)) Directory.CreateDirectory(localSavePath);
            //打开ftp连接
            sFTPHelper.Connect();
            var _ftpFileNames = sFTPHelper.GetFileList(remoteFilePath, SFTPHelper.FileType.OnlyFile, fileExt);
            //读取文件
            foreach (var _file in _ftpFileNames)
            {
                var _ftpFile = remoteFilePath + "/" + _file;
                var _localFile = localSavePath + "/" + _file;
                //下载文件到本地
                if (sFTPHelper.Get(_ftpFile, _localFile))
                {
                    _result.SuccessFile.Add(_localFile);
                    //删除ftp上的文件
                    if (isDelete)
                    {
                        sFTPHelper.Delete(_ftpFile);
                    }
                }
                else
                {
                    _result.FailFile.Add(_file.ToString());
                }
            }
            //释放ftp连接
            sFTPHelper.Disconnect();
            return _result;
        }

        /// <summary>
        /// 上传文件到SFTP
        /// </summary>
        /// <param name="sFTPHelper">FTP对象</param>
        /// <param name="localFilePath">本地文件路径</param>
        /// <param name="remoteFilePath">FTP文件目录路径</param>
        /// <returns></returns>
        public bool SendXMLToSFtp(SFTPHelper sFTPHelper, string localFilePath, string remoteFilePath)
        {
            try
            {
                //打开ftp连接
                sFTPHelper.Connect();
                if (sFTPHelper.Put(localFilePath, remoteFilePath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                //释放ftp连接
                sFTPHelper.Disconnect();
            }
        }

        /// <summary>
        /// 批量上传文件到SFTP
        /// </summary>
        /// <param name="sFTPHelper"></param>
        /// <param name="localFilePathList"></param>
        /// <param name="remoteFilePath"></param>
        /// <returns></returns>
        public List<FtpPutBatchResult> SendXMLToSFtp(SFTPHelper sFTPHelper, List<string> localFilePathList, string remoteFilePath)
        {
            List<FtpPutBatchResult> _result = new List<FtpPutBatchResult>();
            if (localFilePathList.Count > 0)
            {
                try
                {
                    //打开ftp连接
                    sFTPHelper.Connect();
                    foreach (string _f in localFilePathList)
                    {
                        var _r = sFTPHelper.Put(_f, remoteFilePath);
                        if (_r)
                        {
                            _result.Add(new FtpPutBatchResult() { FilePath = _f, Result = true });
                        }
                        else
                        {
                            _result.Add(new FtpPutBatchResult() { FilePath = _f, Result = false });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //释放ftp连接
                    sFTPHelper.Disconnect();
                }
            }
            return _result;
        }
        #endregion
    }
}
