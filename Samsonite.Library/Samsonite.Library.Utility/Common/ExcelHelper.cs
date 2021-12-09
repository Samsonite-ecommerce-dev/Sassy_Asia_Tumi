using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Samsonite.Library.Utility
{
    public class ExcelHelper
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
            if (ZipStrings.CodePage == 1)
            {
                ZipStrings.CodePage = 437;
            }
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="templatePath">模板地址</param>
        /// <param name="data">要导入的数据</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="startRowIndex"></param>
        /// <param name="startColumIndex"></param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcelByTemplate(string templatePath, DataTable data, string sheetName, int startRowIndex = 1, int startColumIndex = 0)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);              //删除旧文件
                File.Copy(templatePath, fileName);  //把模板文件copy到新地址
            }

            ISheet sheet;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                for (int rowIndex = 0; rowIndex < data.Rows.Count; ++rowIndex)
                {
                    int insertRowIndex = startRowIndex + rowIndex;
                    IRow row = sheet.CreateRow(insertRowIndex);
                    for (int coulumIndex = 0; coulumIndex < data.Columns.Count; ++coulumIndex)
                    {
                        int insertColumIndex = startColumIndex + coulumIndex;
                        row.CreateCell(insertColumIndex).SetCellValue(data.Rows[rowIndex][coulumIndex].ToString());
                    }
                }
                workbook.Write(fs); //写入到excel
                return data.Rows.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
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
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                //Colnum
                if (isColumnWritten == true)
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);

                        if (data.Columns[j].ExtendedProperties.ContainsKey("width"))
                        {
                            var width = Convert.ToInt32(data.Columns[j].ExtendedProperties["width"]);
                            sheet.SetColumnWidth(j, width);
                        }
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                //Row
                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var column = data.Columns[j];
                        var cellDataType = column.DataType;
                        var cellContent = data.Rows[i][j];

                        if (column.ExtendedProperties.ContainsKey("isPicture"))
                        {
                            if (!string.IsNullOrEmpty(cellContent.ToString()))
                            {
                                row.Height = Convert.ToInt16(column.ExtendedProperties["height"]);
                                AddCellPicture(sheet, workbook, cellContent.ToString(), count, j);
                            }
                        }
                        else
                        {
                            CreateCellValue(row.CreateCell(j), cellDataType, cellContent);
                        }
                    }
                    ++count;
                }
                //写入到excel
                workbook.Write(fs);

                return count;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellDataColumnType"></param>
        /// <param name="value"></param>
        private void CreateCellValue(ICell cell, Type cellDataColumnType, object value)
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
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="cellCount">总列数</param>
        /// <param name="startRow">第一行数据行号</param>
        /// <param name="startColumn">第一列开始索引</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, int cellCount = -1, int startRow = 0, int startColumn = 0)
        {
            ISheet sheet;
            DataTable data = new DataTable();

            bool isHSSFWordbook = false;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                if (fileName.IndexOf(".xlsx") > 0 || fileName.IndexOf(".xlsm") > 0) // 2007版本及以上版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                {
                    isHSSFWordbook = true;
                    workbook = new HSSFWorkbook(fs);

                }

                sheet = sheetName != null
                    ? (workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0))
                    : workbook.GetSheetAt(0);

                if (sheet == null) return data;

                IRow firstRow = sheet.GetRow(startRow);

                if (cellCount < 0) //如果小于0 就表示取总行数
                {
                    cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
                }
                if (cellCount > firstRow.LastCellNum)  //判断传入参数是否大于总列数，如果是，重新赋值。避免索引越线
                {
                    cellCount = firstRow.LastCellNum;
                }

                for (int i = startColumn; i < cellCount; ++i)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (cell != null)
                    {
                        string cellName = "cell" + i;
                        string cellValue = GetCellValue(cell, workbook, isHSSFWordbook);
                        if (cellValue != null)
                        {
                            cellName = cellValue;
                        }
                        if (data.Columns.Contains(cellName))
                        {
                            cellName = "cell" + i;
                        }
                        DataColumn column = new DataColumn(cellName);
                        data.Columns.Add(column);
                    }
                }

                startRow = startRow + 1;

                //最后一列的标号
                int rowCount = sheet.LastRowNum;
                for (int i = startRow; i <= rowCount; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue; //没有数据的行默认是null　　　　　　　

                    DataRow dataRow = data.NewRow();

                    int columnIndex = 0;
                    for (int j = startColumn; j < cellCount; ++j)
                    {
                        var cell = row.GetCell(j);
                        if (cell != null)//同理，没有数据的单元格都默认是null
                        {
                            dataRow[columnIndex] = GetCellValue(cell, workbook, isHSSFWordbook);
                        }
                        columnIndex++;
                    }
                    data.Rows.Add(dataRow);
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
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

                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    {
                        var anchor = new XSSFClientAnchor(0, 0, 0, 0, col, row, col + 1, row + 1);
                        var pict = (XSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    }
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
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
            DataTable data = new DataTable();
            bool isHssfWordbook = false;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0 || fileName.IndexOf(".xlsm", StringComparison.Ordinal) > 0) // 2007版本及以上版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0) // 2003版本
                {
                    isHssfWordbook = true;
                    workbook = new HSSFWorkbook(fs);
                }

                var sheet = sheetName != null
                    ? (workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0))
                    : workbook.GetSheetAt(0);

                if (sheet == null) return data;
                data = SheetToTable(sheet, cellCount, startRow, startColumn, isHssfWordbook, mergeTitleRow);
                Dispose();
            }
            catch (Exception ex)
            {
                FileLogHelper.WriteError(ex);
                Dispose();
                throw;
            }
            return data;
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
                        string cellValue = GetCellValue(cell, workbook, isHssfWordbook);
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
                            string cellValue = GetCellValue(cell, workbook, isHssfWordbook);
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
                        string value = GetCellValue(cell, workbook, isHssfWordbook);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
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

