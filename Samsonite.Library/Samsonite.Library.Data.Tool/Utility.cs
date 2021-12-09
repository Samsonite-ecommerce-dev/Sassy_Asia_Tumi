using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Samsonite.Library.Data.Tool
{
    public class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="isNullAble"></param>
        /// <returns></returns>
        public static string ConvertTypeName(string typeName, bool isNullAble, string comment = null)
        {
            string _result = string.Empty;
            if (!isNullAble)
            {
                switch (typeName)
                {
                    case "bit": _result = "bool"; break;
                    case "tinyint": _result = "byte"; break;
                    case "smallint": _result = "short"; break;
                    case "int": _result = "int"; break;
                    case "bigint": _result = "long"; break;
                    case "smallmoney": _result = "decimal"; break;
                    case "money": _result = "decimal"; break;
                    case "numeric": _result = "decimal"; break;
                    case "decimal": _result = "decimal"; break;
                    case "float": _result = "double"; break;
                    case "real": _result = "Single"; break;
                    case "smalldatetime": _result = "DateTime"; break;
                    case "datetime": _result = "DateTime"; break;
                    case "datetime2": _result = "DateTime"; break;
                    case "date": _result = "DateTime"; break;
                    case "time": _result = "DateTime"; break;
                    case "timestamp": _result = "DateTime"; break;
                    case "char": _result = "string "; break;
                    case "text": _result = "string"; break;
                    case "varchar": _result = "string"; break;
                    case "nchar": _result = "string"; break;
                    case "ntext": _result = "string"; break;
                    case "nvarchar": _result = "string"; break;
                    case "binary": _result = "byte[]"; break;
                    case "varbinary": _result = "byte[]"; break;
                    case "image": _result = "byte[]"; break;
                    case "uniqueidentifier": _result = "Guid"; break;
                    case "variant": _result = "object"; break;
                }
            }
            else
            {
                switch (typeName)
                {
                    case "bit": _result = "bool?"; break;
                    case "tinyint": _result = "byte?"; break;
                    case "smallint": _result = "short?"; break;
                    case "int": _result = "int?"; break;
                    case "bigint": _result = "long?"; break;
                    case "smallmoney": _result = "decimal?"; break;
                    case "money": _result = "decimal?"; break;
                    case "numeric": _result = "decimal?"; break;
                    case "decimal": _result = "decimal?"; break;
                    case "float": _result = "double?"; break;
                    case "real": _result = "Single?"; break;
                    case "smalldatetime": _result = "DateTime?"; break;
                    case "datetime": _result = "DateTime?"; break;
                    case "datetime2": _result = "DateTime?"; break;
                    case "date": _result = "DateTime?"; break;
                    case "time": _result = "DateTime?"; break;
                    case "timestamp": _result = "DateTime?"; break;
                    case "char": _result = "string "; break;
                    case "text": _result = "string"; break;
                    case "varchar": _result = "string"; break;
                    case "nchar": _result = "string"; break;
                    case "ntext": _result = "string"; break;
                    case "nvarchar": _result = "string"; break;
                    case "binary": _result = "byte[]"; break;
                    case "varbinary": _result = "byte[]"; break;
                    case "image": _result = "byte[]"; break;
                    case "uniqueidentifier": _result = "Guid"; break;
                    case "variant": _result = "object"; break;
                }
            }

            _result = CheckEnumType(_result, comment);

            return _result;
        }
        public static string CheckEnumType(string typeName, string comment)
        {
            if (!string.IsNullOrEmpty(comment)
                && comment.Contains("enum:")
                && "int,short,long".Split(',').Any(s => typeName.ToLower().StartsWith(s)))
            {
                var enumTypeName = comment.Split("enum:").Last().Trim();
                if (typeName.Contains("?"))
                {
                    enumTypeName += "?";
                }
                return enumTypeName;
            }
            return typeName;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="objData">文件数据</param>
        /// <param name="objFilePath">文件保存全路径</param>
        public static void SaveFile(byte[] objData, string objFilePath)
        {
            try
            {
                using (FileStream fs = new FileStream(objFilePath, FileMode.Create))
                {
                    //开始写入
                    fs.Write(objData, 0, objData.Length);
                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="objFileText">文件数据</param>
        /// <param name="objFilePath">文件保存全路径</param>
        public static void SaveFile(string objFileText, string objFilePath)
        {
            try
            {
                using (FileStream fs = new FileStream(objFilePath, FileMode.Create))
                {
                    //获得字节数组
                    byte[] data = new UTF8Encoding().GetBytes(objFileText);
                    //开始写入
                    fs.Write(data, 0, data.Length);
                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
