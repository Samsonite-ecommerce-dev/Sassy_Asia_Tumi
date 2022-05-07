using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Samsonite.Library.Utility
{
    public class NPOIExcelHelper
    {
        private string fileName = null;
        private IWorkbook _workbook = null;
        public NPOIExcelHelper(string fileName)
        {
            this.fileName = fileName;

        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="cellCount">总列数</param>
        /// <param name="startRow">第一行数据行号</param>
        /// <param name="startColumn">第一列开始索引</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, int cellCount = -1, int startRow = 0, int startColumn = 0)
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    bool isHSSFWordbook = false;
                    //2007版本及以上版本
                    if (fileName.IndexOf(".xlsx") > 0 || fileName.IndexOf(".xlsm") > 0)
                        _workbook = new XSSFWorkbook(fs);
                    //2003版本
                    else if (fileName.IndexOf(".xls") > 0)
                    {
                        isHSSFWordbook = true;
                        _workbook = new HSSFWorkbook(fs);
                    }

                    ISheet _sheet = sheetName != null ? (_workbook.GetSheet(sheetName) ?? _workbook.GetSheetAt(0)) : _workbook.GetSheetAt(0);
                    if (_sheet != null)
                    {
                        IRow firstRow = _sheet.GetRow(startRow);
                        //如果小于0 就表示取总行数
                        if (cellCount < 0)
                        {
                            //一行最后一个cell的编号 即总的列数
                            cellCount = firstRow.LastCellNum;
                        }
                        //判断传入参数是否大于总列数，如果是，重新赋值。避免索引越线
                        if (cellCount > firstRow.LastCellNum)
                        {
                            cellCount = firstRow.LastCellNum;
                        }

                        for (int i = startColumn; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellName = "cell" + i;
                                string cellValue = GetCellValue(cell, _workbook, isHSSFWordbook);
                                if (cellValue != null)
                                {
                                    cellName = cellValue;
                                }
                                if (dataTable.Columns.Contains(cellName))
                                {
                                    cellName = "cell" + i;
                                }
                                DataColumn column = new DataColumn(cellName);
                                dataTable.Columns.Add(column);
                            }
                        }

                        startRow = startRow + 1;

                        //最后一列的标号
                        int rowCount = _sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; ++i)
                        {
                            IRow row = _sheet.GetRow(i);
                            //没有数据的行默认是null
                            if (row == null) continue;

                            DataRow dataRow = dataTable.NewRow();

                            int columnIndex = 0;
                            for (int j = startColumn; j < cellCount; ++j)
                            {
                                var cell = row.GetCell(j);
                                //同理，没有数据的单元格都默认是null
                                if (cell != null)
                                {
                                    dataRow[columnIndex] = this.GetCellValue(cell, _workbook, isHSSFWordbook);
                                }
                                columnIndex++;
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                        return dataTable;
                    }
                    else
                    {
                        throw new Exception("The excel file has an incorrect extension. It should be XLS or XLSX!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            try
            {
                int count = 0;
                //2007版本
                if (this.fileName.IndexOf(".xlsx") > 0)
                    _workbook = new XSSFWorkbook();
                //2003版本
                else if (this.fileName.IndexOf(".xls") > 0)
                    _workbook = new HSSFWorkbook();
                if (_workbook != null)
                {
                    ISheet _sheet = _workbook.CreateSheet(sheetName);
                    //colnums
                    if (isColumnWritten == true)
                    {
                        IRow row = _sheet.CreateRow(0);
                        for (var j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }

                    //rows
                    for (var i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = _sheet.CreateRow(count);
                        for (var j = 0; j < data.Columns.Count; ++j)
                        {
                            var column = data.Columns[j];
                            var cellDataType = column.DataType;
                            var cellContent = data.Rows[i][j];

                            this.ConvertCellValue(row.CreateCell(j), cellDataType, cellContent);
                        }
                        count++;
                    }

                    //生成文件
                    using (var fs = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        //写入到excel
                        _workbook.Write(fs);
                        //释放
                        fs.Flush();
                        fs.Close();
                    }

                    return count;
                }
                else
                {
                    throw new Exception("The excel file has an incorrect extension. It should be XLS or XLSX!");
                }
            }
            catch
            {
                return -1;
            }
            finally { }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="cellCount">总列数</param>
        /// <param name="startRow">第一行数据行号</param>
        /// <param name="startColumn">第一列开始索引</param>
        /// <param name="titleRows">标题行</param>
        /// <param name="titleAlignment">标题列对齐方式</param>
        /// <param name="mergeTitleRow">合并标题行</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, int cellCount = -1, int startRow = 0, int startColumn = 0, MergeTitleRow mergeTitleRow = null)
        {
            try
            {
                DataTable data = new DataTable();

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    bool isHssfWordbook = false;
                    //2007版本及以上版本
                    if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0 || fileName.IndexOf(".xlsm", StringComparison.Ordinal) > 0)
                        _workbook = new XSSFWorkbook(fs);
                    //2003版本
                    else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
                    {
                        isHssfWordbook = true;
                        _workbook = new HSSFWorkbook(fs);
                    }
                    var sheet = sheetName != null ? (_workbook.GetSheet(sheetName) ?? _workbook.GetSheetAt(0)) : _workbook.GetSheetAt(0);
                    if (sheet != null)
                    {
                        data = SheetToTable(sheet, cellCount, startRow, startColumn, isHssfWordbook, mergeTitleRow);
                        return data;
                    }
                    else
                    {
                        throw new Exception("The excel file has an incorrect extension. It should be XLS or XLSX!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        /// <summary>
        /// 转换 Sheet 为DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cellCount"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="isHssfWordbook"></param>
        /// <param name="mergeTitleRow">合并标题行</param>
        /// <returns></returns>
        private DataTable SheetToTable(ISheet sheet, int cellCount = -1, int startRow = 0, int startColumn = 0, bool isHssfWordbook = false, MergeTitleRow mergeTitleRow = null)
        {
            DataTable data = new DataTable(sheet.SheetName);

            IRow firstRow = sheet.GetRow(startRow);
            if (cellCount < 0) //如果小于0 就表示取总行数
            {
                cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
            }
            if (cellCount > firstRow.LastCellNum)  //判断传入参数是否大于总列数，如果是，重新赋值。避免索引越线
            {
                cellCount = firstRow.LastCellNum;
            }

            if (mergeTitleRow == null)  //判断是否需要处理特殊标题列
            {
                for (int i = startColumn; i < cellCount; ++i)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (cell != null)
                    {
                        string cellName = "cell" + i;
                        string cellValue = GetCellValue(cell, _workbook, isHssfWordbook);
                        if (cellValue != null)
                        {
                            cellName = cellValue;
                        }
                        if (data.Columns.Contains(cellName))
                        {
                            cellName = "cell" + i;
                        }
                        data.Columns.Add(new DataColumn(cellName));
                    }
                }
                startRow = startRow + 1;
            }
            else
            {
                string[] columns = new string[cellCount];
                foreach (var row in mergeTitleRow.Rows)
                {
                    var sheetRow = sheet.GetRow(row.PaddingRowIndex);
                    int cellLenght = row.Length >= 0 ? row.Length : cellCount - row.PaddingLength;
                    for (int i = 0; i < cellLenght; i++)
                    {
                        int startCellNum = row.PaddingColumnIndex + row.PaddingLength;
                        ICell cell = sheetRow.GetCell(startCellNum + i);
                        if (cell != null)
                        {
                            string cellName = "cell" + i;
                            string cellValue = GetCellValue(cell, _workbook, isHssfWordbook);
                            if (cellValue != null)
                            {
                                cellName = cellValue;
                            }
                            if (data.Columns.Contains(cellName))
                            {
                                cellName = "cell" + i;
                            }
                            columns[startCellNum + i] = cellName;
                        }
                    }
                }

                foreach (var columnName in columns)
                {
                    data.Columns.Add(new DataColumn(columnName));
                }
            }

            int whiteSpaceRows = 0;

            //最后一列的标号
            int rowCount = sheet.LastRowNum;
            for (int i = startRow; i <= rowCount; ++i)
            {
                if (whiteSpaceRows >= 3) //如果空白行大于三行就跳出
                {
                    break;
                }

                IRow row = sheet.GetRow(i);
                if (row == null) continue; //没有数据的行默认是null　　　　　　　

                bool isWhiteSpaceRows = true;

                DataRow dataRow = data.NewRow();
                int columnIndex = 0;
                for (int j = startColumn; j < cellCount; ++j)
                {
                    var cell = row.GetCell(j);
                    if (cell != null)//同理，没有数据的单元格都默认是null
                    {
                        string value = GetCellValue(cell, _workbook, isHssfWordbook);
                        dataRow[columnIndex] = value;

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            isWhiteSpaceRows = false;
                        }
                    }
                    columnIndex++;
                }

                if (isWhiteSpaceRows)
                {
                    whiteSpaceRows++;
                }

                data.Rows.Add(dataRow);
            }
            return data;
        }

        /// <summary>
        /// 获取cell的值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="workbook"></param>
        /// <param name="isHSSFWordbook">是否是03版本</param>
        /// <returns></returns>
        private string GetCellValue(ICell cell, IWorkbook workbook, bool isHSSFWordbook)
        {
            string value = "";
            if (cell == null)
            {
                value = null;
            }
            else
            {
                //读取Excel格式，根据格式读取数据类型
                switch (cell.CellType)
                {
                    case CellType.Blank: //空数据类型处理
                        value = "";
                        break;
                    case CellType.String: //字符串类型
                        value = cell.StringCellValue;
                        break;
                    //case CellType.Numeric: //数字类型                                   
                    //    if (DateUtil.IsValidExcelDate(cell.NumericCellValue))
                    //    {
                    //        value = cell.DateCellValue.ToString();
                    //    }
                    //    else
                    //    {
                    //        value = cell.NumericCellValue.ToString();
                    //    }
                    //    break;
                    case CellType.Formula:
                        if (isHSSFWordbook)
                        {
                            try
                            {
                                HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(workbook);
                                value = e.Evaluate(cell).StringValue;

                                if (string.IsNullOrEmpty(value))
                                {
                                    value = e.EvaluateInCell(cell).ToString();
                                }
                            }
                            catch
                            {
                                value = "";
                            }
                        }
                        else
                        {
                            try
                            {
                                XSSFFormulaEvaluator evaluator = new XSSFFormulaEvaluator(workbook);
                                value = evaluator.Evaluate(cell).StringValue;

                                if (string.IsNullOrEmpty(value))
                                {
                                    value = evaluator.EvaluateInCell(cell).ToString();
                                }
                            }
                            catch
                            {
                                value = "";
                            }
                        }
                        break;
                    default:
                        value = cell.ToString();
                        break;
                }
            }
            return value;
        }

        /// <summary>
        /// cell中插入字段对应类型值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellDataColumnType"></param>
        /// <param name="value"></param>
        private void ConvertCellValue(ICell cell, Type cellDataColumnType, object value)
        {
            if (cellDataColumnType == typeof(Int16) || cellDataColumnType == typeof(Int32) || cellDataColumnType == typeof(Int64))
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (cellDataColumnType == typeof(Decimal) || cellDataColumnType == typeof(Double) || cellDataColumnType == typeof(float))
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (cellDataColumnType == typeof(DateTime))
            {
                cell.SetCellValue(Convert.ToDateTime(value));
            }
            else
            {
                //默认文本
                cell.SetCellValue(value.ToString());
            }
        }

        /// <summary>
        /// 向sheet插入图片
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="workbook"></param>
        /// <param name="fileurl"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AddCellPicture(ISheet sheet, IWorkbook workbook, string fileurl, int row, int col)
        {
            try
            {
                if (File.Exists(fileurl))
                {
                    var bytes = File.ReadAllBytes(fileurl);
                    var pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);
                    var patriarch = sheet.CreateDrawingPatriarch();

                    //2007版本
                    if (fileName.IndexOf(".xlsx") > 0)
                    {
                        var anchor = new XSSFClientAnchor(0, 0, 0, 0, col, row, col + 1, row + 1);
                        var pict = (XSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    }
                    //2003版本
                    else if (fileName.IndexOf(".xls") > 0)
                    {
                        var anchor = new HSSFClientAnchor(0, 0, 0, 0, col, row, col + 1, row + 1);
                        var pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 合并标题行
    /// </summary>
    public class MergeTitleRow
    {
        public List<MergeRow> Rows = new List<MergeRow>();
    }

    public class MergeRow
    {
        /// <summary>
        /// 填充行开始位置---从左到右
        /// </summary>
        public int PaddingColumnIndex { get; set; } = 0;

        /// <summary>
        /// 填充长度，如果是-1 就表示获取默认列长度
        /// </summary>
        public int PaddingLength { get; set; } = -1;

        /// <summary>
        /// 需要填充的行
        /// </summary>
        public int PaddingRowIndex { get; set; }

        /// <summary>
        /// 获取列长度，默认为-1,按行长度自动获取
        /// </summary>
        public int Length { get; set; } = -1;
    }
}
