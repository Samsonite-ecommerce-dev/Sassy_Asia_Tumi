using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Bussness.WebApi.Models;
using Samsonite.Library.Data.Entity.Models;
using System.Linq;

namespace Samsonite.Library.Bussness.WebApi
{
    public class ProductService : IProductService
    {
        private appEntities _appDB;
        public ProductService(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 获取产品集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetProductResponse GetProductQuery(GetProductRequest request)
        {
            GetProductResponse _result = new GetProductResponse();
            var _list = _appDB.Product.AsQueryable();

            //返回数据
            _result.TotalRecord = _list.Count();
            _result.Data = (from item in _list.AsNoTracking().Skip((request.CurrentPage - 1) * request.CurrentPage).Take(request.PageSize)
                            select new ProductInfo()
                            {
                                Sku = item.SKU,
                                Material = item.MaterialId,
                                MaterialDesc = item.MaterialDescription,
                                Grid = item.Gridval,
                                GridDesc = item.ColorDescription,
                                MaterialGroup = item.MaterialGroup,
                                EAN = item.EAN,
                                Collection = item.Collection
                            }).ToList();
            return _result;
        }
    }
}
