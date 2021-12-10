using System;
using System.Collections.Generic;
using System.Text;

namespace Samsonite.Library.Web.Core.Models
{
    public class CommonResult
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// 成功数
        /// </summary>
        public int SuccessRecord { get; set; }

        /// <summary>
        /// 失败数
        /// </summary>
        public int FailRecord { get; set; }
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class CommonResult<T>
    {
        private List<CommonResultData<T>> _resultData = new List<CommonResultData<T>>();
        /// <summary>
        /// 总数
        /// </summary>
        public List<CommonResultData<T>> ResultData
        {
            get { return _resultData; }
            set { _resultData = value; }
        }

        /// <summary>
        /// 成功数
        /// </summary>
        public List<string> SuccessFiles { get; set; }

        /// <summary>
        /// 失败数
        /// </summary>
        public List<string> FailFiles { get; set; }
    }

    public class CommonResultData<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 信息描述
        /// </summary>
        public string ResultMessage { get; set; }
    }
}
