using System;
using System.Collections.Generic;
using System.Linq;
using Renci.SshNet;
using System.IO;

namespace Samsonite.Library.Utility
{
    public class SFTPHelper
    {
        #region private
        private SftpClient Sftp { get; set; }
        public bool Connected
        {
            get
            {
                return Sftp.IsConnected;
            }
        }
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    Sftp.Connect();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new FtpException(ex.Message);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (Sftp != null && Connected)
                {
                    Sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new FtpException(ex.Message);
            }
        }
        public static void CheckLocalFilePath(string localPath)
        {
            var dirPath = localPath.Remove(localPath.Replace("\\", "/").LastIndexOf("/"));
            if (!File.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        #endregion

        public SFTPHelper(string host, string userName, string password)
        {
            string[] arr = host.Split(':');
            string address = arr[0];
            int port = 22;
            if (arr.Length > 1)
            {
                port = Int32.Parse(arr[1]);
            }
            Sftp = new SftpClient(address, port, userName, password);
        }

        /// <summary>
        /// SFTP上传文件
        /// </summary>
        /// <param name="localFilePath">本地路径</param>
        /// <param name="remoteDirectoryPath">远程路径</param>
        /// <param name="remoteFileName">远程文件名</param>
        public bool Put(string localFilePath, string remoteDirectoryPath, string remoteFileName)
        {
            try
            {
                using var file = File.OpenRead(localFilePath);
                if (!Sftp.Exists(remoteDirectoryPath))
                {
                    Sftp.CreateDirectory(remoteDirectoryPath);
                }
                Sftp.UploadFile(file, $"{remoteDirectoryPath}/{remoteFileName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// SFTP上传文件
        /// </summary>
        /// <param name="localFilePath">本地路径</param>
        /// <param name="remoteFilePath">远程路径</param>
        public bool Put(string localFilePath, string remoteFilePath)
        {
            try
            {
                using var file = File.OpenRead(localFilePath);
                Sftp.UploadFile(file, remoteFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// SFTP获取文件
        /// </summary>
        /// <param name="remoteFilePath">远程路径</param>
        /// <param name="localFilePath">本地路径</param>
        public bool Get(string remoteFilePath, string localFilePath)
        {
            try
            {
                CheckLocalFilePath(localFilePath);
                var byt = Sftp.ReadAllBytes(remoteFilePath);
                File.WriteAllBytes(localFilePath, byt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 下载目录下面的所有文件
        /// </summary>
        /// <param name="remoteDirectoryPath">远程文件夹路径</param>
        /// <param name="localDirectoryPath">本地文件夹路径</param>
        public void GetAll(string remoteDirectoryPath, string localDirectoryPath)
        {
            try
            {
                Sftp.ListDirectory(remoteDirectoryPath).ToList().ForEach(f =>
                {
                    if (!f.IsDirectory)
                    {
                        var remoteFilePath = $"{remoteDirectoryPath}/{f.Name}";
                        var localFilePath = $"{localDirectoryPath}/{f.Name}";
                        CheckLocalFilePath(localFilePath);
                        var byt = Sftp.ReadAllBytes(remoteFilePath);
                        File.WriteAllBytes(localFilePath, byt);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new FtpException(string.Format(ex.Message));
            }
        }

        /// <summary>
        /// 获取SFTP文件列表
        /// </summary>
        /// <param name="remoteDirectoryPath">远程目录</param>
        /// <param name="fileSuffix">文件后缀</param>
        /// <returns></returns>
        public List<string> GetFileList(string remoteDirectoryPath, FileType fileType = FileType.OnlyFile, string fileSuffix = null)
        {
            try
            {
                var files = Sftp.ListDirectory(remoteDirectoryPath);
                var objList = new List<string>();
                foreach (var file in files)
                {
                    string name = file.Name;
                    if (fileType == FileType.All)
                    {
                        objList.Add(name);
                    }
                    else if (fileType == FileType.OnlyDir && file.IsDirectory)
                    {
                        objList.Add(name);
                    }
                    else if (fileType == FileType.OnlyFile && !file.IsDirectory)
                    {
                        if (string.IsNullOrEmpty(fileSuffix) ||
                            (name.Length > (fileSuffix.Length + 1) && fileSuffix.ToLower() == name.ToLower().Substring(name.Length - fileSuffix.Length)))
                        {
                            objList.Add(name);
                        }
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new FtpException(string.Format(ex.Message));
            }
        }

        /// <summary>
        /// 移动SFTP文件
        /// </summary>
        /// <param name="oldRemotePath">旧远程路径</param>
        /// <param name="newRemotePath">新远程路径</param>
        public void Move(string oldRemotePath, string newRemotePath)
        {
            try
            {
                Sftp.RenameFile(oldRemotePath, newRemotePath);
            }
            catch (Exception ex)
            {
                throw new FtpException(string.Format(ex.Message));
            }
        }

        /// <summary>
        /// 删除SFTP文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        public void Delete(string remoteFilePath)
        {
            try
            {
                Sftp.Delete(remoteFilePath);
            }
            catch (Exception ex)
            {
                throw new FtpException(string.Format(ex.Message));
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="remoteDirectoryPath">远程目录</param>
        public void CreateDirectory(string remoteDirectoryPath)
        {
            try
            {
                string[] paths = remoteDirectoryPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string curPath = "/";
                for (int i = 0; i < paths.Length; i++)
                {
                    curPath += paths[i];
                    if (!Sftp.Exists(curPath))
                    {
                        Sftp.CreateDirectory(curPath);
                    }
                    if (i < paths.Length - 1)
                    {
                        curPath += "/";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FtpException(string.Format(ex.Message));
            }
        }
        public enum FileType
        {
            OnlyFile,
            OnlyDir,
            All,
        }
    }//end class
    public class FtpException : Exception
    {
        public FtpException() : base()
        {

        }
        public FtpException(string message) : base(message)
        {

        }
    }
}//end namespace
