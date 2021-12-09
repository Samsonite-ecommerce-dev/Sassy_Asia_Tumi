using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class SMSSend
    {
        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 接收人手机号码
        /// </summary>
        public string RecvMobile { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 短信标题
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// 发件人ID，0表示系统
        /// </summary>
        public int SendUserid { get; set; }

        /// <summary>
        /// 发件人IP地址
        /// </summary>
        public string SendUserIP { get; set; }

        /// <summary>
        /// 发送状态(0:未发送,1:已发送,2:发送失败)
        /// </summary>
        public int SendState { get; set; }

        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendCount { get; set; }

        /// <summary>
        /// 发送结果
        /// </summary>
        public string SendMessage { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
