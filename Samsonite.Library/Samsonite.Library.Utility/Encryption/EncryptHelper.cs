using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Samsonite.Library.Utility
{
    public class EncryptHelper
    {
        #region DES加密
        private static string _DES_Key = "omscore86";   //PassWord加密Key 
        /// <summary>
        /// DES加密帐号口令 
        /// </summary>
        /// <param name="objPassWord"></param>
        /// <returns>string</returns>
        public static string DESEncrypt(string objPassWord)
        {
            return DESEncryptMethod(objPassWord, _DES_Key);
        }
        /// <summary>
        /// DES加密帐号口令 
        /// </summary>
        /// <param name="objPassWord"></param>
        /// <param name="objPassWordKey"></param>
        /// <returns>string</returns>
        public static string DESEncrypt(string objPassWord, string objPassWordKey)
        {
            return DESEncryptMethod(objPassWord, objPassWordKey);
        }

        /// <summary>
        /// DES解密帐号口令 
        /// </summary>
        /// <param name="objPassWord"></param>
        /// <returns>string</returns>
        public static string DESDecrypt(string objPassWord)
        {
            return DESDecryptMethod(objPassWord, _DES_Key);
        }
        /// <summary>
        /// DES解密帐号口令 
        /// </summary>
        /// <param name="objPassWord"></param>
        /// <param name="objPassWordKey"></param>
        /// <returns>string</returns>
        public static string DESDecrypt(string objPassWord, string objPassWordKey)
        {
            return DESDecryptMethod(objPassWord, objPassWordKey);
        }

        /// <summary>
        /// DES加密过程 
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns>string</returns>
        private static string DESEncryptMethod(string pToEncrypt, string sKey)
        {
            //把字符串放到byte数组中
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //建立加密对象的密钥和偏移量
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5_32(sKey).ToUpper().Substring(0, 8));
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5_32(sKey).ToUpper().Substring(0, 8));
            //使得输入密码必须输入英文文本 
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    StringBuilder _result = new StringBuilder();
                    foreach (byte b in ms.ToArray())
                    {
                        _result.AppendFormat("{0:X2}", b);
                    }
                    return _result.ToString();
                }
            }
        }

        /// <summary>
        /// DES解密过程 
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns>string</returns>
        private static string DESDecryptMethod(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            //建立加密对象的密钥和偏移量，此值重要，不能修改   
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5_32(sKey).ToUpper().Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5_32(sKey).ToUpper().Substring(0, 8));
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return System.Text.Encoding.Default.GetString(ms.ToArray());
                }
            }
        }
        #endregion

        #region md5加密
        /// <summary>
        /// 16位Md5加密(取32位加密的9~25字符)
        /// </summary>
        /// <param name="objStr">字符串</param>
        /// <returns>string</returns>
        public static string Md5_16(string objStr)
        {
            return Md5_32(objStr).Substring(8, 16);
        }

        /// <summary>
        /// 32位Md5加密
        /// </summary>
        /// <param name="objStr">字符串</param>
        /// <returns>string</returns>
        public static string Md5_32(string objStr)
        {
            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
            {
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(objStr));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString().ToLower();
            }
        }
        #endregion

        #region base64加密
        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="objText"></param>
        /// <returns></returns>
        public static string EncodeBase64(string objText)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(objText);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Base64加密失败,错误信息:" + ex.Message);
            }
        }

        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="objText"></param>
        /// <returns></returns>
        public static string DecodeBase64(string objText)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(objText);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Base64解密失败,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region HMAC_SHA
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="objText"></param>
        /// <returns></returns>
        public static string SHA256(string objText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(objText);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="objText"></param>
        /// <param name="objSecret"></param>
        /// <returns></returns>
        public static string HMAC_SHA1(string objText, string objSecret)
        {
            HMACSHA1 _provider = new HMACSHA1();
            _provider.Key = Encoding.UTF8.GetBytes(objSecret);
            byte[] _hashBytes = _provider.ComputeHash(Encoding.UTF8.GetBytes(objText));
            StringBuilder _str = new StringBuilder();
            foreach (byte Byte in _hashBytes)
            {
                _str.AppendFormat("{0:x2}", Byte);
            }
            return _str.ToString();
        }

        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="objText"></param>
        /// <param name="objSecret"></param>
        /// <returns></returns>
        public static string HMAC_SHA256(string objText, string objSecret)
        {
            HMACSHA256 _provider = new HMACSHA256();
            _provider.Key = Encoding.UTF8.GetBytes(objSecret);
            byte[] _hashBytes = _provider.ComputeHash(Encoding.UTF8.GetBytes(objText));
            StringBuilder _str = new StringBuilder();
            foreach (byte Byte in _hashBytes)
            {
                _str.AppendFormat("{0:x2}", Byte);
            }
            return _str.ToString();
        }

        /// <summary>
        /// HMACSHA512加密
        /// </summary>
        /// <param name="objText"></param>
        /// <param name="objSecret"></param>
        /// <returns></returns>
        public static string HMAC_SHA512(string objText, string objSecret)
        {
            HMACSHA512 _provider = new HMACSHA512();
            _provider.Key = Encoding.UTF8.GetBytes(objSecret);
            byte[] _hashBytes = _provider.ComputeHash(Encoding.UTF8.GetBytes(objText));
            StringBuilder _str = new StringBuilder();
            foreach (byte Byte in _hashBytes)
            {
                _str.AppendFormat("{0:x2}", Byte);
            }
            return _str.ToString();
        }
        #endregion
    }
}
