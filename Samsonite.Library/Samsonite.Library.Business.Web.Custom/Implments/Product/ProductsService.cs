using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core.Models;
using System.Linq;

namespace Samsonite.Library.Business.Web.Custom
{
    public class ProductsService : IProductsService
    {
        private appEntities _appDB;
        public ProductsService(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<Product> GetQuery(ProductsSearchRequest request)
        {
            QueryResponse<Product> _result = new QueryResponse<Product>();
            var _list = _appDB.Product.AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                //搜索条件
                //0.sku(默认)
                //1.mat+grid
                if (request.SearchType == 1)
                {
                    _list = _list.Where(p => (p.MaterialId + "-" + p.Gridval).Contains(request.Keyword));
                }
                else
                {
                    _list = _list.Where(p => p.SKU.Contains(request.Keyword));
                }
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.OrderBy(p => p.ID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }
    }
}
