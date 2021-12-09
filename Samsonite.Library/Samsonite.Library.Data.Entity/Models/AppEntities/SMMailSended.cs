using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SMMailSended
    {
        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 接收人邮箱
        /// </summary>
        public string RecvMail { get; set; }

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string MailTitle { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailContent { get; set; }

        /// <summary>
        /// 附件地址
        /// </summary>
        public string MailAttachment { get; set; }

        /// <summary>
        /// 发件人ID，0表示系统
        /// </summary>
        public int SendUserid { get; set; }

        /// <summary>
        /// 发件人IP地址
        /// </summary>
        public string SendUserIP { get; set; }

        /// <summary>
        /// 发送状态(0:为发送,1:已发送)
        /// </summary>
        public int SendState { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 邮件发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

    }
}
