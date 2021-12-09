using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.WebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.WebApi
{
    public class SASService : ISASService
    {
        private appEntities _appDB;
        public SASService(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 根据SKU获取关联的配件号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetSparePartResponse GetSpareParts(GetSparePartRequest request)
        {
            GetSparePartResponse _result = new GetSparePartResponse();
            List<SkuRelated> _datas = new List<SkuRelated>();
            var _list = _appDB.View_ProductSparePart.AsQueryable();

            if (!string.IsNullOrEmpty(request.Sku))
            {
                _list = _list.Where(p => p.SKU.Contains(request.Sku));
            }

            if (request.GroupID > 0)
            {
                _list = _list.Where(p => p.GroupID == request.GroupID);
            }

            var _skuArray = _list.GroupBy(p => p.SKU).Select(o => o.Key).ToList();
            foreach (var item in _skuArray)
            {
                _datas.Add(new SkuRelated()
                {
                    Sku = item,
                    SpareParts = (from sp in _list.Where(p => p.SKU == item)
                                  select new SparePartInfo()
                                  {
                                      SparePartID = sp.SparePartID,
                                      SpartPartDesc = sp.SparePartDescription,
                                      SpartPartImage = sp.ImageUrl,
                                      GroupID = sp.GroupID,
                                      GroupDesc = sp.GroupName
                                  }).ToList()

                });
            }

            //返回信息
            _result.Data = _datas;

            return _result;
        }

        /// <summary>
        /// 获取配件分组集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetSparePartGroupsResponse GetSparePartGroups(GetSparePartGroupsRequest request)
        {
            GetSparePartGroupsResponse _result = new GetSparePartGroupsResponse();
            _result.Data = (from item in _appDB.GroupInfo
                            select new SparePartGroupInfo()
                            {
                                GroupID = item.GroupID,
                                GroupDesc = item.GroupDescription
                            }).ToList();
            return _result;
        }
    }
}
