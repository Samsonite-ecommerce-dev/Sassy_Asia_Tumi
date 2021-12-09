using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Samsonite.Library.Utility
{
    public class FTPHelper
    {
        #region 属性

        /// <summary>
        /// Ftp服务器地址（IP或域名）
        /// </summary>
        public string FtpServerAddress { get; set; }

        /// <summary>
        /// FTP服务器默认目录
        /// </summary>
        public string FtpFilePath { get; set; }

        /// <summary>
        /// FTP服务器登录用户名
        /// </summary>
        public string FtpUserID { get; set; }

        /// <summary>
        /// FTP服务器登录密码
        /// </summary>
        public string FtpPassword { get; set; }

    #endregion

        #region private
        private FtpWebRequest request = null;
        private FtpWebResponse response = null;
        private string FtpURI
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FtpServerAddress))
                {
                    throw new Exception("ftp服务器地址未设置！");
                }
                FtpServerAddress = FtpServerAddress.Replace(" ", "");
                FtpFilePath = FtpFilePath.Replace(" ", "");
                return $"ftp://{FtpServerAddress}/{FtpFilePath}/";
            }
        }
        private FtpWebResponse Open(Uri uri, string ftpMethod)
        {
            request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Method = ftpMethod;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Credentials = new NetworkCredential(this.FtpUserID, this.FtpPassword);
            return (FtpWebResponse)request.GetResponse();
        }
        private FtpWebRequest OpenRequest(Uri uri, string ftpMethod)
        {
            request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = ftpMethod;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Credentials = new NetworkCredential(this.FtpUserID, this.FtpPassword);
            return request;
        }
        private FtpOperationResult<T> Operation<T>(Func<T> fun)
        {
            try
            {
                var data = fun();
                return new FtpOperationResult<T>
                {
                    IsSuccessful = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new FtpOperationResult<T>
                {
                    IsSuccessful = false,
                    Message = ex.Message,
                    Exception = ex,
                };
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        #endregion

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="saveFilePath">下载后的保存路径</param>
        /// <param name="downloadFileName">要下载的文件名</param>
        public void Download(string saveFilePath, string downloadFileName)
        {
                using FileStream outputStream = new FileStream(saveFilePath + "\\" + downloadFileName, FileMode.Create);
                response = Open(new Uri(FtpURI + downloadFileName), WebRequestMethods.Ftp.DownloadFile);
                using Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="localFilePath">本地文件路径</param>
        public void Upload(string localFilePath)
        {
                FileInfo fileInf = new FileInfo(localFilePath);
                request = OpenRequest(new Uri(FtpURI + fileInf.Name), WebRequestMethods.Ftp.UploadFile);
                request.ContentLength = fileInf.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                using (var fs = fileInf.OpenRead())
                {
                    using (var strm = request.GetRequestStream())
                    {
                        contentLen = fs.Read(buff, 0, buffLength);
                        while (contentLen != 0)
                        {
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }
                    }
                }
        }

        #region others
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="remoteDirectoryName">目录名</param>
        public void CreateDirectory(string remoteDirectoryName)
        {
                response = Open(new Uri(FtpURI + remoteDirectoryName), WebRequestMethods.Ftp.MakeDirectory);
        }

        /// <summary>
        /// 更改目录或文件名
        /// </summary>
        /// <param name="currentName">当前名称</param>
        /// <param name="newName">修改后新名称</param>
        public void ReName(string currentName, string newName)
        {
                request = OpenRequest(new Uri(FtpURI + currentName), WebRequestMethods.Ftp.Rename);
                request.RenameTo = newName;
                response = (FtpWebResponse)request.GetResponse();
        }

        /// <summary>  
        /// 切换当前目录  
        /// </summary>
        /// <param name="DirectoryName"></param> 
        /// <param name="IsRoot">true:绝对路径 false:相对路径</param>   
        public void GotoDirectory(string DirectoryName, bool IsRoot)
        {
                if (IsRoot)
                {
                    FtpFilePath = DirectoryName;
                }
                else
                {
                    FtpFilePath += "/" + DirectoryName;
                }
        }

        /// <summary>
        /// 删除目录(包括下面所有子目录和子文件)
        /// </summary>
        /// <param name="remoteDirectoryName">要删除的带路径目录名：RemoveDirectory("web/test")</param>
        public void RemoveDirectory(string remoteDirectoryName)
        {
                GotoDirectory(remoteDirectoryName, true);
                var listAll = ListFilesAndDirectories();
                foreach (var m in listAll)
                {
                    if (m.IsDirectory)
                        RemoveDirectory(m.Path);
                    else
                        DeleteFile(m.Name);
                }
                GotoDirectory(remoteDirectoryName, true);
                response = Open(new Uri(FtpURI), WebRequestMethods.Ftp.RemoveDirectory);
        }

        /// <summary>  
        /// 删除文件  
        /// </summary>  
        /// <param name="remoteFileName">要删除的文件名</param>
        public void DeleteFile(string remoteFileName)
        {
                response = Open(new Uri(FtpURI + remoteFileName), WebRequestMethods.Ftp.DeleteFile);
        }

        /// <summary>
        /// 获取当前目录的文件和一级子目录信息
        /// </summary>
        /// <returns>该方法并不可靠，注意服务器返回格式，特别注意文件名是否包含空格。</returns>
        public List<FileStruct> ListFilesAndDirectories()
        {
            var fileList = new List<FileStruct>();
            response = Open(new Uri(FtpURI), WebRequestMethods.Ftp.ListDirectoryDetails);
            using (var stream = response.GetResponseStream())
            {
                using var sr = new StreamReader(stream);
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    /*windows 格式
                     * 08-10-11  12:02PM       <DIR>          Version2
                        06-25-09  02:41PM            144700153 image34.gif
                        06-25-09  02:51PM            144700153 update orders.txt  //该文件名中有空格将导致无法正确获取文件名
                        11-04-10  02:45PM            144700214 digger.tif

                    *unix 格式
                    *   d--x--x--x    2 ftp      ftp          4096 Mar 07  2002 bin
                        -rw-r--r--    1 ftp      ftp        659450 Jun 15 05:07 TEST.TXT
                        -rw-r--r--    1 ftp      ftp      101786380 Sep 08  2008 TEST03-05.TXT
                        drwxrwxr-x    2 ftp      ftp          4096 May 06 12:24 dropoff
                     */

                    string[] arrs = line.Split(' ');
                    var name = arrs[^1];
                    if (name != "." && name != "..")
                    {
                        fileList.Add(new FileStruct()
                        {
                            IsDirectory = line.ToLower().Contains("<dir>") || line.Trim().Substring(0,1).ToLower() == "d",
                            Name = name,
                            Path =  $"{FtpFilePath}/{name}"
                        });
                    }
                }
            }
            return fileList;
        }

        /// <summary>       
        /// 列出当前目录的所有文件       
        /// </summary>       
        public List<FileStruct> ListFiles()
        {
                return ListFilesAndDirectories().Where(m => !m.IsDirectory).ToList();
        }

        /// <summary>       
        /// 列出当前目录的所有一级子目录       
        /// </summary>       
        public List<FileStruct> ListDirectories()
        {
            return ListFilesAndDirectories().Where(m => m.IsDirectory).ToList();
        }

        /// <summary>       
        /// 判断当前目录下指定的子目录或文件是否存在       
        /// </summary>       
        /// <param name="remoteName">指定的目录或文件名</param>      
        public bool IsExist(string remoteName)
        {
            return ListFilesAndDirectories().Any(m => m.Name == remoteName);
        }

        /// <summary>       
        /// 判断当前目录下指定的一级子目录是否存在       
        /// </summary>       
        /// <param name="remoteDirectoryName">指定的目录名</param>      
        public bool IsDirectoryExist(string remoteDirectoryName)
        {
            return ListDirectories().Any(m => m.Name == remoteDirectoryName);
        }

        /// <summary>       
        /// 判断当前目录下指定的子文件是否存在      
        /// </summary>       
        /// <param name="remoteFileName">远程文件名</param>       
        public bool IsFileExist(string remoteFileName)
        {
            return ListFiles().Any(m => m.Name == remoteFileName);
        }

        #endregion
    }//end class

    public class FileStruct
    {
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        //public DateTime CreateTime { get; set; }
        /// <summary>
        /// 文件或目录名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
    }
    public class FtpOperationResult<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public T Data { get; set; }
    }
   
}// end namespace
