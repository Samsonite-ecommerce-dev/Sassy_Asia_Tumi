using System.Collections.Generic;

namespace Samsonite.Library.Core.Models
{
    public class SapFTPDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ftp配置信息
        /// </summary>
        public FtpDto Ftp { get; set; }

        /// <summary>
        /// 下载路径
        /// </summary>
        public string RemoteFilePath { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>
        /// 本地保存路径
        /// </summary>
        public string LocalSavePath { get; set; }
    }

    /// <summary>
    /// FTP对象
    /// </summary>
    public class FtpDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string FtpName { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string FtpServerIp { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }


        /// <summary>
        /// 账号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ftp目录
        /// </summary>
        public string RemoteFilePath { get; set; }

        /// <summary>
        /// 是否删除原始文件
        /// </summary>
        public bool IsDeleteOriginalFile { get; set; }
    }

    public class FtpPutBatchResult
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; set; }
    }

    /// <summary>
    /// ftp返回文件
    /// </summary>
    public class FTPResult
    {
        /// <summary>
        /// 成功文件
        /// </summary>
        public List<string> SuccessFile { get; set; }

        /// <summary>
        /// 失败文件
        /// </summary>
        public List<string> FailFile { get; set; }
    }
}
