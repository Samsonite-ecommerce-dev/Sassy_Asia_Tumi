using System;

namespace Samsonite.Library.Web.Core.Models
{
    /// <summary>
    /// 自定义特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class CustomPropertyAttribute : Attribute
    {
        public string CustomName { get; set; }
    }
}
