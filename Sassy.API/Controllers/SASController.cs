using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Bussness.WebApi;
using Samsonite.Library.Bussness.WebApi.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.WebApi;
using Samsonite.Library.Core.WebApi.Models;
using Samsonite.Library.Core.WebApi.Utils;
using System;

namespace Samsonite.Library.API.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class SASController : BaseController
    {
        private IProductService _productService;
        private ISparePartService _sparePartService;
        private UtilsHelper _utilsHelper;
        public SASController(IProductService productService, ISparePartService sparePartService)
        {
            _productService = productService;
            _sparePartService = sparePartService;
            _utilsHelper = new UtilsHelper();
        }

        #region Product
        /// <summary>
        /// 获取产品集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("products/get")]
        public ApiGetResponse GetProducts([FromQuery]GetProductRequest request)
        {
            ApiPageResponse _result = new ApiPageResponse();

            //过滤参数
            ValidateHelper.Validate(request);

            request.PageSize = _utilsHelper.ValidatePageSize(request.PageSize);
            request.CurrentPage = _utilsHelper.ValidateCurrentPage(request.CurrentPage);

            try
            {
                var _res = _productService.GetProductQuery(request);
                //返回信息
                _result.Code = (int)ApiResultCode.Success;
                _result.Message = string.Empty;
                _result.Data = _res.Data;
                _result.TotalRecord = _res.TotalRecord;
                _result.TotalPage = (int)PagerHelper.CountTotalPage(_res.TotalRecord, request.PageSize);
                _result.PageSize = request.PageSize;
                _result.CurrentPage = request.CurrentPage;
            }
            catch (ApiException ex)
            {
                //返回信息
                _result.Code = (int)ex.ErrorCode;
                _result.Message = ex.Message;
            }
            return _result;
        }
        #endregion

        #region Spare Part
        /// <summary>
        /// 获取配件集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("spareparts/get")]
        public ApiGetResponse GetSpareParts([FromQuery]GetSparePartRequest request)
        {
            ApiPageResponse _result = new ApiPageResponse();

            //过滤参数
            ValidateHelper.Validate(request);

            request.PageSize = _utilsHelper.ValidatePageSize(request.PageSize);
            request.CurrentPage = _utilsHelper.ValidateCurrentPage(request.CurrentPage);

            try
            {
                var _res = _sparePartService.GetSparePartQuery(request);
                //返回信息
                _result.Code = (int)ApiResultCode.Success;
                _result.Message = string.Empty;
                _result.Data = _res.Data;
                _result.TotalRecord = _res.TotalRecord;
                _result.TotalPage = (int)PagerHelper.CountTotalPage(_res.TotalRecord, request.PageSize);
                _result.PageSize = request.PageSize;
                _result.CurrentPage = request.CurrentPage;
            }
            catch (ApiException ex)
            {
                //返回信息
                _result.Code = (int)ex.ErrorCode;
                _result.Message = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 获取SKU关联的配件数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("spareparts/groups/get")]
        public ApiGetResponse GetSparePartGroups([FromQuery]GetSparePartGroupsRequest request)
        {
            ApiGetResponse _result = new ApiGetResponse();

            //过滤参数
            ValidateHelper.Validate(request);

            try
            {
                var _res = _sparePartService.GetSparePartGroups(request);
                //返回信息
                _result.Code = (int)ApiResultCode.Success;
                _result.Message = string.Empty;
                _result.Data = _res.Data;
            }
            catch (ApiException ex)
            {
                //返回信息
                _result.Code = (int)ex.ErrorCode;
                _result.Message = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 获取SKU关联的配件数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("spareparts/related/get")]
        public ApiGetResponse GetRelatedSpareParts([FromQuery]GetSparePartRelatedRequest request)
        {
            ApiGetResponse _result = new ApiGetResponse();

            //过滤参数
            ValidateHelper.Validate(request);

            try
            {
                if (string.IsNullOrEmpty(request.Sku))
                {
                    throw new ApiException((int)ApiResultCode.InvalidParameter, "Please input a SKU!");
                }

                var _res = _sparePartService.GetSparePartRelateds(request);
                //返回信息
                _result.Code = (int)ApiResultCode.Success;
                _result.Message = string.Empty;
                _result.Data = _res.Data;
            }
            catch (ApiException ex)
            {
                //返回信息
                _result.Code = (int)ex.ErrorCode;
                _result.Message = ex.Message;
            }
            return _result;
        }
        #endregion
    }
}