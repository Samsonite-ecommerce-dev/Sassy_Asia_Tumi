using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Utility;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Web
{
    public interface IFtpService
    {
        /// <summary>
        /// 获取Ftp信息
        /// </summary>
        /// <param name="ftpIdentify"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        FtpDto GetFtp(string ftpIdentify, bool isDelete = false);

        /// <summary>
        /// 从FTP下载文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        FTPResult DownFileFromFTP(SapFTPDto config);

        /// <summary>
        /// 从SFTP上读取特定格式文件
        /// </summary>
        /// <param name="sFTPHelper">FTP对象</param>
        /// <param name="remoteFilePath">FTP文件目录路径</param>
        /// <param name="localSavePath">本地文件目录路径</param>
        /// <param name="fileExt">文件后缀名</param>
        /// <param name="isDelete">是否删除ftp上文件</param>
        /// <returns></returns>
        FTPResult GetFilesFromSFtp(SFTPHelper sFTPHelper, string remoteFilePath, string localSavePath, string fileExt, bool isDelete = true);

        /// <summary>
        /// 上传文件到SFTP
        /// </summary>
        /// <param name="sFTPHelper">FTP对象</param>
        /// <param name="localFilePath">本地文件路径</param>
        /// <param name="remoteFilePath">FTP文件目录路径</param>
        /// <returns></returns>
        bool SendXMLToSFtp(SFTPHelper sFTPHelper, string localFilePath, string remoteFilePath);

        /// <summary>
        /// 批量上传文件到SFTP
        /// </summary>
        /// <param name="sFTPHelper"></param>
        /// <param name="localFilePathList"></param>
        /// <param name="remoteFilePath"></param>
        /// <returns></returns>
        List<FtpPutBatchResult> SendXMLToSFtp(SFTPHelper sFTPHelper, List<string> localFilePathList, string remoteFilePath);
    }
}
