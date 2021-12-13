using Samsonite.Library.Bussness.WebApi.Models;

namespace Samsonite.Library.Bussness.WebApi
{
    public interface ISparePartService
    {
        /// <summary>
        /// 获取配件集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetSparePartResponse GetSparePartQuery(GetSparePartRequest request);

        /// <summary>
        /// 获取配件分组集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetSparePartGroupsResponse GetSparePartGroups(GetSparePartGroupsRequest request);

        /// <summary>
        /// 根据sku获取关联的配件号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetSparePartRelatedResponse GetSparePartRelateds(GetSparePartRelatedRequest request);
    }
}
