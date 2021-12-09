using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;

namespace Samsonite.Library.Utility
{
    public class HttpHelper
    {
        public static string GetRequestIP(HttpContext context)
        {
            string ip = string.Empty;
            if (context != null)
            {
                ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Connection.RemoteIpAddress.ToString();
                }
            }
            return ip;
        }

        public static string GetAbsolutePath(HttpContext context)
        {
            string url = string.Empty;
            if (context != null)
            {
                bool isHttps = context.Request.IsHttps;
                string host = context.Request.Host.Value;
                url = context.Request.GetDisplayUrl().Replace(host, "");
                if (isHttps)
                {
                    url = url.Replace("https://", "");
                }
                else
                {
                    url = url.Replace("http://", "");
                }
            }
            return url;
        }
    }
}