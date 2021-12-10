﻿using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Custom.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Samsonite.Library.Business.Custom
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
            var ftpResult = this.DownFileFormSAP(sapFTPDto);
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

                    //过滤空行
                    if (!string.IsNullOrEmpty(columns[1]) && !string.IsNullOrEmpty(columns[2]))
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
                            if (sAPMaterialDto.SapManufacturerSku.Length >= 3)
                                sAPMaterialDto.LineId = sAPMaterialDto.SapManufacturerSku.Substring(0, 3);
                            if (sAPMaterialDto.SapManufacturerSku.Length >= 6)
                                sAPMaterialDto.ColourId = sAPMaterialDto.SapManufacturerSku.Substring(4, 2);
                            if (sAPMaterialDto.SapManufacturerSku.Length >= 9)
                                sAPMaterialDto.SizeId = sAPMaterialDto.SapManufacturerSku.Substring(6, 3);
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
                //List<ProductLineSize> productLineSizes = new List<ProductLineSize>();
                //List<ProductLineColor> productLineColors = new List<ProductLineColor>();
                //List<ProductLine> productLines = new List<ProductLine>();

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

                //foreach (var item in sAPMaterialDtos)
                //{
                //    string sku = item.SapManufacturerSku.ToUpper();
                //    string line = sku.Length >= 3 ? sku.Substring(0, 3) : "";
                //    string lineColor = sku.Length >= 6 ? sku.Substring(4, 2) : "";
                //    //最后三位
                //    string lineSize = sku.Length >= 9 ? sku.Substring(sku.Length - 3, 3) : "";
                //    productLines.Add(new ProductLine
                //    {
                //        LineID = line,
                //        LineDescription = VariableHelper.SaferequestSQL(item.SapCollection),
                //        LineText = string.Empty
                //    });

                //    productLineColors.Add(new ProductLineColor
                //    {
                //        LineID = line,
                //        ColorID = lineColor,
                //        ColorDescription = VariableHelper.SaferequestSQL(item.SapColorDescription),
                //        ColorText = string.Empty
                //    });

                //    productLineSizes.Add(new ProductLineSize
                //    {
                //        LineID = line,
                //        SizeID = lineSize,
                //        SizeDescription = VariableHelper.SaferequestSQL(item.SapName),
                //        SizeText = string.Empty,
                //        Length = 0,
                //        Width = 0,
                //        Height = 0,
                //        Volume = 0
                //    });
                //}

                ////Line对象去重
                //var distinctLine = productLines.Where(t => !string.IsNullOrWhiteSpace(t.LineID)).GroupBy(t => t.LineID).Select(t => t.FirstOrDefault()).ToList();

                ////过滤重复 LineColours
                //var distinctLineColours = productLineColors.Where(t => !string.IsNullOrWhiteSpace(t.ColorID)).GroupBy(t => t.LineID + t.ColorID).Select(t => t.FirstOrDefault()).ToList();

                ////过滤重复 LineSizes
                //var distinctLineSizes = productLineSizes.Where(t => !string.IsNullOrWhiteSpace(t.SizeID)).GroupBy(t => t.LineID + t.SizeID).Select(t => t.FirstOrDefault()).ToList();

                ////处理Line
                //if (distinctLine.Any())
                //{
                //    int linePageSize = 1000;
                //    int lineTotalPage = (int)Math.Ceiling(distinctLine.Count / (decimal)linePageSize);
                //    StringBuilder lineSqlBuilder = new StringBuilder();
                //    //批量保存
                //    for (int page = 0; page < lineTotalPage; page++)
                //    {
                //        lineSqlBuilder = new StringBuilder();
                //        foreach (var item in distinctLine.Skip(page * linePageSize).Take(linePageSize))
                //        {
                //            lineSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [ProductLine] WHERE LineID = N'{item.LineID}')");
                //            lineSqlBuilder.AppendLine("BEGIN");
                //            lineSqlBuilder.AppendLine("INSERT INTO [ProductLine](LineID,LineDescription,LineText,AddDate,EditDate)");
                //            lineSqlBuilder.AppendLine($" VALUES(N'{item.LineID}',N'{item.LineDescription}',N'{item.LineText}',getdate(),getdate())");
                //            lineSqlBuilder.AppendLine("END");
                //            lineSqlBuilder.AppendLine("ELSE");
                //            lineSqlBuilder.AppendLine("BEGIN");
                //            lineSqlBuilder.AppendLine($"UPDATE [ProductLine] SET LineDescription=N'{item.LineDescription}',EditDate=getdate() WHERE LineID = N'{item.LineID}'");
                //            lineSqlBuilder.AppendLine("END");
                //        }
                //        _appDB.Database.ExecuteSqlRaw(lineSqlBuilder.ToString());
                //    }
                //}
                ////处理Line Color
                //if (distinctLineColours.Any())
                //{
                //    int colorPageSize = 1000;
                //    int colorTotalPage = (int)Math.Ceiling(distinctLineColours.Count / (decimal)colorPageSize);
                //    StringBuilder colorSqlBuilder = new StringBuilder();
                //    //批量保存
                //    for (int page = 0; page < colorTotalPage; page++)
                //    {
                //        colorSqlBuilder = new StringBuilder();
                //        foreach (var item in distinctLineColours.Skip(page * colorPageSize).Take(colorPageSize))
                //        {
                //            colorSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [ProductLineColor] WHERE LineID = N'{item.LineID}' AND ColorID=N'{item.ColorID}')");
                //            colorSqlBuilder.AppendLine("BEGIN");
                //            colorSqlBuilder.AppendLine("INSERT INTO [ProductLineColor](LineID,ColorID,ColorDescription,ColorText,AddDate,EditDate)");
                //            colorSqlBuilder.AppendLine($" VALUES(N'{item.LineID}',N'{item.ColorID}',N'{item.ColorDescription}',N'{item.ColorText}',getdate(),getdate())");
                //            colorSqlBuilder.AppendLine("END");
                //            colorSqlBuilder.AppendLine("ELSE");
                //            colorSqlBuilder.AppendLine("BEGIN");
                //            colorSqlBuilder.AppendLine($"UPDATE [ProductLineColor] SET ColorDescription=N'{item.ColorDescription}',EditDate=getdate() WHERE LineID = N'{item.LineID}' AND ColorID=N'{item.ColorID}'");
                //            colorSqlBuilder.AppendLine("END");
                //        }
                //        _appDB.Database.ExecuteSqlRaw(colorSqlBuilder.ToString());
                //    }
                //}
                ////处理Line Size
                //if (distinctLineSizes.Any())
                //{
                //    int sizePageSize = 1000;
                //    int sizeTotalPage = (int)Math.Ceiling(distinctLineSizes.Count / (decimal)sizePageSize);
                //    StringBuilder sizeSqlBuilder = new StringBuilder();
                //    //批量保存
                //    for (int page = 0; page < sizeTotalPage; page++)
                //    {
                //        sizeSqlBuilder = new StringBuilder();
                //        foreach (var item in distinctLineSizes.Skip(page * sizePageSize).Take(sizePageSize))
                //        {
                //            sizeSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [ProductLineSize] WHERE LineID = N'{item.LineID}' AND SizeID=N'{item.SizeID}')");
                //            sizeSqlBuilder.AppendLine("BEGIN");
                //            sizeSqlBuilder.AppendLine("INSERT INTO [ProductLineSize](LineID,SizeID,SizeDescription,SizeText,[Length],Width,Height,Volume,AddDate,EditDate)");
                //            sizeSqlBuilder.AppendLine($" VALUES(N'{item.LineID}',N'{item.SizeID}',N'{item.SizeDescription}',N'{item.SizeText}',{item.Length},{item.Width},{item.Height},{item.Volume},getdate(),getdate())");
                //            sizeSqlBuilder.AppendLine("END");
                //            sizeSqlBuilder.AppendLine("ELSE");
                //            sizeSqlBuilder.AppendLine("BEGIN");
                //            sizeSqlBuilder.AppendLine($"UPDATE [ProductLineSize] SET SizeDescription=N'{item.SizeDescription}',EditDate=getdate() WHERE LineID = N'{item.LineID}' AND SizeID=N'{item.SizeID}'");
                //            sizeSqlBuilder.AppendLine("END");
                //        }
                //        _appDB.Database.ExecuteSqlRaw(sizeSqlBuilder.ToString());
                //    }
                //}

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
                                skuSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [Product] WHERE SKU = N'{item.SKU}')");
                                skuSqlBuilder.AppendLine("BEGIN");
                                skuSqlBuilder.AppendLine("INSERT INTO [Product](SKU,MaterialId,Gridval,MaterialDescription,ColorDescription,MaterialGroup,[Collection],ConstructionType,[Status],AddDate,EditDate)");
                                skuSqlBuilder.AppendLine($" VALUES(N'{item.SKU}',N'{item.MaterialId}',N'{item.Gridval}',N'{item.MaterialDescription}',N'{item.ColorDescription}',N'{item.MaterialGroup}',N'{item.Collection}',N'{item.ConstructionType}',N'{item.Status}',getdate(),getdate())");
                                skuSqlBuilder.AppendLine("END");
                                skuSqlBuilder.AppendLine("ELSE");
                                skuSqlBuilder.AppendLine("BEGIN");
                                skuSqlBuilder.AppendLine($"UPDATE [Product] SET MaterialId=N'{item.MaterialId}',Gridval=N'{item.Gridval}',MaterialDescription=N'{item.MaterialDescription}',ColorDescription=N'{item.ColorDescription}',MaterialGroup=N'{item.MaterialGroup}',[Collection]=N'{item.Collection}',ConstructionType=N'{item.ConstructionType}',[Status]=N'{item.Status}',EditDate=getdate() WHERE SKU = N'{item.SKU}'");
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
            var ftpResult = this.DownFileFormSAP(sapFTPDto);
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
            var ftpResult = this.DownFileFormSAP(sapFTPDto);
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

        #region 函数
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private FTPResult DownFileFormSAP(SapFTPDto config)
        {
            FtpDto _ftpDto = config.Ftp;
            //FTP文件目录
            SFTPHelper sftpHelper = new SFTPHelper(_ftpDto.FtpServerIp, _ftpDto.UserId, _ftpDto.Password);
            //本地保存文件目录
            string _localPath = $"{AppDomain.CurrentDomain.BaseDirectory + config.LocalSavePath}/{DateTime.Now.ToString("yyyy-MM")}/{DateTime.Now.ToString("yyyyMMdd")}";
            //下载文件
            return _ftpService.DownFileFromsFtp(sftpHelper, config.RemoteFilePath, _localPath, config.FileExt, _ftpDto.IsDeleteOriginalFile);
        }
        #endregion
    }
}
