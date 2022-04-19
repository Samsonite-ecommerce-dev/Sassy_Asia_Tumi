using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;

namespace Sassy.APP.Helper
{
    public class LogHelper
    {
        #region 日志级别
        /// <summary>
        /// 日志等级集合
        /// </summary>
        /// <returns></returns>
        private List<object[]> LogLevelReflect()
        {
            List<object[]> _result = new List<object[]>()
            {
                new object[] { (int)LogLevel.Info, "Info" },
                new object[] { (int)LogLevel.Warning, "Warning" },
                new object[] { (int)LogLevel.Error, "Error" },
                new object[] { (int)LogLevel.Debug, "Debug" }
            };
            return _result;
        }

        /// <summary>
        /// 日志等级列表
        /// </summary>
        /// <returns></returns>
        public List<DefineSelectOption> LogLevelObject()
        {
            List<DefineSelectOption> _result = new List<DefineSelectOption>();
            foreach (var _o in LogLevelReflect())
            {
                _result.Add(new DefineSelectOption
                {
                    Label = _o[1].ToString(),
                    Value = _o[0]
                });
            }
            return _result;
        }

        /// <summary>
        /// 日志等级显示值
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string GetLogTypeDisplay(int status)
        {
            string _result = string.Empty;
            foreach (var _O in LogLevelReflect())
            {
                if ((int)_O[0] == status)
                {
                    _result = _O[1].ToString();
                    break;
                }
            }
            return _result;
        }
        #endregion
    }
}