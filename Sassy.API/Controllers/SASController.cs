using Microsoft.AspNetCore.Mvc;
using Samsonite.Library.Utility;
using Samsonite.Library.WebApi;
using Samsonite.Library.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Samsonite.Library.API.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class SASController : BaseController
    {
        private ISASService _sASService;
        public SASController(ISASService sASService)
        {
            _sASService = sASService;
        }

        /// <summary>
        /// 获取SKU关联的配件数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("spareparts/related/get")]
        public ApiGetResponse GetRelatedSpareParts([FromQuery]GetSparePartRequest request)
        {
            ApiGetResponse _result = new ApiGetResponse();

            //过滤参数
            ValidateHelper.Validate(request);

            try
            {
                if (string.IsNullOrEmpty(request.Sku))
                {
                    throw new Exception("Please input a SKU!");
                }

                var _res = _sASService.GetSpareParts(request);
                //返回信息
                _result.Code = (int)ApiResultCode.Success;
                _result.Message = string.Empty;
                _result.Data = _res.Data;
            }
            catch (Exception ex)
            {
                //返回信息
                _result.Code = (int)ApiResultCode.Fail;
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
                var _res = _sASService.GetSparePartGroups(request);
                //返回信息
                _result.Code = (int)ApiResultCode.Success;
                _result.Message = string.Empty;
                _result.Data = _res.Data;
            }
            catch (Exception ex)
            {
                //返回信息
                _result.Code = (int)ApiResultCode.Fail;
                _result.Message = ex.Message;
            }
            return _result;
        }
    }
}