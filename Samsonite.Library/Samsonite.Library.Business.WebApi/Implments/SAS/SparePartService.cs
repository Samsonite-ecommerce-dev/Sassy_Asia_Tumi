using Samsonite.Library.Bussness.WebApi.Models;
using Samsonite.Library.Data.Entity.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Bussness.WebApi
{
    public class SparePartService : ISparePartService
    {
        private appEntities _appDB;
        public SparePartService(appEntities appEntities)
        {
            _appDB = appEntities;
        }

        /// <summary>
        /// 获取配件集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetSparePartResponse GetSparePartQuery(GetSparePartRequest request)
        {
            GetSparePartResponse _result = new GetSparePartResponse();
            var _list = _appDB.View_SparePart.AsQueryable();

            //返回数据
            _result.TotalRecord = _list.Count();
            _result.Data = (from item in _list.Skip((request.CurrentPage - 1) * request.CurrentPage).Take(request.PageSize)
                            select new SparePartInfo()
                            {
                                SparePartID = item.SparePartID,
                                SpartPartDesc = item.SparePartDescription,
                                SpartPartImage = item.ImageUrl,
                                GroupID = item.GroupID,
                                GroupDesc = item.GroupName,
                                BasePrice = item.BasicPrice,
                                UnitofMeasure = item.UnitofMeasure,
                                Status = item.Status
                            }).ToList();
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

        /// <summary>
        /// 根据SKU获取关联的配件号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetSparePartRelatedResponse GetSparePartRelateds(GetSparePartRelatedRequest request)
        {
            GetSparePartRelatedResponse _result = new GetSparePartRelatedResponse();
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
    }
}
