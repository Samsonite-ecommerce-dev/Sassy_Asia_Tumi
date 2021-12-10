using Samsonite.Library.Business.Custom.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;
using System.Collections.Generic;

namespace Samsonite.Library.Business.Custom
{
    public interface ISparePartQueryService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<View_SparePart> GetQuery(SparePartQuerySearchRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(SparePartQueryEditRequest request);

        /// <summary>
        /// 获取状态列表
        /// </summary>
        /// <returns></returns>
        List<string> GetStatusOption();

        /// <summary>
        /// 创建图片名称
        /// </summary>
        /// <param name="sparepartID"></param>
        /// <returns></returns>
        string CreateImageName(long sparepartID);

        /// <summary>
        /// 获取图片Html代码
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        string GetImageHtml(string imageName, int width);

        /// <summary>
        /// 获取图片Http访问路径
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        string GetImageHttp(string imageName);
    }
}
