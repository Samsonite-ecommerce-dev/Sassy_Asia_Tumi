using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.Core.Models;

namespace Samsonite.Library.Business.Web.Basic
{
    public interface IFtpConfigService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        QueryResponse<FTPInfo> GetQuery(FtpConfigSearchRequest request);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Add(FtpConfigAddRequest request);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PostResponse Edit(FtpConfigEditRequest request);

        /// <summary>
        /// 根据标识获取FTP信息
        /// </summary>
        /// <param name="ftpIdentify"></param>
        /// <returns></returns>
        FTPInfo GetFtpInfo(string ftpIdentify);
    }
}
