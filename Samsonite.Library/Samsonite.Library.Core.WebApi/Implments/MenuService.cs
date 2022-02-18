using Samsonite.Library.Core.WebApi.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Core.WebApi
{
    public class MenuService : IMenuService
    {
        /// <summary>
        /// 获取api接口用途
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public int GetAPIType(string controller)
        {
            int _result = 0;
            switch (controller)
            {
                case "SAS":
                    _result = (int)ApiType.SAS;
                    break;
            }

            return _result;
        }

        /// <summary>
        /// 功能组列表
        /// </summary>
        /// <returns></returns>
        public List<ApiMenuGroup> InterfaceOptions()
        {
            List<ApiMenuGroup> _result = new List<ApiMenuGroup>();
            //SAS接口
            _result.Add(new ApiMenuGroup()
            {
                GroupID = 1,
                GroupName = "SAS",
                ControllerName = ApiType.SAS.ToString(),
                Interfaces = new List<ApiMenuInfo>()
                {
                    new ApiMenuInfo()
                    {
                         ID=100,
                         RouteGroup="",
                         InterfaceName="Get Products",
                         ActionName="GetProducts",
                         SeqNumber=100
                    },
                    new ApiMenuInfo()
                    {
                         ID=200,
                         RouteGroup="",
                         InterfaceName="Get SpareParts",
                         ActionName="GetSpareParts",
                         SeqNumber=200
                    },
                    new ApiMenuInfo()
                    {
                         ID=210,
                         RouteGroup="",
                         InterfaceName="Get Spare Part Groups",
                         ActionName="GetSparePartGroups",
                         SeqNumber=210
                    },
                    new ApiMenuInfo()
                    {
                         ID=220,
                         RouteGroup="",
                         InterfaceName="Get Related SpareParts",
                         ActionName="GetRelatedSpareParts",
                         SeqNumber=220
                    }
                },
                RootID = 1
            });
            return _result;
        }
    }
}
