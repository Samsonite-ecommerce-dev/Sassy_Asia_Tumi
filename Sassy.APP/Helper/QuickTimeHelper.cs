using Samsonite.Library.Core;
using System;
using System.Collections.Generic;

namespace GlobalIT.Helper
{
    public class QuickTimeHelper
    {
        private IBaseService _baseService;
        public QuickTimeHelper(IBaseService baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// 快速选择时间
        /// </summary>
        /// <returns>List</returns>
        public List<object[]> QuickTimeOption()
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack();

            List<object[]> _result = new List<object[]>()
            {
                new object[] { 1, _languagePack["common_qiucktime_1"] },
                new object[] { 2, _languagePack["common_qiucktime_2"] },
                new object[] { 3, _languagePack["common_qiucktime_3"] },
                new object[] { 4, _languagePack["common_qiucktime_4"] },
                new object[] { 5, _languagePack["common_qiucktime_5"] },
                new object[] { 6, _languagePack["common_qiucktime_6"] },
                new object[] { 7, _languagePack["common_qiucktime_7"] },
                new object[] { 8, _languagePack["common_qiucktime_8"] }
            };
            return _result;
        }

        /// <summary>
        /// 返回对应的SQL查询语句
        /// </summary>
        /// <param name="objType">时间类型</param>
        /// <param name="objParaTime">查询的时间字段</param>
        /// <returns></returns>
        public string[] GetQuickTime(int objType)
        {
            string[] _result = new string[2];
            switch (objType)
            {
                case 1:
                    _result[0] = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 2:
                    _result[0] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 3:
                    _result[0] = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 4:
                    _result[0] = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 5:
                    int _week = (int)DateTime.Now.DayOfWeek;
                    _result[0] = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 6:
                    _result[0] = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 7:
                    _result[0] = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case 8:
                    _result[0] = DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd 00:00:00");
                    _result[1] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    break;
                default:
                    _result[0] = string.Empty;
                    _result[1] = string.Empty;
                    break;
            }
            return _result;
        }
    }
}