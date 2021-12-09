using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Samsonite.Library.Business.Models;
using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Samsonite.Library.Business
{
    public class UploadSparePartService : IUploadSparePartService
    {
        private IBaseService _baseService;
        private IHostEnvironment _hostEnvironment;
        private appEntities _appDB;
        public UploadSparePartService(IBaseService baseService, IHostEnvironment hostEnvironment, appEntities appEntities)
        {
            _baseService = baseService;
            _hostEnvironment = hostEnvironment;
            _appDB = appEntities;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<UploadSparePartImportResponse> Import(UploadSparePartImportRequest request)
        {
            QueryResponse<UploadSparePartImportResponse> _result = new QueryResponse<UploadSparePartImportResponse>();
            //初始化返回信息
            var _list = new List<UploadSparePartImportResponse>();

            string _filePath = $"{_hostEnvironment.ContentRootPath}/wwwroot/{request.FileName}";
            if (System.IO.File.Exists(_filePath))
            {
                //读取文件
                var _excelHelper = new ExcelHelper(_filePath);
                int skuCloumnSrartIndex = 3;
                MergeTitleRow mergeTitleRow = new MergeTitleRow();
                mergeTitleRow.Rows.Add(new MergeRow
                {
                    PaddingColumnIndex = 0,
                    PaddingLength = skuCloumnSrartIndex,
                    PaddingRowIndex = 0
                });
                mergeTitleRow.Rows.Add(new MergeRow
                {
                    PaddingColumnIndex = 0,
                    PaddingLength = 0,
                    PaddingRowIndex = 1,
                    Length = skuCloumnSrartIndex
                });
                var table = _excelHelper.ExcelToDataTable("", -1, 2, 0, mergeTitleRow);
                foreach (DataRow row in table.Rows)
                {
                    var tmpSkus = new List<UploadSparePartSku>();
                    for (int i = skuCloumnSrartIndex; i < table.Columns.Count; i++)
                    {
                        if (row[i] != null && row[i].ToString().Trim().ToUpper() == "X")
                        {
                            var tmpSku = VariableHelper.SaferequestSQL(table.Columns[i].ColumnName);
                            tmpSkus.Add(new UploadSparePartSku()
                            {
                                SKU = tmpSku,
                                //LineID = tmpSku.Length > 3 ? tmpSku.Substring(0, 3) : "",
                                //ColorID = tmpSku.Length > 6 ? tmpSku.Substring(4, 2) : "",
                                //SizeID = tmpSku.Length >= 9 ? tmpSku.Substring(6, 3) : ""
                            });
                        }
                    }

                    _list.Add(new UploadSparePartImportResponse()
                    {
                        SparePartId = VariableHelper.SaferequestInt64(row[0]),
                        SparePartDesc = VariableHelper.SaferequestSQL(row[1].ToString()),
                        VersionID = VariableHelper.SaferequestSQL(row[2].ToString()),
                        GroupID = 0,
                        Skus = tmpSkus
                    });
                }
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 保存导入Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<UploadSparePartImportResponse> ImportSave(UploadSparePartImportRequest request)
        {
            //加载语言包
            var _languagePack = _baseService.CurrentLanguagePack();

            List<UploadSparePartImportResponse> _result = new List<UploadSparePartImportResponse>();

            string _filePath = $"{_hostEnvironment.ContentRootPath}/wwwroot/{request.FileName}";
            if (System.IO.File.Exists(_filePath))
            {
                //读取文件
                var _excelHelper = new ExcelHelper(_filePath);
                int skuCloumnSrartIndex = 3;
                MergeTitleRow mergeTitleRow = new MergeTitleRow();
                mergeTitleRow.Rows.Add(new MergeRow
                {
                    PaddingColumnIndex = 0,
                    PaddingLength = skuCloumnSrartIndex,
                    PaddingRowIndex = 0
                });
                mergeTitleRow.Rows.Add(new MergeRow
                {
                    PaddingColumnIndex = 0,
                    PaddingLength = 0,
                    PaddingRowIndex = 1,
                    Length = skuCloumnSrartIndex
                });
                var table = _excelHelper.ExcelToDataTable("", -1, 2, 0, mergeTitleRow);
                List<string> skus = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    var sparePartId = VariableHelper.SaferequestInt64(row[0]);
                    //过滤重复的SparePartID
                    //注:防止下面提示错误信息时候的Dictionary因为数据重复而出错
                    if (!_result.Exists(p => p.SparePartId == sparePartId))
                    {
                        var tmpSkus = new List<UploadSparePartSku>();
                        var versionID = VariableHelper.SaferequestSQL(row[2].ToString());
                        for (int i = skuCloumnSrartIndex; i < table.Columns.Count; i++)
                        {
                            if (row[i] != null && row[i].ToString().Trim().ToUpper() == "X")
                            {
                                var tmpSku = VariableHelper.SaferequestSQL(table.Columns[i].ColumnName);
                                if (!skus.Contains(tmpSku))
                                {
                                    skus.Add(tmpSku);
                                }

                                tmpSkus.Add(new UploadSparePartSku()
                                {
                                    SKU = tmpSku,
                                    //LineID = tmpSku.Length > 3 ? tmpSku.Substring(0, 3) : "",
                                    //ColorID = tmpSku.Length > 6 ? tmpSku.Substring(4, 2) : "",
                                    //SizeID = tmpSku.Length >= 9 ? tmpSku.Substring(6, 3) : "",
                                });
                            }
                        }

                        _result.Add(new UploadSparePartImportResponse()
                        {
                            SparePartId = sparePartId,
                            SparePartDesc = VariableHelper.SaferequestSQL(row[1].ToString()),
                            VersionID = VariableHelper.SaferequestSQL(row[2].ToString()),
                            GroupID = 0,
                            Skus = tmpSkus
                        });
                    }
                }

                //保存数据
                if (_result.Any())
                {
                    var sparePartIds = _result.Select(p => p.SparePartId).Distinct().ToList();

                    //缓存校验数据到本地,提供性能
                    var existsSparepart = _appDB.SparePart.Where(p => sparePartIds.Contains(p.SparePartID)).ToList();
                    var existsSku = _appDB.Product.Where(p => skus.Contains(p.SKU)).Select(o => o.SKU).ToList();
                    var existsSparepartIds = _appDB.SparePart.Where(p => sparePartIds.Contains(p.SparePartID)).Select(p => p.SparePartID).ToList();

                    int pageSize = 100;
                    int totalPage = (int)Math.Ceiling(_result.Count / (decimal)pageSize);
                    StringBuilder spSqlBuilder = new StringBuilder();
                    //批量保存
                    for (int page = 0; page < totalPage; page++)
                    {
                        try
                        {
                            var errSparePartIds = new Dictionary<long, string>();
                            spSqlBuilder = new StringBuilder();
                            foreach (var item in _result.Skip(page * pageSize).Take(pageSize))
                            {
                                if (!existsSparepartIds.Contains(item.SparePartId))
                                {
                                    errSparePartIds.Add(item.SparePartId, "The SparePart dose not exists!");
                                    continue;
                                }
                                else
                                {
                                    var errSkus = new List<string>();
                                    foreach (var s in item.Skus)
                                    {
                                        if (!existsSku.Contains(s.SKU))
                                        {
                                            errSkus.Add(s.SKU);
                                            continue;
                                        }

                                        spSqlBuilder.AppendLine($"IF NOT EXISTS(SELECT * from [ProductSparePart] WHERE SKU=N'{s.SKU}' AND VersionID=N'{item.VersionID}' AND SparePartID = {item.SparePartId})");
                                        spSqlBuilder.AppendLine("BEGIN");
                                        spSqlBuilder.AppendLine("INSERT INTO [ProductSparePart](SKU,LineID,SizeID,ColorID,GroupID,VersionID,SparePartID,AddDate,EditDate)");
                                        spSqlBuilder.AppendLine($" VALUES(N'{s.SKU}',N'{s.LineID}',N'{s.SizeID}',N'{s.ColorID}',ISNULL((SELECT TOP 1 GroupID from SparePart WHERE SparePartID={item.SparePartId}),0),N'{item.VersionID}',{item.SparePartId},getdate(),getdate())");
                                        spSqlBuilder.AppendLine("END");
                                        spSqlBuilder.AppendLine("ELSE");
                                        spSqlBuilder.AppendLine("BEGIN");
                                        spSqlBuilder.AppendLine($"UPDATE [ProductSparePart] SET GroupID=ISNULL((SELECT TOP 1 GroupID from SparePart WHERE SparePartID={item.SparePartId}),0),EditDate=getdate() WHERE SKU=N'{s.SKU}' AND VersionID=N'{item.VersionID}' AND SparePartID = {item.SparePartId}");
                                        spSqlBuilder.AppendLine("END");
                                    }

                                    if (errSkus.Any())
                                    {
                                        errSparePartIds.Add(item.SparePartId, $"The SKU:{string.Join(",", errSkus)} dose not exists!");
                                    }
                                }
                            }

                            if (spSqlBuilder.Length > 0)
                            {
                                //执行SQL语句
                                var tmpResult = _appDB.Database.ExecuteSqlRaw(spSqlBuilder.ToString());
                                if (tmpResult > 0)
                                {
                                    //返回信息
                                    foreach (var item in _result.Skip(page * pageSize).Take(pageSize))
                                    {
                                        if (!errSparePartIds.ContainsKey(item.SparePartId))
                                        {
                                            item.Result = true;
                                            item.ResultMsg = string.Empty;
                                        }
                                        else
                                        {
                                            item.Result = false;
                                            item.ResultMsg = errSparePartIds[item.SparePartId];
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Data save fail!");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //返回信息
                            foreach (var item in _result.Skip(page * pageSize).Take(pageSize))
                            {
                                item.Result = false;
                                item.ResultMsg = $"<label class=\"text-danger\">{ex.Message}</label>";
                            }
                        }
                    }
                }
            }

            //返回全部错误数据
            return _result.Where(p => !p.Result).ToList();
        }
    }
}
