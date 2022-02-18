using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.WorkService.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Web.WorkService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Samsonite.Library.Business.WorkService
{
    public class SAPService : ISAPService
    {
        private IFtpService _ftpService;
        private appEntities _appDB;
        public SAPService(IFtpService ftpService, appEntities appEntities)
        {
            _ftpService = ftpService;
            _appDB = appEntities;
        }

        #region 保存产品
        /// <summary>
        /// 下载产品信息
        /// </summary>
        /// <returns></returns>
        public CommonResult<SAPMaterialResponse> DownMaterial()
        {
            CommonResult<SAPMaterialResponse> _result = new CommonResult<SAPMaterialResponse>();
            //FTP信息
            var _ftpIdentify = "sap_material";
            var _ftpInfo = _ftpService.GetFtp(_ftpIdentify, true);
            var sapFTPDto = new SapFTPDto()
            {
                Ftp = _ftpInfo,
                Name = "SAP Material",
                FileExt = "txt",
                RemoteFilePath = $"{_ftpInfo.RemoteFilePath}/material",
                LocalSavePath = @"\SAP\Material"

            };
            var ftpResult = _ftpService.DownFileFromFTP(sapFTPDto);
            //遍历文件
            foreach (var file in ftpResult.SuccessFile)
            {
                //解析文件
                var items = this.ParseMaterials(file);
                //保存数据
                var r = this.SaveMaterials(items);
                //结果
                _result.ResultData.AddRange(r);
            }
            //返回信息
            _result.SuccessFiles = ftpResult.SuccessFile;
            _result.FailFiles = ftpResult.FailFile;
            return _result;
        }

        /// <summary>
        /// 解析产品信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<SAPMaterialDto> ParseMaterials(string filePath)
        {
            List<SAPMaterialDto> _result = new List<SAPMaterialDto>();
            if (!File.Exists(filePath)) return _result;
            var lines = File.ReadAllLines(filePath);
            int index = 0;
            foreach (var item in lines)
            {
                index++;
                //第一行标题不取
                if (index > 1)
                {
                    var columns = item.Split("|");

                    //过滤空行(Spare Part不为空)
                    //注:sku可能为空
                    if (!string.IsNullOrEmpty(columns[2]))
                    {
                        SAPMaterialDto sAPMaterialDto = new SAPMaterialDto();
                        sAPMaterialDto.SapMaterialType = columns.Length >= 1 ? columns[0].ToUpper() : ""; ;
                        sAPMaterialDto.SapManufacturerSku = columns.Length >= 2 ? columns[1] : "";
                        sAPMaterialDto.SapMaterialId = columns.Length >= 3 ? columns[2] : ""; ;
                        sAPMaterialDto.SapColor = columns.Length >= 4 ? columns[3] : ""; ;
                        sAPMaterialDto.SapName = columns.Length >= 5 ? columns[4] : ""; ;
                        sAPMaterialDto.SapColorDescription = columns.Length >= 6 ? columns[5] : "";
                        sAPMaterialDto.SapMaterialGroup = columns.Length >= 7 ? columns[6] : "";
                        sAPMaterialDto.SapCollection = columns.Length >= 8 ? columns[7] : "";
                        sAPMaterialDto.SapConstructionType = columns.Length >= 9 ? columns[8] : "";
                        sAPMaterialDto.SapStatus = columns.Length >= 10 ? columns[9] : "";
                        //ZFGS:Material
                        //ZSPA:SparePart
                        if (sAPMaterialDto.SapMaterialType == "ZFGS")
                        {
                            if (string.IsNullOrEmpty(sAPMaterialDto.SapManufacturerSku))
                            {
                                //如果sku为空,则用material+Grid代替
                                sAPMaterialDto.SapManufacturerSku = $"{sAPMaterialDto.SapMaterialId}-{sAPMaterialDto.SapColor}";
                            }
                        }
                        else
                        {
                            if (int.TryParse(sAPMaterialDto.SapMaterialId, out int id))
                                sAPMaterialDto.SapMaterialIdInt = id;
                            if (int.TryParse(sAPMaterialDto.SapMaterialGroup, out id))
                                sAPMaterialDto.SapMaterialGroupInt = id;
                        }

                        _result.Add(sAPMaterialDto);
                    }
                }
            }
            return _result;
        }

        /// <summary>
        /// 保存产品信息
        /// </summary>
        /// <param name="sAPMaterialDtos"></param>
        private List<CommonResultData<SAPMaterialResponse>> SaveMaterials(List<SAPMaterialDto> sAPMaterialDtos)
        {
            List<CommonResultData<SAPMaterialResponse>> _result = new List<CommonResultData<SAPMaterialResponse>>();
            if (sAPMaterialDtos.Any())
            {
                //materials
                var materials = sAPMaterialDtos.Where(t => t.SapMaterialType.ToUpper() == "ZFGS").ToList();
                var r_materials = this.SaveMaterials_ZFGS(materials);
                _result.AddRange(r_materials);
                //sparepart
                var spareparts = sAPMaterialDtos.Where(t => t.SapMaterialType.ToUpper() == "ZSPA").ToList();
                var r_spareparts = this.SaveMaterials_ZSPA(spareparts);
                _result.AddRange(r_spareparts);
            }
            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sAPMaterialDtos"></param>
        /// <returns></returns>
        private List<CommonResultData<SAPMaterialResponse>> SaveMaterials_ZFGS(List<SAPMaterialDto> sAPMaterialDtos)
        {
            try
            {
                List<CommonResultData<SAPMaterialResponse>> _result = new List<CommonResultData<SAPMaterialResponse>>();
                List<Product> products = new List<Product>();

                foreach (var item in sAPMaterialDtos)
                {
                    products.Add(new Product
                    {
                        SKU = item.SapManufacturerSku,
                        MaterialId = item.SapMaterialId,
                        Collection = VariableHelper.SaferequestSQL(item.SapCollection),
                        ColorDescription = VariableHelper.SaferequestSQL(item.SapColorDescription),
                        Gridval = VariableHelper.SaferequestSQL(item.SapColor),
                        ConstructionType = VariableHelper.SaferequestSQL(item.SapConstructionType),
                        Status = item.SapStatus,
                        MaterialDescription = VariableHelper.SaferequestSQL(item.SapName),
                        MaterialGroup = VariableHelper.SaferequestSQL(item.SapMaterialGroup),
                    });
                }

                //处理Sku
                if (products.Any())
                {
                    int skuPageSize = 1000;
                    int skuTotalPage = (int)Math.Ceiling(products.Count / (decimal)skuPageSize);
                    StringBuilder skuSqlBuilder = new StringBuilder();
                    //批量保存
                    for (int page = 0; page < skuTotalPage; page++)
                    {
                        try
                        {
                            skuSqlBuilder = new StringBuilder();
                            foreach (var item in products.Skip(page * skuPageSize).Take(skuPageSize))
                            {
                                skuSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [Product] WHERE MaterialId = N'{item.MaterialId}' AND Gridval=N'{item.Gridval}')");
                                skuSqlBuilder.AppendLine("BEGIN");
                                skuSqlBuilder.AppendLine("INSERT INTO [Product](SKU,MaterialId,Gridval,MaterialDescription,ColorDescription,MaterialGroup,[Collection],ConstructionType,[Status],AddDate,EditDate)");
                                skuSqlBuilder.AppendLine($" VALUES(N'{item.SKU}',N'{item.MaterialId}',N'{item.Gridval}',N'{item.MaterialDescription}',N'{item.ColorDescription}',N'{item.MaterialGroup}',N'{item.Collection}',N'{item.ConstructionType}',N'{item.Status}',getdate(),getdate())");
                                skuSqlBuilder.AppendLine("END");
                                skuSqlBuilder.AppendLine("ELSE");
                                skuSqlBuilder.AppendLine("BEGIN");
                                skuSqlBuilder.AppendLine($"UPDATE [Product] SET SKU=N'{item.SKU}',MaterialDescription=N'{item.MaterialDescription}',ColorDescription=N'{item.ColorDescription}',MaterialGroup=N'{item.MaterialGroup}',[Collection]=N'{item.Collection}',ConstructionType=N'{item.ConstructionType}',[Status]=N'{item.Status}',EditDate=getdate() WHERE MaterialId = N'{item.MaterialId}' AND Gridval=N'{item.Gridval}'");
                                skuSqlBuilder.AppendLine("END");
                            }
                            var tmpResult = _appDB.Database.ExecuteSqlRaw(skuSqlBuilder.ToString());
                            if (tmpResult > 0)
                            {
                                foreach (var item in products.Skip(page * skuPageSize).Take(skuPageSize))
                                {
                                    //添加结果
                                    _result.Add(new CommonResultData<SAPMaterialResponse>()
                                    {
                                        Data = new SAPMaterialResponse()
                                        {
                                            SKU = item.SKU,
                                            SparePartId = 0
                                        },
                                        Result = true,
                                        ResultMessage = string.Empty
                                    });
                                }
                            }
                            else
                            {
                                throw new Exception("Data save fail!");
                            }
                        }
                        catch
                        {
                            //注:SKU保存出错时继续执行后续记录
                            foreach (var item in products.Skip(page * skuPageSize).Take(skuPageSize))
                            {
                                //添加结果
                                _result.Add(new CommonResultData<SAPMaterialResponse>()
                                {
                                    Data = new SAPMaterialResponse()
                                    {
                                        SKU = item.SKU,
                                        SparePartId = 0
                                    },
                                    Result = false,
                                    ResultMessage = string.Empty
                                });
                            }
                        }
                    }
                }
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sAPMaterialDtos"></param>
        /// <returns></returns>
        private List<CommonResultData<SAPMaterialResponse>> SaveMaterials_ZSPA(List<SAPMaterialDto> sAPMaterialDtos)
        {
            try
            {
                List<CommonResultData<SAPMaterialResponse>> _result = new List<CommonResultData<SAPMaterialResponse>>();
                List<SparePart> spareParts = new List<SparePart>();
                List<GroupInfo> groupInfos = new List<GroupInfo>();

                foreach (var item in sAPMaterialDtos)
                {
                    var groupID = VariableHelper.SaferequestInt(item.SapMaterialGroup);
                    groupInfos.Add(new GroupInfo
                    {
                        GroupID = groupID,
                        GroupDescription = VariableHelper.SaferequestSQL(item.SapCollection),
                        GroupText = string.Empty
                    });

                    var sapMaterialId = VariableHelper.SaferequestInt64(item.SapMaterialId);
                    if (sapMaterialId > 0)
                    {
                        spareParts.Add(new SparePart
                        {
                            SparePartID = sapMaterialId,
                            SparePartDescription = VariableHelper.SaferequestSQL(item.SapName),
                            ImageUrl = $"SP{sapMaterialId}.jpg",
                            GroupID = groupID,
                            Status = VariableHelper.SaferequestSQL(item.SapStatus)
                        }); ;
                    }
                }

                //过滤重复Group
                var distinctGroups = groupInfos.GroupBy(t => t.GroupID).Select(t => t.FirstOrDefault()).ToList();

                //处理Group
                if (distinctGroups.Any())
                {
                    int groupPageSize = 1000;
                    int groupTotalPage = (int)Math.Ceiling(distinctGroups.Count / (decimal)groupPageSize);
                    StringBuilder groupSqlBuilder = new StringBuilder();
                    //批量保存
                    for (int page = 0; page < groupTotalPage; page++)
                    {
                        groupSqlBuilder = new StringBuilder();
                        foreach (var item in distinctGroups.Skip(page * groupPageSize).Take(groupPageSize))
                        {
                            groupSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [GroupInfo] WHERE GroupID = {item.GroupID})");
                            groupSqlBuilder.AppendLine("BEGIN");
                            groupSqlBuilder.AppendLine("INSERT INTO [GroupInfo](GroupID,GroupDescription,GroupText,AddDate,EditDate)");
                            groupSqlBuilder.AppendLine($" VALUES('{item.GroupID}',N'{item.GroupDescription}',N'{item.GroupText}',getdate(),getdate())");
                            groupSqlBuilder.AppendLine("END");
                            groupSqlBuilder.AppendLine("ELSE");
                            groupSqlBuilder.AppendLine("BEGIN");
                            groupSqlBuilder.AppendLine($"UPDATE [GroupInfo] SET GroupDescription=N'{item.GroupDescription}',EditDate=getdate() WHERE GroupID = {item.GroupID}");
                            groupSqlBuilder.AppendLine("END");
                        }
                        _appDB.Database.ExecuteSqlRaw(groupSqlBuilder.ToString());
                    }
                }

                //处理sparepart
                if (spareParts.Any())
                {
                    int spPageSize = 1000;
                    int spTotalPage = (int)Math.Ceiling(spareParts.Count / (decimal)spPageSize);
                    StringBuilder spSqlBuilder = new StringBuilder();
                    //批量保存
                    for (int page = 0; page < spTotalPage; page++)
                    {
                        try
                        {
                            spSqlBuilder = new StringBuilder();
                            foreach (var item in spareParts.Skip(page * spPageSize).Take(spPageSize))
                            {
                                spSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [SparePart] WHERE SparePartID = {item.SparePartID})");
                                spSqlBuilder.AppendLine("BEGIN");
                                spSqlBuilder.AppendLine("INSERT INTO [SparePart](SparePartID,SparePartDescription,GroupID,CustomizeSparePart,ImageUrl,BasicPrice,CostPrice,Currency,UnitofMeasure,ValidFrom,ValidTo,PriceUpdateDate,AvailableStock,InventoryStock,InventoryUpdateDate,[Status],Remark,AddDate,EditDate)");
                                spSqlBuilder.AppendLine($" VALUES({item.SparePartID},N'{item.SparePartDescription}',{item.GroupID},'',N'{item.ImageUrl}',0,0,'','',NULL,NULL,NULL,0,0,NULL,N'{item.Status}','',getdate(),getdate())");
                                spSqlBuilder.AppendLine("END");
                                spSqlBuilder.AppendLine("ELSE");
                                spSqlBuilder.AppendLine("BEGIN");
                                spSqlBuilder.AppendLine($"UPDATE [SparePart] SET SparePartDescription=N'{item.SparePartDescription}',Status=N'{item.Status}',EditDate=getdate() WHERE SparePartID = {item.SparePartID}");
                                spSqlBuilder.AppendLine("END");
                            }
                            var tmpResult = _appDB.Database.ExecuteSqlRaw(spSqlBuilder.ToString());
                            if (tmpResult > 0)
                            {
                                foreach (var item in spareParts.Skip(page * spPageSize).Take(spPageSize))
                                {
                                    //添加结果
                                    _result.Add(new CommonResultData<SAPMaterialResponse>()
                                    {
                                        Data = new SAPMaterialResponse()
                                        {
                                            SKU = string.Empty,
                                            SparePartId = item.SparePartID
                                        },
                                        Result = true,
                                        ResultMessage = string.Empty
                                    });
                                }
                            }
                            else
                            {
                                throw new Exception("Data save fail!");
                            }
                        }
                        catch
                        {
                            //注:SKU保存出错时继续执行后续记录
                            foreach (var item in spareParts.Skip(page * spPageSize).Take(spPageSize))
                            {
                                //添加结果
                                _result.Add(new CommonResultData<SAPMaterialResponse>()
                                {
                                    Data = new SAPMaterialResponse()
                                    {
                                        SKU = string.Empty,
                                        SparePartId = item.SparePartID
                                    },
                                    Result = false,
                                    ResultMessage = string.Empty
                                });
                            }
                        }
                    }
                }
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存产品库存
        /// <summary>
        /// 下载产品库存
        /// </summary>
        /// <returns></returns>
        public CommonResult<SAPSparePartInventoryResponse> DownSparePartInventory()
        {
            CommonResult<SAPSparePartInventoryResponse> _result = new CommonResult<SAPSparePartInventoryResponse>();
            //FTP信息
            var _ftpIdentify = "sap_material";
            var _ftpInfo = _ftpService.GetFtp(_ftpIdentify, true);
            var sapFTPDto = new SapFTPDto()
            {
                Ftp = _ftpInfo,
                Name = "SAP Material",
                FileExt = "txt",
                RemoteFilePath = $"{_ftpInfo.RemoteFilePath}/inventory",
                LocalSavePath = @"\SAP\Inventory"

            };
            var ftpResult = _ftpService.DownFileFromFTP(sapFTPDto);
            //遍历文件
            foreach (var file in ftpResult.SuccessFile)
            {
                //解析文件
                var items = this.ParseSparePartInventorys(file);
                //保存数据
                var r = this.SaveSparePartInventorys(items);
                //结果
                _result.ResultData.AddRange(r);
            }
            //返回信息
            _result.SuccessFiles = ftpResult.SuccessFile;
            _result.FailFiles = ftpResult.FailFile;
            return _result;
        }

        /// <summary>
        /// 解析产品库存
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<SAPSparePartInventoryDto> ParseSparePartInventorys(string filePath)
        {
            List<SAPSparePartInventoryDto> _result = new List<SAPSparePartInventoryDto>();
            if (!File.Exists(filePath)) return _result;
            var lines = File.ReadAllLines(filePath);
            int index = 0;
            foreach (var item in lines)
            {
                index++;
                //第一行标题不取
                if (index > 1)
                {
                    var columns = item.Split("|");
                    var sparePartId = VariableHelper.SaferequestInt64(columns[0]);
                    //过滤空行
                    if (sparePartId > 0)
                    {
                        _result.Add(new SAPSparePartInventoryDto()
                        {
                            SparePartId = sparePartId,
                            //注意:库存值为10.000,需要使用decimal转化
                            Quantity = (int)VariableHelper.SaferequestNullDecimal(columns[1]),
                            StockDate = VariableHelper.ParseDate(columns[2])
                        });
                    }
                }
            }
            return _result;
        }

        /// <summary>
        /// 保存产品库存
        /// </summary>
        /// <param name="sAPSparePartInventoryDtos"></param>
        private List<CommonResultData<SAPSparePartInventoryResponse>> SaveSparePartInventorys(List<SAPSparePartInventoryDto> sAPSparePartInventoryDtos)
        {
            try
            {
                List<CommonResultData<SAPSparePartInventoryResponse>> _result = new List<CommonResultData<SAPSparePartInventoryResponse>>();
                if (sAPSparePartInventoryDtos.Any())
                {
                    int pageSize = 1000;
                    int totalPage = (int)Math.Ceiling(sAPSparePartInventoryDtos.Count / (decimal)pageSize);
                    StringBuilder sqlBuilder = new StringBuilder();
                    //批量保存
                    for (int page = 0; page < totalPage; page++)
                    {
                        try
                        {
                            sqlBuilder = new StringBuilder();
                            foreach (var item in sAPSparePartInventoryDtos.Skip(page * pageSize).Take(pageSize))
                            {
                                sqlBuilder.AppendLine($"UPDATE [SparePart] SET AvailableStock='{item.Quantity}',InventoryStock='{item.Quantity}',InventoryUpdateDate='{item.StockDate}' WHERE SparePartID = {item.SparePartId}");
                            }
                            var tmpResult = _appDB.Database.ExecuteSqlRaw(sqlBuilder.ToString());
                            if (tmpResult > 0)
                            {
                                foreach (var item in sAPSparePartInventoryDtos.Skip(page * pageSize).Take(pageSize))
                                {
                                    //添加结果
                                    _result.Add(new CommonResultData<SAPSparePartInventoryResponse>()
                                    {
                                        Data = new SAPSparePartInventoryResponse()
                                        {
                                            SparePartId = item.SparePartId,
                                            Quantity = item.Quantity
                                        },
                                        Result = true,
                                        ResultMessage = string.Empty
                                    });
                                }
                            }
                            else
                            {
                                throw new Exception("Data save fail!");
                            }
                        }
                        catch
                        {
                            //注:SKU保存出错时继续执行后续记录
                            foreach (var item in sAPSparePartInventoryDtos.Skip(page * pageSize).Take(pageSize))
                            {
                                //添加结果
                                _result.Add(new CommonResultData<SAPSparePartInventoryResponse>()
                                {
                                    Data = new SAPSparePartInventoryResponse()
                                    {
                                        SparePartId = item.SparePartId,
                                        Quantity = item.Quantity
                                    },
                                    Result = false,
                                    ResultMessage = string.Empty
                                });
                            }
                        }
                    }
                }
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存产品价格
        /// <summary>
        /// 下载产品价格
        /// </summary>
        /// <returns></returns>
        public CommonResult<SAPSparePartPriceResponse> DownSparePartPrice()
        {
            CommonResult<SAPSparePartPriceResponse> _result = new CommonResult<SAPSparePartPriceResponse>();
            //FTP信息
            var _ftpIdentify = "sap_material";
            var _ftpInfo = _ftpService.GetFtp(_ftpIdentify, true);
            var sapFTPDto = new SapFTPDto()
            {
                Ftp = _ftpInfo,
                Name = "SAP Material",
                FileExt = "txt",
                RemoteFilePath = $"{_ftpInfo.RemoteFilePath}/price",
                LocalSavePath = @"\SAP\Price"

            };
            var ftpResult = _ftpService.DownFileFromFTP(sapFTPDto);
            //遍历文件
            foreach (var file in ftpResult.SuccessFile)
            {
                //解析文件
                var items = this.ParseSparePartPrices(file);
                //保存数据
                var r = this.SaveSparePartPrices(items);
                //结果
                _result.ResultData.AddRange(r);
            }
            //返回信息
            _result.SuccessFiles = ftpResult.SuccessFile;
            _result.FailFiles = ftpResult.FailFile;
            return _result;
        }

        /// <summary>
        /// 解析产品价格
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<SAPSparePartPriceDto> ParseSparePartPrices(string filePath)
        {
            List<SAPSparePartPriceDto> _result = new List<SAPSparePartPriceDto>();
            if (!File.Exists(filePath)) return _result;
            var lines = File.ReadAllLines(filePath);
            int index = 0;
            foreach (var item in lines)
            {
                index++;
                //第一行标题不取
                if (index > 1)
                {
                    var columns = item.Split("|");
                    var sparePartId = VariableHelper.SaferequestInt64(columns[0]);
                    //过滤空行
                    if (sparePartId > 0)
                    {
                        _result.Add(new SAPSparePartPriceDto()
                        {
                            SparePartId = sparePartId,
                            ValidFrom = VariableHelper.ParseDate(columns[1]),
                            ValidTo = VariableHelper.ParseDate(columns[2]),
                            ProductPrice = VariableHelper.SaferequestDecimal(columns[3]),
                            Quantity = VariableHelper.SaferequestInt(columns[4]),
                            UnitofMeasure = VariableHelper.SaferequestSQL(columns[5]),
                            Currency = VariableHelper.SaferequestSQL(columns[6])
                        });
                    }
                }
            }
            return _result;
        }

        /// <summary>
        /// 保存产品价格
        /// </summary>
        /// <param name="sAPSparePartPriceDtos"></param>
        private List<CommonResultData<SAPSparePartPriceResponse>> SaveSparePartPrices(List<SAPSparePartPriceDto> sAPSparePartPriceDtos)
        {
            try
            {
                List<CommonResultData<SAPSparePartPriceResponse>> _result = new List<CommonResultData<SAPSparePartPriceResponse>>();
                if (sAPSparePartPriceDtos.Any())
                {
                    int pageSize = 1000;
                    int totalPage = (int)Math.Ceiling(sAPSparePartPriceDtos.Count / (decimal)pageSize);
                    StringBuilder sqlBuilder = new StringBuilder();
                    //批量保存
                    for (int page = 0; page < totalPage; page++)
                    {
                        try
                        {
                            sqlBuilder = new StringBuilder();
                            foreach (var item in sAPSparePartPriceDtos.Skip(page * pageSize).Take(pageSize))
                            {
                                sqlBuilder.AppendLine($"UPDATE [SparePart] SET BasicPrice={item.ProductPrice},CostPrice={item.ProductPrice},Currency=N'{item.Currency}',UnitofMeasure=N'{item.UnitofMeasure}',ValidFrom='{item.ValidFrom}',ValidTo='{item.ValidTo}',PriceUpdateDate=getdate() WHERE SparePartID = {item.SparePartId}");
                            }
                            var tmpResult = _appDB.Database.ExecuteSqlRaw(sqlBuilder.ToString());
                            if (tmpResult > 0)
                            {
                                foreach (var item in sAPSparePartPriceDtos.Skip(page * pageSize).Take(pageSize))
                                {
                                    //添加结果
                                    _result.Add(new CommonResultData<SAPSparePartPriceResponse>()
                                    {
                                        Data = new SAPSparePartPriceResponse()
                                        {
                                            SparePartId = item.SparePartId,
                                            Price = item.ProductPrice
                                        },
                                        Result = true,
                                        ResultMessage = string.Empty
                                    });
                                }
                            }
                            else
                            {
                                throw new Exception("Data save fail!");
                            }
                        }
                        catch
                        {
                            //注:SKU保存出错时继续执行后续记录
                            foreach (var item in sAPSparePartPriceDtos.Skip(page * pageSize).Take(pageSize))
                            {
                                //添加结果
                                _result.Add(new CommonResultData<SAPSparePartPriceResponse>()
                                {
                                    Data = new SAPSparePartPriceResponse()
                                    {
                                        SparePartId = item.SparePartId,
                                        Price = item.ProductPrice
                                    },
                                    Result = false,
                                    ResultMessage = string.Empty
                                });
                            }
                        }
                    }
                }
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
