using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Samsonite.Sassy.Test
{
    public class TestApi
    {
        //测试
        private static string localSite = "http://localhost:5001/";
        //private static string localSite = "http://sassytest.tumi-asia.com/";
        private static string secret = "c83QnLScml1nn544uXwO55JgdVQ4Bf9h";

        //正式
        //private string localSite = "https://sassy.tumi-asia.com/";
        //private static string secret = "c83QnLScml1nn544uXwO55JgdVQ4Bf9h";

        #region SAS
        public static void TestSAS()
        {
            APIGetRelatedSpareParts();

            //APIGetSparePartGroups();
        }

        public static void APIGetRelatedSpareParts()
        {
            try
            {
                IDictionary<string, string> objParams = new Dictionary<string, string>();
                //默认参数
                objParams.Add("userid", "sasuser");
                objParams.Add("version", "1.0");
                objParams.Add("format", "json");
                objParams.Add("timestamp", TimeHelper.DateTimeToUnixTimestamp(DateTime.Now).ToString());
                //传递参数
                //objParams.Add("sku", "I25*");
                objParams.Add("groupid", "312");
                //测试Key
                string _sign = CreateSign(objParams, secret);
                objParams.Add("sign", _sign);
                string _params = string.Empty;
                foreach (var _item in objParams)
                {
                    _params += $"&{_item.Key}={_item.Value}";
                }
                _params = _params.Substring(1);
                localSite = $"{localSite}sas/spareparts/related/get?{_params}";
                //Console.WriteLine(localSite);
                HttpWebRequest req = null;
                if (localSite.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TrustAllValidationCallback);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(localSite));
                }
                else
                {
                    req = (HttpWebRequest)WebRequest.Create(localSite);
                }
                req.Method = "GET";
                //req.KeepAlive = true;
                req.Timeout = 0xea60;
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                DateTime _beginTime = DateTime.Now;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string _r = reader.ReadToEnd();
                        Console.WriteLine(_r);
                    }
                    if (response != null) response.Close();
                }
                DateTime _endTime = DateTime.Now;
                TimeSpan TS = new TimeSpan(_endTime.Ticks - _beginTime.Ticks);
                Console.WriteLine(TS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void APIGetSparePartGroups()
        {
            try
            {
                IDictionary<string, string> objParams = new Dictionary<string, string>();
                //默认参数
                objParams.Add("userid", "sasuser");
                objParams.Add("version", "1.0");
                objParams.Add("format", "json");
                objParams.Add("timestamp", TimeHelper.DateTimeToUnixTimestamp(DateTime.Now).ToString());
                //传递参数
                //测试Key
                string _sign = CreateSign(objParams, secret);
                objParams.Add("sign", _sign);
                string _params = string.Empty;
                foreach (var _item in objParams)
                {
                    _params += $"&{_item.Key}={_item.Value}";
                }
                _params = _params.Substring(1);
                localSite = $"{localSite}sas/spareparts/groups/get?{_params}";
                //Console.WriteLine(localSite);
                HttpWebRequest req = null;
                if (localSite.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TrustAllValidationCallback);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(localSite));
                }
                else
                {
                    req = (HttpWebRequest)WebRequest.Create(localSite);
                }
                req.Method = "GET";
                //req.KeepAlive = true;
                req.Timeout = 0xea60;
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                DateTime _beginTime = DateTime.Now;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string _r = reader.ReadToEnd();
                        Console.WriteLine(_r);
                    }
                    if (response != null) response.Close();
                }
                DateTime _endTime = DateTime.Now;
                TimeSpan TS = new TimeSpan(_endTime.Ticks - _beginTime.Ticks);
                Console.WriteLine(TS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        private static string CreateSign(IDictionary<string, string> parameters, string secret)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            string _query = "";
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                //过滤null和空的字符
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    _query += $"{key}{value}";
                }
            }

            // 第三步：使用HMAC加密
            byte[] bytes;
            HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
            bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(_query));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }

        private static bool TrustAllValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; // ignore ssl certificate check
        }
    }
}
