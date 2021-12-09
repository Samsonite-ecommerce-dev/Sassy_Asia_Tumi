namespace Samsonite.Library.Core.Models
{
    /// <summary>
    /// 短信信息
    /// </summary>
    public class SMSModel
    {
        /// <summary>
        /// 短信接口地址
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccountSid { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 发送号码
        /// </summary>
        public string SendPhoneNumber { get; set; }
    }
}
