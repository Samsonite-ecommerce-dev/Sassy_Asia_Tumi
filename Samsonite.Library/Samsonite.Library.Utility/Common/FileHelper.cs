using System;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace Samsonite.Library.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        private static string _direPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 创建文件目录
        /// </summary>
        /// <param name="objPath"></param>
        public static void CreateDirectory(string objPath)
        {
            try
            {
                string _path = _direPath + "/" + objPath;
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 读取文件夹下面的所有文件(返回文件绝对路径)
        /// </summary>
        /// <param name="objPath"></param>
        /// <param name="objFiles"></param>
        /// <returns></returns>
        public static List<string> ReadFiles(string objPath, List<string> objFiles)
        {
            try
            {
                List<string> _result = objFiles;
                DirectoryInfo _dir = new DirectoryInfo(objPath);
                //读取目录
                foreach (var _d in _dir.GetDirectories())
                {
                    _result = ReadFiles(_d.FullName, objFiles);
                }
                //读取文件
                foreach (var _f in _dir.GetFiles())
                {
                    if (!_result.Contains(_f.FullName))
                    {
                        _result.Add(_f.FullName);
                    }
                }
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        /// <summary>
        /// 从全路径中获取文件名称
        /// </summary>
        /// <param name="objFilePath"></param>
        /// <returns></returns>
        public static string GetFileName(string objFilePath)
        {
            try
            {
                string _result = string.Empty;
                if (!string.IsNullOrEmpty(objFilePath))
                {
                    string[] _array = objFilePath.Split('/');
                    _result = _array[_array.Length - 1];
                }
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
