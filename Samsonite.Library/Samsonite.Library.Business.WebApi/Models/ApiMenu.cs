using System.Collections.Generic;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class ApiMenuGroup
    {
        /// <summary>
        /// 
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 组值
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int RootID { get; set; }

        /// <summary>
        /// 功能组列表
        /// </summary>
        public List<ApiMenuInfo> Interfaces { get; set; }
    }

    public class ApiMenuInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 路由组
        /// </summary>
        public string RouteGroup { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// 功能值
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SeqNumber { get; set; }
    }
}
