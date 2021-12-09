using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Samsonite.Library.Utility
{
    public class VariableHelper
    {
        /// <summary>
        /// 过滤SByte型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>SByte</returns>
        public static SByte SaferequestSByte(object objValue)
        {
            SByte result;
            return SByte.TryParse(Convert.ToString(objValue), out result) ? result : (sbyte)0;
        }

        /// <summary>
        /// 过滤byte型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>int</returns>
        public static Byte SaferequestByte(object objValue)
        {
            Byte result = new Byte();
            return Byte.TryParse(Convert.ToString(objValue), out result) ? result : (byte)0;
        }

        /// <summary>
        /// 过滤Int16型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>Int16</returns>
        public static Int16 SaferequestInt16(object objValue)
        {
            Int16 result = new Int16();
            return Int16.TryParse(Convert.ToString(objValue), out result) ? result : (Int16)0;
        }

        /// <summary>
        /// 过滤Int32型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>Int32</returns>
        public static Int32 SaferequestInt(object objValue)
        {
            Int32 result = new Int32();
            return Int32.TryParse(Convert.ToString(objValue), out result) ? result : 0;
        }

        /// <summary>
        /// 过滤Init64型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>Int64</returns>
        public static Int64 SaferequestInt64(object objValue)
        {
            Int64 result = new Int64();
            return Int64.TryParse(Convert.ToString(objValue), out result) ? result : 0;
        }

        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>string</returns>
        public static string SaferequestStr(object objValue)
        {
            string _result = string.Empty;
            if (objValue != null)
            {
                _result = objValue.ToString();
                if (string.IsNullOrEmpty(_result))
                {
                    objValue = string.Empty;
                }
                else
                {
                    _result = _result.Trim();
                    //过滤sql注入
                    _result = SaferequestSQL(_result);
                    //过滤脚本
                    _result = SaferequestScript(_result);
                }
            }
            return _result;
        }

        /// <summary>
        /// 过滤字符串集合
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string[] SaferequestStrArray(object objValue)
        {
            string[] result = null;
            if (objValue != null)
            {
                if (!string.IsNullOrEmpty(objValue.ToString()))
                {
                    if (!string.IsNullOrEmpty(objValue.ToString()))
                    {
                        List<string> list = new List<string>();
                        string[] array = objValue.ToString().Split(',');
                        foreach (string str in array)
                        {
                            list.Add(str);
                        }
                        result = list.ToArray();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 过滤搜索字符
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <param name="objLength">截取长度</param>
        /// <returns>string</returns>
        public static string SaferequestKeywordStr(string objValue, int objLength)
        {
            if (string.IsNullOrEmpty(objValue))
            {
                objValue = string.Empty;
            }
            else
            {
                objValue = objValue.Trim();
                if (objValue.Length > objLength) objValue = objValue.Substring(0, objLength);
                //过滤sql注入
                objValue = SaferequestSQL(objValue);
                //过滤脚本
                objValue = SaferequestScript(objValue);
            }

            return objValue;
        }

        /// <summary>
        /// 过滤编辑器字符
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>string</returns>
        public static string SaferequestEditor(string objValue)
        {
            if (string.IsNullOrEmpty(objValue))
            {
                objValue = string.Empty;
            }
            else
            {
                //过滤sql注入
                objValue = SaferequestSQL(objValue);
                //过滤脚本
                objValue = SaferequestScript(objValue);
            }

            return objValue;
        }

        /// <summary>
        /// 过滤float型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>float</returns>
        public static float SaferequestFloat(object objValue)
        {
            float result = new float();
            return float.TryParse(Convert.ToString(objValue), out result) ? result : 0;
        }

        /// <summary>
        /// 过滤decimal型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>decimal</returns>
        public static Decimal SaferequestDecimal(object objValue)
        {
            Decimal result = new Decimal();
            return Decimal.TryParse(Convert.ToString(objValue), out result) ? result : 0;
        }

        /// <summary>
        /// 过滤decimal型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>decimal</returns>
        public static Decimal? SaferequestNullDecimal(object objValue)
        {
            Decimal result = new Decimal();
            Decimal? _default = null;
            if (objValue != null)
            {
                if (objValue.ToString() == "")
                {
                    return _default;
                }
                else
                {
                    return Decimal.TryParse(objValue.ToString(), out result) ? result : _default;
                }
            }
            else
            {
                return _default;
            }
        }

        /// <summary>
        /// 过滤double型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>double</returns>
        public static Double SaferequestDouble(object objValue)
        {
            Double result = new Double();
            return Double.TryParse(Convert.ToString(objValue), out result) ? result : 0;
        }

        /// <summary>
        /// 过滤布尔型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>bool</returns>
        public static Boolean SaferequestBool(object objValue)
        {
            Boolean result = new Boolean();
            return Boolean.TryParse(Convert.ToString(objValue), out result) ? result : false;
        }


        /// <summary>
        /// 过滤布尔型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>int</returns>
        public static int SaferequestBoolToInt(bool objValue)
        {
            return (SaferequestBool(objValue)) ? 1 : 0;
        }

        /// <summary>
        /// 过滤布尔型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>int</returns>
        public static bool SaferequestIntToBool(object objValue)
        {
            return (SaferequestInt(objValue) == 1);
        }

        /// <summary>
        /// 过滤时间类型1753-01-01 00:00:00 
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>dateTime</returns>
        public static DateTime SaferequestTime(object objValue)
        {
            DateTime result = new DateTime();
            DateTime _default = DateTime.Now;
            return (objValue == null) ? _default : DateTime.TryParse(objValue.ToString(), out result) ? result : _default;
        }

        /// <summary>
        /// 过滤时间类型
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>dateTime</returns>
        public static DateTime? SaferequestNullTime(object objValue)
        {
            DateTime result = new DateTime();
            DateTime? _default = null;
            if (objValue != null)
            {
                if (objValue.ToString() == "0001-01-01 00:00:00")
                {
                    return _default;
                }
                else
                {
                    return DateTime.TryParse(objValue.ToString(), out result) ? result : _default;
                }
            }
            else
            {
                return _default;
            }
        }

        /// <summary>
        /// 过滤时间类型0001-01-01 00:00:00 
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>dateTime2</returns>
        public static DateTime SaferequestTime2(object objValue)
        {
            DateTime result = new DateTime();
            DateTime _default = Convert.ToDateTime("1753-01-01 00:00:00");
            return (objValue == null) ? _default : DateTime.TryParse(objValue.ToString(), out result) ? result : _default;
        }

        /// <summary>
        /// 过滤int集合
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static int[] SaferequestIntArray(object objValue)
        {
            int[] result = null;
            if (objValue != null)
            {
                if (!string.IsNullOrEmpty(objValue.ToString()))
                {
                    List<int> list = new List<int>();
                    string[] array = objValue.ToString().Split(',');
                    foreach (string str in array)
                    {
                        list.Add(VariableHelper.SaferequestInt(str));
                    }
                    result = list.ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// 过滤Int64集合
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static long[] SaferequestInt64Array(object objValue)
        {
            Int64[] result = null;
            if (objValue != null)
            {
                if (!string.IsNullOrEmpty(objValue.ToString()))
                {
                    if (!string.IsNullOrEmpty(objValue.ToString()))
                    {
                        List<Int64> list = new List<Int64>();
                        string[] array = objValue.ToString().Split(',');
                        foreach (string str in array)
                        {
                            list.Add(VariableHelper.SaferequestInt64(str));
                        }
                        result = list.ToArray();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 生成伦敦UTC标准时间
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string SaferequestUTCTime(DateTime objValue)
        {
            return string.Format("{0}.000Z", objValue.ToUniversalTime().ToString("s"));
        }

        /// <summary>
        /// 从201510131601字符串转换成 Datetime
        /// </summary>
        /// <param name="objDate"></param>
        /// <returns></returns>
        public static DateTime ParseDate(string objDate)
        {
            try
            {
                int year = int.Parse(objDate.Substring(0, 4));
                int month = int.Parse(objDate.Substring(4, 2));
                int day = int.Parse(objDate.Substring(6, 2));

                int hour = 0;
                int min = 0;
                if (objDate.Length > 8)
                {
                    hour = int.Parse(objDate.Substring(8, 2));
                }
                if (objDate.Length > 10)
                {
                    min = int.Parse(objDate.Substring(10, 2));
                }
                return new DateTime(year, month, day, hour, min, 0, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 翻页页码(即整数必须大于1)
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>int</returns>
        public static int SaferequestPage(object objValue)
        {
            int result = SaferequestInt(objValue);
            if (result < 1) result = 1;
            return result;
        }

        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>int</returns>
        public static int SaferequestPositiveInt(object objValue)
        {
            int result = SaferequestInt(objValue);
            if (result < 0) result = 0;
            return result;
        }

        /// <summary>
        /// 数据库主键ID
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>int</returns>
        public static int SaferequestIdentify(object objValue)
        {
            int result = SaferequestInt(objValue);
            if (result < 1) result = 1;
            return result;
        }

        /// <summary>
        /// 过滤null
        /// </summary>
        /// <param name="objValue">参数</param>
        /// <returns>string</returns>
        public static string SaferequestNull(object objValue)
        {
            return (objValue == null) ? string.Empty : Convert.ToString(objValue);
        }

        /// <summary>
        /// 过滤SQL注入
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string SaferequestSQL(string objValue)
        {
            //过滤'-- 
            objValue = Regex.Replace(objValue, @"(')|(--)", "", RegexOptions.IgnoreCase);
            return objValue;
        }

        /// <summary>
        /// 过滤脚本
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string SaferequestScript(string objValue)
        {
            objValue = Regex.Replace(objValue, @"<[\s]*?script[\s\S]+<[\s]*?/script[\s]*?>", "", RegexOptions.IgnoreCase);
            objValue = Regex.Replace(objValue, @" href[ ^=]*=\s*(['""\s]?)[\w]*script+?:([/s/S]*[^\1]*?)\1[\s]*", "", RegexOptions.IgnoreCase);
            objValue = Regex.Replace(objValue, @"javascript[\s]*?:[\S]*?\(\w*?\)[\;]*?", "", RegexOptions.IgnoreCase);
            objValue = Regex.Replace(objValue, @"on\w+=\s*(['""\s]?)([/s/S]*[^\1]*?)\1[\s]*", "", RegexOptions.IgnoreCase);
            return objValue;
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="str">参数名称</param>
        /// <returns>string</returns>
        public static bool IsNumeric(string str)
        {
            Regex reg = new Regex(@"^[0-9]+$");
            if (reg.Match(str).Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 计算字符串实际长度，中文大写字母算2个字符
        /// </summary>
        /// <param name="objStr">参数名称</param>
        /// <returns>int</returns>
        public static int StrLength(string objStr)
        {
            Regex _regex = new Regex("[\u4e00-\u9fa5]|[A-Z]", RegexOptions.Compiled);
            char[] strChar = objStr.ToCharArray();
            int nLength = 0;
            for (int i = 0; i < strChar.Length; i++)
            {
                if (_regex.IsMatch((strChar[i]).ToString()))
                {
                    nLength += 2;
                }
                else
                {
                    nLength = nLength + 1;
                }
            }
            return nLength;
        }

        /// <summary>
        /// 计算字符串实际长度,泰文算2个字符
        /// </summary>
        /// <param name="objStr">参数名称</param>
        /// <returns>int</returns>
        public static int ThaiStrLength(string objStr)
        {
            Regex _regex = new Regex("[\u0e00-\u0e7f]", RegexOptions.Compiled);
            char[] strChar = objStr.ToCharArray();
            int nLength = 0;
            for (int i = 0; i < strChar.Length; i++)
            {
                if (_regex.IsMatch((strChar[i]).ToString()))
                {
                    nLength += 2;
                }
                else
                {
                    nLength = nLength + 1;
                }
            }
            return nLength;
        }

        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="objStr">参数名称</param>
        /// <param name="objLength">截取的长度</param>
        /// <returns>string</returns>
        public static string StrSubstring(string objStr, int objLength)
        {
            string _result = string.Empty;
            Regex _regex = new Regex("[\u4e00-\u9fa5]|[A-Z]", RegexOptions.Compiled);
            char[] strChar = objStr.ToCharArray();
            int nLength = 0;
            for (int i = 0; i < strChar.Length; i++)
            {
                if (_regex.IsMatch((strChar[i]).ToString()))
                {
                    nLength += 2;
                }
                else
                {
                    nLength = nLength + 1;
                }

                if (nLength <= objLength)
                {
                    _result += strChar[i];
                }
                else
                {
                    break;
                }
            }
            return _result;
        }

        /// <summary>
        /// 截取指定长度字符串,同时在后面加特殊符号
        /// </summary>
        /// <param name="objStr">参数名称</param>
        /// <param name="objLength">截取的长度</param>
        /// <param name="objSign">截取标志</param>
        /// <returns>string</returns>
        public static string StrSubstringSign(string objStr, int objLength, string objSign = "")
        {
            string _result = string.Empty;
            if (!string.IsNullOrEmpty(objStr))
            {
                if (string.IsNullOrEmpty(objSign)) objSign = "..";
                if (StrLength(objStr) >= objLength)
                {
                    _result = StrSubstring(objStr, objLength) + objSign;
                }
                else
                {
                    _result = objStr;
                }
            }
            return _result;
        }

        /// <summary>
        /// 清除特殊字符
        /// </summary>
        /// <param name="objStr">参数名称</param>
        /// <returns>string</returns>
        public static string InputText(string objStr)
        {
            string _result = objStr.Trim();
            if (string.IsNullOrEmpty(_result))
                return string.Empty;
            _result = Regex.Replace(_result, "[\\s]{2,}", " "); //两个或者更多的空格
            _result = Regex.Replace(_result, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n"); //<br>
            _result = Regex.Replace(_result, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " "); //&nbsp;
            _result = Regex.Replace(_result, "<(.|\\n)*?>", string.Empty); //其它一些tag
            _result = _result.Replace("'", "''");
            return _result;
        }

        /// <summary>
        /// Html脚本转化
        /// </summary>
        /// <param name="objHtml">参数名称</param>
        /// <returns>string</returns>
        public static string HtmlDecode(string objHtml)
        {
            string _result = objHtml;
            if (!string.IsNullOrEmpty(_result))
            {
                _result = _result.Replace(">", "&gt");
                _result = _result.Replace("<", "&lt;");
                _result = _result.Replace(Convert.ToChar(9).ToString(), "&nbsp;");
                _result = _result.Replace(Convert.ToChar(32).ToString(), "&nbsp;");
                _result = _result.Replace(Convert.ToChar(34).ToString(), "&quot;");
                _result = _result.Replace(Convert.ToChar(39).ToString(), "&#39;");
                _result = _result.Replace(Convert.ToChar(13).ToString(), "");
                _result = _result.Replace(Convert.ToChar(10).ToString(), "<br/>");
            }
            return _result;
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="objSize">参数名称</param>
        /// <returns>string</returns>
        public static string FormatSize(long objSize)
        {
            int t = 0;
            double _size = objSize;
            string _unit = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                _size = _size / 1024;
                if (_size < 1)
                {
                    break;
                }
                else
                {
                    t = t + 1;
                }
            }

            switch (t)
            {
                case 0:
                    _unit = "B";
                    break;
                case 1:
                    _unit = "KB";
                    break;
                case 2:
                    _unit = "MB";
                    break;
                case 3:
                    _unit = "GB";
                    break;
                case 4:
                    _unit = "TB";
                    break;
            }

            return Convert.ToString(Math.Round(_size * 1024, 2)) + _unit;
        }

        /// <summary>
        /// 生成固定位数的随机字符串
        /// </summary>
        /// <param name="objLength"></param>
        /// <returns></returns>
        public static string CreateRandStr(int objLength)
        {
            string _result = string.Empty;
            Random rd = new Random();
            for (int t = 0; t < objLength; t++)
            {
                _result += rd.Next(0, 10);
            }
            return _result;
        }

        /**/
        /// <summary> 
        /// 小数的四舍五入算法 
        /// </summary> 
        /// <param name="objDigital">要进行处理的数据</param> 
        /// <param name="objLen">保留的小数位数</param> 
        /// <returns>decimal</returns> 
        public static decimal DigitalRound(decimal objDigital, int objLen)
        {
            decimal _result = 0;
            bool isNegative = false;
            //如果是负数 
            if (objDigital < 0)
            {
                isNegative = true;
                objDigital = -objDigital;
            }

            int _Rate = 1;
            for (int i = 1; i <= objLen; i++)
            {
                _Rate = _Rate * 10;
            }
            _result = Math.Round(objDigital * _Rate + (decimal)0.001, 0);
            _result = _result / _Rate;

            if (isNegative)
            {
                _result = -_result;
            }

            return _result;
        }

        /**/
        /// <summary> 
        /// 处理金额显示格式，如果没有分角，咋不显示小数点后2位
        /// </summary> 
        /// <param name="objMoney">金额</param>
        /// <returns>decimal</returns> 
        public static string FormateMoney(decimal objMoney)
        {
            string _result = string.Empty;
            if ((long)objMoney == objMoney)
            {
                _result = objMoney.ToString("N0");
            }
            else if ((long)(objMoney * 10) == objMoney * 10)
            {
                _result = objMoney.ToString("N01");
            }
            else
            {
                _result = objMoney.ToString("N02");
            }

            return _result;
        }

        /// <summary>
        /// 处理时间
        /// </summary>
        /// <param name="objTime"></param>
        /// <param name="objFormat"></param>
        /// <returns></returns>
        public static string FormateTime(DateTime? objTime, string objFormat)
        {
            string _result = string.Empty;
            if (objTime != null)
            {
                try
                {
                    _result = Convert.ToDateTime(objTime).ToString(objFormat);
                }
                catch
                {

                }
            }

            return _result;
        }

        /// <summary>
        /// 格式化Json
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }
    }
}
