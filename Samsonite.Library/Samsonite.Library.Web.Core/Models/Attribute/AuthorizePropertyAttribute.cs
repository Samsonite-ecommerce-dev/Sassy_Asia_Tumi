using System;

namespace Samsonite.Library.Web.Core.Models
{
    /// <summary>
    /// 过滤器自定义特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class AuthorizePropertyAttribute : Attribute
    {
        public string Action { get; set; }

        public bool IsAntiforgeryToken { get; set; }
    }
}
