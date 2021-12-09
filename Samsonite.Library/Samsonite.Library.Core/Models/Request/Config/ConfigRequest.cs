namespace Samsonite.Library.Core.Models
{
    public class ConfigUpdateRequest
    {
        public string LanguagePack { get; set; }

        //public string ProductIDConfig { get; set; }

        //public int AmountAccuracy { get; set; }

        public string MailStmp { get; set; }

        public int MailPort { get; set; }

        public string MailUserName { get; set; }

        public string MailPassword { get; set; }

        public string SMSHost { get; set; }

        public string SMSAccount { get; set; }

        public string SMSAuthToken { get; set; }

        public string SMSSender { get; set; }

        //public bool IsUseAPI { get; set; }

        public string SkinStyle { get; set; }
    }
}
