
using Newtonsoft.Json.Linq;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.WebApi.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Samsonite.Sassy.Test
{
    public class TestApi
    {
        private string localSite = string.Empty;
        private string userid = string.Empty;
        private string version = string.Empty;
        private string format = string.Empty;
        private string secret = string.Empty;
        private string method = string.Empty;
        private UtilsHelper _utilsHelper;
        public TestApi()
        {
            //测试
            this.localSite = "http://tumisassyapitest.tumi-asia.com/";
            //this.localSite = "http://127.0.0.1:5001/";
            this.secret = "c83QnLScml1nn544uXwO55JgdVQ4Bf9h";

            //正式
            //private string localSite = "https://sassy.tumi-asia.com/";
            //private static string secret = "c83QnLScml1nn544uXwO55JgdVQ4Bf9h";

            this.userid = "sasuser";
            this.version = "1.0";
            this.format = "json";
            this.method = "md5";

            _utilsHelper = new UtilsHelper();
        }

        #region SAS
        public void TestSAS()
        {
            //APIGetProducts();

            //APIGetSpareParts();

            APIGetRelatedSpareParts();

            //APIGetSparePartGroups();
        }

        public void APIGetProducts()
        {
            try
            {
                IDictionary<string, string> objParams = new Dictionary<string, string>();
                //默认参数
                objParams.Add("userid", this.userid);
                objParams.Add("version", this.version);
                objParams.Add("format", this.format);
                objParams.Add("timestamp", TimeHelper.DateTimeToUnixTimestamp(DateTime.Now).ToString());
                //传递参数
                objParams.Add("pagesize", "100");
                objParams.Add("currentpage", "1");
                //签名
                objParams.Add("sign", _utilsHelper.CreateSign(objParams, this.secret, this.method));
                //执行请求
                this.DoGet($"{this.localSite}sas/products/get", objParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void APIGetSpareParts()
        {
            try
            {
                IDictionary<string, string> objParams = new Dictionary<string, string>();
                //默认参数
                objParams.Add("userid", this.userid);
                objParams.Add("version", this.version);
                objParams.Add("format", this.format);
                objParams.Add("timestamp", TimeHelper.DateTimeToUnixTimestamp(DateTime.Now).ToString());
                //传递参数
                objParams.Add("pagesize", "100");
                objParams.Add("currentpage", "1");
                //签名
                objParams.Add("sign", _utilsHelper.CreateSign(objParams, this.secret, this.method));
                //执行请求
                this.DoGet($"{this.localSite}sas/spareparts/get", objParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void APIGetRelatedSpareParts()
        {
            try
            {
                IDictionary<string, string> objParams = new Dictionary<string, string>();
                //默认参数
                objParams.Add("userid", this.userid);
                objParams.Add("version", this.version);
                objParams.Add("format", this.format);
                objParams.Add("timestamp", TimeHelper.DateTimeToUnixTimestamp(DateTime.Now).ToString());
                //传递参数
                objParams.Add("sku", "025503960TTMG3");
                //objParams.Add("groupid", "312");
                //签名
                objParams.Add("sign", _utilsHelper.CreateSign(objParams, this.secret, this.method));
                //执行请求
                this.DoGet($"{this.localSite}sas/spareparts/related/get", objParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void APIGetSparePartGroups()
        {
            try
            {
                IDictionary<string, string> objParams = new Dictionary<string, string>();
                //默认参数
                objParams.Add("userid", this.userid);
                objParams.Add("version", this.version);
                objParams.Add("format", this.format);
                objParams.Add("timestamp", TimeHelper.DateTimeToUnixTimestamp(DateTime.Now).ToString());
                //传递参数
                //签名
                objParams.Add("sign", _utilsHelper.CreateSign(objParams, this.secret, this.method));
                //执行请求
                this.DoGet($"{this.localSite}sas/spareparts/groups/get", objParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region 函数
        /// <summary>
        /// get
        /// </summary>
        /// <param name="objUrl"></param>
        /// <param name="objParams"></param>
        private void DoGet(string objUrl, IDictionary<string, string> objParams)
        {
            string _params = string.Empty;
            foreach (var _item in objParams)
            {
                _params += $"&{_item.Key}={_item.Value}";
            }
            _params = _params.Substring(1);
            objUrl = $"{objUrl}?{_params}";
            HttpWebRequest req = null;
            if (this.localSite.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TrustAllValidationCallback);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(objUrl));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(objUrl);
            }
            req.Method = WebRequestMethods.Http.Get;
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
                response.Close();
            }
            DateTime _endTime = DateTime.Now;
            TimeSpan TS = new TimeSpan(_endTime.Ticks - _beginTime.Ticks);
            Console.WriteLine(TS);
        }

        /// <summary>
        /// post
        /// </summary>
        /// <param name="objUrl"></param>
        /// <param name="objParams"></param>
        /// <param name="objPostData"></param>
        private void DoPost(string objUrl, IDictionary<string, string> objParams, object objPostData)
        {
            string _params = string.Empty;
            foreach (var _item in objParams)
            {
                _params += $"&{_item.Key}={_item.Value}";
            }
            _params = _params.Substring(1);
            objUrl = $"{objUrl}?{_params}";
            //获取信息
            HttpWebRequest req = null;
            if (this.localSite.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TrustAllValidationCallback);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(objUrl));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(objUrl);
            }

            req.ReadWriteTimeout = 5 * 1000;
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = "application/json";
            var _data = JsonSerializer.Serialize(objPostData);

            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(_data);
            }
            using (var response = req.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    Console.WriteLine(responseText);
                    JObject obj = JsonHelper.JsonDeserialize<JObject>(responseText);
                    if (obj.GetValue("Code").ToString() == "100")
                    {
                        Console.WriteLine("success");
                    }
                    else
                    {
                        Console.WriteLine("fail");
                    }
                }
            }
        }

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
        #endregion

        private static bool TrustAllValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; // ignore ssl certificate check
        }
    }
}
