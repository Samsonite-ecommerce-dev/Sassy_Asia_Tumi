using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Business.Web.Custom
{
    public class ProductQueryService : IProductQueryService
    {
        private IBaseService _baseService;
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public ProductQueryService(IBaseService baseService, IAppLogService appLogService, appEntities appEntities)
        {
            _baseService = baseService;
            _appLogService = appLogService;
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<View_ProductSparePart> GetQuery(ProductQuerySearchRequest request)
        {
            QueryResponse<View_ProductSparePart> _result = new QueryResponse<View_ProductSparePart>();
            var _list = _appDB.View_ProductSparePart.AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                //搜索条件
                if (request.SearchType == 1)
                {
                    //查询sku
                    var products = _appDB.Product.Where(p => (p.MaterialId + "-" + p.Gridval).Contains(request.Keyword)).GroupBy(p => p.SKU).Select(o => o.Key).ToList();
                    _list = _list.Where(p => products.Contains(p.SKU));
                }
                else
                {
                    _list = _list.Where(p => p.SKU.Contains(request.Keyword));
                }
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.SparePartID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 查询Line列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<ProductLine> GetLineQuery(ProductQuerySearchLineRequest request)
        {
            QueryResponse<ProductLine> _result = new QueryResponse<ProductLine>();
            var _list = _appDB.ProductLine.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.SearchKey))
            {
                _list = _list.Where(p => p.LineID.Contains(request.SearchKey) || p.LineDescription.Contains(request.SearchKey));
            }
            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.LineID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 返回Color列表
        /// </summary>
        /// <returns></returns>
        public List<ProductLineColor> GetColorQuery(ProductQuerySearchColorRequest request)
        {
            var _list = _appDB.ProductLineColor.AsQueryable();

            if (!string.IsNullOrEmpty(request.LineID))
            {
                _list = _list.Where(p => p.LineID == request.LineID);
            }

            return _list.AsNoTracking().ToList();
        }

        /// <summary>
        /// 查询Size列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ProductLineSize> GetSizeQuery(ProductQuerySearchSizeRequest request)
        {
            var _list = _appDB.ProductLineSize.AsQueryable();

            if (!string.IsNullOrEmpty(request.LineID))
            {
                _list = _list.Where(p => p.LineID == request.LineID);
            }

            return _list.AsNoTracking().ToList();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Delete(int[] ids)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack;

            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception(_languagePack["common_data_need_one"]);
                }

                ProductSparePart objProductSparePart = new ProductSparePart();
                foreach (var id in ids)
                {
                    objProductSparePart = _appDB.ProductSparePart.Where(p => p.ID == id).SingleOrDefault();
                    if (objProductSparePart != null)
                    {
                        _appDB.ProductSparePart.Remove(objProductSparePart);
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, _languagePack["common_data_no_exsit"]));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("ProductSparePart", string.Join(",", ids));
                //返回信息
                return new PostResponse
                {
                    Result = true,
                    Message = _languagePack["common_data_delete_success"]
                };
            }
            catch (Exception ex)
            {
                return new PostResponse
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 查询Group列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<GroupInfo> GetGroupQuery(ProductQuerySearchGroupRequest request)
        {
            List<GroupInfo> _result = new List<GroupInfo>();
            var _list = _appDB.ProductSparePart.AsQueryable();

            if (!string.IsNullOrEmpty(request.LineID) && !string.IsNullOrEmpty(request.SizeID))
            {
                _list = _list.Where(p => p.LineID == request.LineID && p.SizeID == request.SizeID);
            }

            if (_list.Any())
            {
                var _groupIDs = _list.GroupBy(p => p.GroupID).Select(o => o.Key);
                _result = _appDB.GroupInfo.Where(p => _groupIDs.Contains(p.GroupID)).AsNoTracking().ToList();
            }

            return _result;
        }
    }
}
