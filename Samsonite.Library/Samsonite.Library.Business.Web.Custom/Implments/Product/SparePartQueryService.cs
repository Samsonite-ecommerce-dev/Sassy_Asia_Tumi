using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Samsonite.Library.Business.Web.Custom
{
    public class SparePartQueryService : ISparePartQueryService
    {
        private IBaseService _baseService;
        private IAppLogService _appLogService;
        private IHostEnvironment _hostEnvironment;
        private AppConfigModel _currentApplicationConfig;
        private appEntities _appDB;
        public SparePartQueryService(IBaseService baseService, IAppLogService appLogService, IHostEnvironment hostEnvironment, appEntities appEntities)
        {
            _baseService = baseService;
            _appLogService = appLogService;
            _hostEnvironment = hostEnvironment;
            _currentApplicationConfig = _baseService.CurrentApplicationConfig;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<View_SparePart> GetQuery(SparePartQuerySearchRequest request)
        {
            QueryResponse<View_SparePart> _result = new QueryResponse<View_SparePart>();
            var _list = _appDB.View_SparePart.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.SparePartKey))
            {
                _list = _list.Where(p => p.SparePartID.ToString().Contains(request.SparePartKey) || p.SparePartDescription.Contains(request.SparePartKey));
            }

            if (request.GroupID > 0)
            {
                _list = _list.Where(p => p.GroupID == request.GroupID);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                _list = _list.Where(p => p.Status == request.Status);
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.SparePartID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Edit(SparePartQueryEditRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                SparePart objData = _appDB.SparePart.Where(p => p.SparePartID == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.ImageUrl = request.ImageUrl;

                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<SparePart>(objData, objData.SparePartID.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = _languagePack["common_data_save_success"]
                    };
                }
                else
                {
                    throw new Exception(_languagePack["common_data_load_false"]);
                }
            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取状态列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetStatusOption()
        {
            return _appDB.SparePart.GroupBy(p => p.Status).Select(o => o.Key).ToList();
        }

        /// <summary>
        /// 创建图片名称
        /// </summary>
        /// <param name="sparepartID"></param>
        /// <returns></returns>
        public string CreateImageName(long sparepartID)
        {
            return $"SP{sparepartID}.jpg";
        }

        /// <summary>
        /// 获取图片相对路径
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private string GetImageUrl(string imageName)
        {
            return $"{_baseService.CurrentApplicationConfig.GlobalConfig.ImagePath}/SparePartImage/{imageName}";
        }

        /// <summary>
        /// 获取图片绝对路径
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private string GetImageAbsoluteUrl(string imageName)
        {
            return $"{_hostEnvironment.ContentRootPath}/wwwroot/{this.GetImageUrl(imageName)}";
        }

        /// <summary>
        /// 获取图片Html代码
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public string GetImageHtml(string imageName, int width)
        {
            string _imageUrl = this.GetImageUrl(imageName);
            var _absolutePath = $"{_hostEnvironment.ContentRootPath}/wwwroot/{_imageUrl}";
            //判断是否存在该图片
            if (File.Exists(_absolutePath))
            {
                return $"<a href=\"javascript:appVue.previewImage('{_imageUrl}')\" class=\"common-image\"><img src=\"{_imageUrl}\" style=\"width:{width}px\" /></a>";
            }
            else
            {
                return $"<div class=\"common-image-none\" style=\"width:{width}px\">No Pic</div>";
            }
        }

        /// <summary>
        /// 获取图片Http访问路径
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public string GetImageHttp(string imageName)
        {
            return $"{_currentApplicationConfig.GlobalConfig.HttpURL}{_currentApplicationConfig.GlobalConfig.ImagePath}/SparePartImage/{imageName}";
        }
    }
}
