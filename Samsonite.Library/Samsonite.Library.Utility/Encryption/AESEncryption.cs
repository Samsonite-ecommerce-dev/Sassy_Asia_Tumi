using System;
using System.Text;
using System.Security.Cryptography;

namespace Samsonite.Library.Utility
{
    /// <summary>
    /// AES 加密算法
    /// </summary>
    public class AESEncryption
    {
        private static string encryptKey = "6Y!KI*&^#,i#$@x1%)5^6(7*9uIo/Ee1";

        //默认密钥向量
        private static readonly byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        /// <summary>
        /// AES 加密 非ECB模式，需要密钥向量
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encrypt(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
                return string.Empty;
            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Mode = CipherMode.CBC;
            rijndaelProvider.Padding = PaddingMode.PKCS7;
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 解密 非ECB模式，需要密钥向量
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string Decrypt(string decryptString)
        {
            if (string.IsNullOrEmpty(decryptString))
                return string.Empty;
            try
            {
                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Mode = CipherMode.CBC;
                rijndaelProvider.Padding = PaddingMode.PKCS7;
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// AES-128-CBC  需要偏移向量
        /// </summary>
        /// <param name="encryptString">明文</param>
        /// <param name="key">加密密钥</param>
        /// <param name="ivParameter">偏移向量</param>
        /// <returns></returns>
        public static string CbcEncrypt(string encryptString, string key, string ivParameter = "")
        {
            var ivKeys = Keys;
            if (!string.IsNullOrEmpty(ivParameter))
            {
                ivKeys = Encoding.UTF8.GetBytes(ivParameter.Substring(0, 16));
            }
            if (string.IsNullOrEmpty(encryptString))
                return encryptString;
            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
            rijndaelProvider.IV = ivKeys;
            rijndaelProvider.Padding = PaddingMode.PKCS7;
            rijndaelProvider.Mode = CipherMode.CBC;
            rijndaelProvider.BlockSize = 128;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 解密 非ECB模式,需要密钥向量
        /// </summary>
        /// <param name="decryptString">明文</param>
        /// <param name="key">加密密钥</param>
        /// <param name="ivParameter"></param>
        /// <returns></returns>
        public static string CbcDecrypt(string decryptString, string key, string ivParameter = "")
        {
            var ivKeys = Keys;
            if (!string.IsNullOrEmpty(ivParameter))
            {
                ivKeys = Encoding.UTF8.GetBytes(ivParameter.Substring(0, 16));
            }
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(decryptString))
                return decryptString;
            try
            {
                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
                rijndaelProvider.IV = ivKeys;
                rijndaelProvider.Padding = PaddingMode.PKCS7;
                rijndaelProvider.Mode = CipherMode.CBC;
                rijndaelProvider.BlockSize = 128;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();
                byte[] inputData = Convert.FromBase64String(decryptString);
                //var inputData = Encoding.UTF8.GetBytes(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return decryptString;
            }
        }

        /// <summary>
        ///  AES 加密  ECB模式
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            var rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES 解密  ECB模式
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
