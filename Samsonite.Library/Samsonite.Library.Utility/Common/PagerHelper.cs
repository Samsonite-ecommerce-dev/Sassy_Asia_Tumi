using System;

namespace Samsonite.Library.Utility
{
    public class PagerHelper
    {
        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <param name="objTotalCount"></param>
        /// <param name="objPagesize"></param>
        /// <returns></returns>
        public static long CountTotalPage(long objTotalCount, long objPagesize)
        {
            if (objTotalCount < 0)
                objTotalCount = 0;
            return (objTotalCount % objPagesize == 0) ? objTotalCount / objPagesize : objTotalCount / objPagesize + 1;
        }
    }
}

