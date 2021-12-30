using Microsoft.AspNetCore.Http;
using Samsonite.Library.Utility;
using System;
using System.Text.Json;

namespace Samsonite.Library.Web.Core
{
    public class AntiforgeryService : IAntiforgeryService
    {
        private IBaseService _baseService;
        private IHttpContextAccessor _httpContextAccessor;
        private const string _secretKey = "sat!#B8t*1Ji^Z23";
        private const string _feildName = "__RequestVerificationToken";
        private const string _headerType = "Antiforgery";
        private string _headerAlg = AntiforgeryTokenAlg.HS256.ToString();
        private string _cookieName = string.Empty;
        public AntiforgeryService(IBaseService baseService, IHttpContextAccessor httpContextAccessor)
        {
            _baseService = baseService;
            _httpContextAccessor = httpContextAccessor;
            _cookieName = $"{baseService.CurrentApplicationConfig.GlobalConfig.CookieKey}.Antiforgery";
        }

        /// <summary>
        /// 创建Token值
        /// </summary>
        /// <returns></returns>
        public string AntiForgeryTokenValue()
        {
            var _cookieToken = GetCookieToken();
            //如果不存在或者已经失效,则重新创建
            if (_cookieToken == null)
            {
                _cookieToken = GenerateTokens();
            }
            //创建requestToken
            var _currentLoginUser = _baseService.CurrentLoginUser;
            var _requestToken = new AntiforgeryTokenModel()
            {
                HeaderInfo = new AntiforgeryTokenModel.Header()
                {
                    Alg = _headerAlg,
                    Type = _headerType,
                },
                SecurityInfo = new AntiforgeryTokenModel.Security()
                {
                    SecurityToken = _cookieToken.SecurityInfo.SecurityToken,
                    IsCookieToken = false,
                },
                PayloadInfo = new AntiforgeryTokenModel.Payload()
                {
                    UserName = (_currentLoginUser != null) ? _currentLoginUser.UserName : "",
                    Timestamp = TimeHelper.DateTimeToUnixTimestamp(DateTime.Now)
                }
            };
            _requestToken.Signature = this.GenerateSignature(_requestToken);
            //返回隐藏域代码
            return this.Serialize(_requestToken);
        }

        /// <summary>
        /// 创建隐藏域
        /// </summary>
        /// <returns></returns>
        public string AntiForgeryToken()
        {
            string _requestToken = this.AntiForgeryTokenValue();
            //返回隐藏域代码
            return $"<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"{_requestToken}\" />";
        }

        /// <summary>
        /// 验证Token是否匹配
        /// </summary>
        public bool ValidateRequest()
        {
            try
            {
                var _cookieToken = this.GetCookieToken();
                var _requestToken = this.GetRequestToken();

                //验证签名
                if (!string.Equals(_requestToken.Signature, this.GenerateSignature(_requestToken)))
                {
                    return false;
                }

                if (_cookieToken == null || _requestToken == null)
                {
                    return false;
                }

                if (!_cookieToken.SecurityInfo.IsCookieToken || _requestToken.SecurityInfo.IsCookieToken)
                {
                    return false;
                }

                if (!string.Equals(_cookieToken.SecurityInfo.SecurityToken, _requestToken.SecurityInfo.SecurityToken))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(_requestToken.PayloadInfo.UserName))
                {
                    return false;
                }

                var _currentLoginUser = _baseService.CurrentLoginUser;
                string _userName = (_currentLoginUser != null) ? _currentLoginUser.UserName : "";
                if (!string.Equals(_userName, _requestToken.PayloadInfo.UserName))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建令牌
        /// </summary>
        /// <returns></returns>
        private string GenerateSecurityToken()
        {
            int _length = 16;
            string _str = string.Empty;
            string[] _array = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Random rnd = new Random();
            for (int i = 0; i < _length; i++)
            {
                rnd = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")) + i);
                _str += _array[rnd.Next(0, 61)];
            }
            return EncryptHelper.Md5_32(_str);
        }

        /// <summary>
        /// 生成Cookie
        /// </summary>
        /// <returns></returns>
        private AntiforgeryTokenModel GenerateTokens()
        {
            var _currentLoginUser = _baseService.CurrentLoginUser;
            var _newCookieToken = new AntiforgeryTokenModel()
            {
                HeaderInfo = new AntiforgeryTokenModel.Header()
                {
                    Alg = _headerAlg,
                    Type = _headerType,
                },
                SecurityInfo = new AntiforgeryTokenModel.Security()
                {
                    //创建令牌
                    SecurityToken = this.GenerateSecurityToken(),
                    IsCookieToken = true,
                },
                PayloadInfo = new AntiforgeryTokenModel.Payload()
                {
                    UserName = (_currentLoginUser != null) ? _currentLoginUser.UserName : "",
                    Timestamp = TimeHelper.DateTimeToUnixTimestamp(DateTime.Now)
                }
            };
            //重置cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(_cookieName);
            //不设置过期时间,默认关闭浏览器时过期
            _httpContextAccessor.HttpContext.Response.Cookies.Append(_cookieName, this.Serialize(_newCookieToken), new CookieOptions() { HttpOnly = true });
            return _newCookieToken;
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GenerateSignature(AntiforgeryTokenModel token)
        {
            if (token.HeaderInfo.Alg == AntiforgeryTokenAlg.HS1.ToString())
            {
                return EncryptHelper.HMAC_SHA1($"{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.HeaderInfo))},{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.SecurityInfo))},{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.PayloadInfo))}", _secretKey);
            }
            else if (token.HeaderInfo.Alg == AntiforgeryTokenAlg.HS512.ToString())
            {
                return EncryptHelper.HMAC_SHA512($"{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.HeaderInfo))},{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.SecurityInfo))},{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.PayloadInfo))}", _secretKey);
            }
            else
            {
                return EncryptHelper.HMAC_SHA256($"{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.HeaderInfo))},{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.SecurityInfo))},{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.PayloadInfo))}", _secretKey);
            }
        }

        /// <summary>
        /// 序列化Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string Serialize(AntiforgeryTokenModel token)
        {
            string _base64Token = $"{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.HeaderInfo))}.{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.SecurityInfo))}.{EncryptHelper.EncodeBase64(JsonSerializer.Serialize(token.PayloadInfo))}";
            if (!string.IsNullOrEmpty(token.Signature))
            {
                _base64Token += $".{token.Signature}";
            }
            return AESEncryption.Encrypt(_base64Token);
        }

        /// <summary>
        /// 反序列化Token
        /// </summary>
        /// <param name="serializedToken"></param>
        /// <returns></returns>
        private AntiforgeryTokenModel Deserialize(string serializedToken)
        {
            AntiforgeryTokenModel _result = new AntiforgeryTokenModel();
            try
            {
                string _token = AESEncryption.Decrypt(serializedToken);
                string[] _arrayToken = _token.Split(".");
                _result.HeaderInfo = JsonSerializer.Deserialize<AntiforgeryTokenModel.Header>(EncryptHelper.DecodeBase64(_arrayToken[0]));
                _result.SecurityInfo = JsonSerializer.Deserialize<AntiforgeryTokenModel.Security>(EncryptHelper.DecodeBase64(_arrayToken[1]));
                _result.PayloadInfo = JsonSerializer.Deserialize<AntiforgeryTokenModel.Payload>(EncryptHelper.DecodeBase64(_arrayToken[2]));
                if (_arrayToken.Length >= 4)
                {
                    _result.Signature = _arrayToken[3];
                }
            }
            catch
            {
                _result = null;
            }
            return _result;
        }

        /// <summary>
        /// 获取Cookie的信息
        /// </summary>
        /// <returns></returns>
        private AntiforgeryTokenModel GetCookieToken()
        {
            AntiforgeryTokenModel _result = new AntiforgeryTokenModel();
            string _cookieToken = string.Empty;
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(_cookieName, out _cookieToken);
            if (!string.IsNullOrEmpty(_cookieToken))
            {
                _result = this.Deserialize(_cookieToken);
            }
            else
            {
                _result = null;
            }
            return _result;
        }

        /// <summary>
        /// 获取请求的信息
        /// </summary>
        /// <returns></returns>
        private AntiforgeryTokenModel GetRequestToken()
        {
            AntiforgeryTokenModel _result = new AntiforgeryTokenModel();
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(_feildName))
            {
                string _requestToken = _httpContextAccessor.HttpContext.Request.Headers[_feildName].ToString();
                _result = this.Deserialize(_requestToken);
            }
            else
            {
                _result = null;
            }
            return _result;
        }
    }

    public class AntiforgeryTokenModel
    {
        public Header HeaderInfo { get; set; }

        public Security SecurityInfo { get; set; }

        public Payload PayloadInfo { get; set; }

        public string Signature { get; set; }

        public class Header
        {
            public string Alg { get; set; }

            public string Type { get; set; }
        }

        public class Security
        {
            public string SecurityToken { get; set; }

            public bool IsCookieToken { get; set; }
        }

        public class Payload
        {
            public string UserName { get; set; }

            public long Timestamp { get; set; }
        }
    }

    public enum AntiforgeryTokenAlg
    {
        HS1 = 1,
        HS256 = 2,
        HS512 = 3
    }
}
