using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class FTPInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// FTP标识
        /// </summary>
        public string FTPIdentify { get; set; }

        /// <summary>
        /// FTP名称
        /// </summary>
        public string FTPName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 远程路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序ID
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
