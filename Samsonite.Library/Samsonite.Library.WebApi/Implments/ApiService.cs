using Samsonite.Library.WebApi.Models;
using System.Collections.Generic;

namespace Samsonite.Library.WebApi
{
    public class ApiService : IApiService
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
                         InterfaceName="Get Related SpareParts",
                         ActionName="GetRelatedSpareParts",
                         SeqNumber=100
                    },
                    new ApiMenuInfo()
                    {
                         ID=110,
                         RouteGroup="",
                         InterfaceName="Get Spare Part Groups",
                         ActionName="GetSparePartGroups",
                         SeqNumber=110
                    }
                },
                RootID = 1
            });
            return _result;
        }
    }
}
