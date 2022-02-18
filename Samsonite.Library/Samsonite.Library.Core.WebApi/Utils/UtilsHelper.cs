﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Samsonite.Library.Core.WebApi.Utils
{
    public class UtilsHelper
    {
        #region 页码验证
        /// <summary>
        /// 验证页码数
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int ValidatePageSize(int pageSize)
        {
            if (pageSize > GlobalConfig.MaxPageSize) pageSize = GlobalConfig.MaxPageSize;
            if (pageSize <= 0) pageSize = GlobalConfig.DefaultPageSize;
            return pageSize;
        }

        /// <summary>
        /// 验证当前页码
        /// </summary>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public int ValidateCurrentPage(int currentPage)
        {
            if (currentPage < 1) currentPage = 1;
            return currentPage;
        }
        #endregion

        #region 时间格式转换
        /// <summary>
        /// 从 201510131601 字符串转换成 Datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime parseDate(string date)
        {
            try
            {
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(4, 2));
                int day = int.Parse(date.Substring(6, 2));

                int hour = 0;
                int min = 0;
                int second = 0;
                if (date.Length > 8)
                {
                    hour = int.Parse(date.Substring(8, 2));
                }
                if (date.Length > 10)
                {
                    min = int.Parse(date.Substring(10, 2));
                }
                if (date.Length > 12)
                {
                    second = int.Parse(date.Substring(12, 2));
                }
                return new DateTime(year, month, day, hour, min, second, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 签名
        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secret"></param>
        /// <param name="encryptMethod"></param>
        /// <returns></returns>
        public string CreateSign(IDictionary<string, string> parameters, string secret, string encryptMethod)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            string _query = string.Empty;
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
            if (GlobalConfig.SIGN_METHOD_SHA256.Equals(encryptMethod))
            {
                HMACSHA256 _provider = new HMACSHA256();
                _provider.Key = Encoding.UTF8.GetBytes(secret);
                bytes = _provider.ComputeHash(Encoding.UTF8.GetBytes(_query));
            }
            else
            {
                HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
                bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(_query));
            }

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }
        #endregion
    }
}
