using System;

namespace Samsonite.Library.Utility
{
    public class TimeHelper
    {
        public enum DateInterval
        {
            Second, Minute, Hour, Day, Week, Month, Quarter, Year
        }

        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="objInterval">类型</param>
        /// <param name="objStartDate">起始时间</param>
        /// <param name="objEndDate"></param>
        /// <returns>string</returns>
        public static long DateDiff(DateInterval objInterval, DateTime objStartDate, DateTime objEndDate)
        {
            long lngDateDiffValue = 0;
            TimeSpan TS = new TimeSpan(objEndDate.Ticks - objStartDate.Ticks);
            switch (objInterval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case DateInterval.Day:
                    lngDateDiffValue = (long)TS.TotalDays;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long)(TS.Days / 30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long)(TS.Days / 365);
                    break;
            }
            return (lngDateDiffValue);
        }

        /// <summary>
        /// 差异时间
        /// </summary>
        /// <param name="objtime">时间</param>
        /// <returns>string</returns>
        public static string LongTimeAgo(DateTime objtime)
        {
            string _result = string.Empty;
            long _time = 0;
            TimeSpan TS = new System.TimeSpan(DateTime.Now.Ticks - objtime.Ticks);
            _time = (long)TS.TotalSeconds;
            if (_time >= 0)
            {
                //秒
                if (_time <= 60)
                {
                    _result = _time + "秒前";
                }
                //分钟
                else if (_time <= 60 * 30)
                {
                    _result = _time / 60 + "分钟前";
                }
                //半小时
                else if (_time <= 60 * 60)
                {
                    _result = "半小时前";
                }
                //小时
                else if (_time <= 60 * 60 * 12)
                {
                    _result = _time / 60 / 60 + "小时前";
                }
                //半天
                else if (_time <= 60 * 60 * 24)
                {
                    _result = "半天前";
                }
                //天
                else if (_time <= 60 * 60 * 24 * 7)
                {
                    _result = _time / 60 / 60 / 24 + "天前";
                }
                //周
                else if (_time <= 60 * 60 * 24 * 15)
                {
                    _result = _time / 60 / 60 / 24 / 7 + "周前";
                }
                //半月
                else if (_time <= 60 * 60 * 24 * 30)
                {
                    _result = "半个月前";
                }
                //月
                else if (_time <= 60 * 60 * 24 * 30 * 6)
                {
                    _result = _time / 60 / 60 / 24 / 30 + "个月前";
                }
                //半年
                else if (_time <= 60 * 60 * 24 * 30 * 12)
                {
                    _result = "半年前";
                }
                //年
                else
                {
                    _result = _time / 60 / 60 / 24 / 30 / 12 + "年前";
                }
            }

            return _result;
        }

        /// <summary>
        /// 日期转换成unix时间戳
        /// UNIX时间戳的0按照ISO 8601规范为 ：1970-01-01T00:00:00Z
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            try
            {
                DateTime zeroTime = new DateTime(1970, 1, 1, 0, 0, 0);
                int timeStamp = Convert.ToInt32((dateTime.ToUniversalTime() - zeroTime).TotalSeconds);
                return timeStamp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long unixTimeStamp)
        {
            try
            {
                DateTime zeroTime = new DateTime(1970, 1, 1, 0, 0, 0);
                long lTime = ((long)unixTimeStamp * 10000000);
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime targetDt = zeroTime.Add(toNow);
                targetDt = TimeZoneInfo.ConvertTimeFromUtc(targetDt, TimeZoneInfo.Local);
                return targetDt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
