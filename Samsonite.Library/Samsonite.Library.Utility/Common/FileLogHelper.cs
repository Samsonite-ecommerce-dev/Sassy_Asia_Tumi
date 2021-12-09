using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Samsonite.Library.Utility
{
    /// <summary>
    /// 文本日志
    /// </summary>
    public static class FileLogHelper
    {
        private static string direPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 检验文件目录信息
        /// </summary>
        /// <param name="objFolder"></param>
        /// <param name="objFilename"></param>
        /// <param name="isAPFolder">目录地址是否绝对路径</param>
        /// <returns></returns>
        private static string CheckFilePath(string objFolder, string objFilename,bool isAPFolder = false)
        {
            string currentDire = string.Empty;
            if (!string.IsNullOrEmpty(objFolder) && isAPFolder)
            {
                currentDire = objFolder;
            }
            else if(!string.IsNullOrEmpty(objFolder) && !isAPFolder )
            {
                currentDire = direPath + @"\Log\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + objFolder;
            }
            else 
            {
                currentDire = direPath + @"\Log\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyyMMdd");
            }
            //文件目录
            if (!Directory.Exists(currentDire))
            {
                Directory.CreateDirectory(currentDire);
            }

            string filePath = "";
            if (string.IsNullOrWhiteSpace(objFilename))
            {
                filePath = currentDire + @"\" + DateTime.Now.ToString("yyyyMMddHH") + ".log";
            }
            else
            {
                filePath = currentDire + @"\" + objFilename + ".log";
            }
            return filePath;
        }


        /// <summary>
        /// 写入错误
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="objFilename"></param>
        public static void WriteError(Exception ex, string objFilename = "")
        {
            StringBuilder error = new StringBuilder();
            error.AppendLine(ex.Message);
            if (ex.InnerException != null)
            {
                error.AppendLine(ex.InnerException.Message);
            }
            error.AppendLine(ex.StackTrace);
            WriteLogMethod(new[] { error.ToString() }, objFilename);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="objLog"></param>
        public static void WriteLog(string objLog)
        {
            WriteLogMethod(new[] { objLog });
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="objLog"></param>
        /// <param name="objFilename"></param>
        public static void WriteLog(string objLog, string objFilename)
        {
            WriteLogMethod(new[] { objLog }, objFilename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objLog"></param>
        /// <param name="objFilename"></param>
        /// <param name="objFolder"></param>
        public static void WriteLog(string objLog, string objFilename, string objFolder)
        {
            WriteLogMethod(new[] { objLog }, objFilename, objFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objLog"></param>
        /// <param name="objFilename"></param>
        /// <param name="objFolder"></param>
        /// <param name="isAPFolder"></param>
        public static void WriteLog(string objLog, string objFilename, string objFolder,bool isAPFolder)
        {
            WriteLogMethod(new[] { objLog }, objFilename, objFolder,isAPFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objLog"></param>
        /// <param name="objFilename"></param>
        /// <param name="objFolder"></param>
        public static void WriteLog(string[] objLog, string objFilename, string objFolder)
        {
            WriteLogMethod(objLog, objFilename, objFolder);
        }

        /// <summary>
        /// 写入日志方法
        /// </summary>
        /// <param name="objLog"></param>
        /// <param name="objFilename"></param>
        /// <param name="objFolder"></param>
        /// <param name="isApFolder"></param>
        private static void WriteLogMethod(IEnumerable<string> objLog, string objFilename = "", string objFolder = "",bool isApFolder=false)
        {
            string filePath = CheckFilePath(objFolder, objFilename, isApFolder);
            string dateLog = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            File.AppendAllLines(filePath, new[] { dateLog });
            File.AppendAllLines(filePath, objLog);
        }
    }
}
