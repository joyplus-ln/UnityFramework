using ExcelConverter.Tools;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using FastBundle;
using UnityEditor;

namespace ExcelConverter.Excel.Editor
{
    public class ExcelReader
    {
        public static ExcelData[] GetSingleExcelData(string InExcelPath)
        {
            if (File.Exists(InExcelPath))
            {
                FileInfo info = new FileInfo(InExcelPath);
                if (info.Exists && info.Name[0] != '~'
                                && (info.Extension.Equals(".xlsx") || info.Extension.Equals(".xls")))
                {
                    using (FileStream stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IWorkbook book;

                        if (info.Extension.Equals(".xlsx")) book = new XSSFWorkbook(stream);
                        else book = new HSSFWorkbook(stream);

                        int sheetCount = book.NumberOfSheets;
                        var dataList = new List<ExcelData>(sheetCount);

                        for (int i = 0; i < sheetCount; i++)
                        {
                            ISheet sheet = book.GetSheetAt(i);

                            int rowCount = sheet.LastRowNum + 1;

                            if (rowCount < 2) continue; //no content

                            ExcelData data = new ExcelData
                            {
                                SheetName = sheet.SheetName.Equals("Sheet1") || sheet.SheetName.Equals("工作表1")
                                    ? info.Name.Replace(info.Extension, string.Empty)
                                    : sheet.SheetName
                            };

                            IRow[] headRow =
                            {
                                sheet.GetRow(ExcelConfig.NAME_ROW_INDEX_I), //Name
                                sheet.GetRow(ExcelConfig.TYPE_ROW_INDEX_I), //Type
                                sheet.GetRow(ExcelConfig.DESCRIBES_ROW_INDEX_I) //Describes
                            };

                            int colCount = headRow[ExcelConfig.NAME_ROW_INDEX_I].LastCellNum >
                                           headRow[ExcelConfig.TYPE_ROW_INDEX_I].LastCellNum
                                ? headRow[ExcelConfig.TYPE_ROW_INDEX_I].LastCellNum
                                : headRow[ExcelConfig.NAME_ROW_INDEX_I].LastCellNum;

                            if (colCount > 0)
                            {
                                data.DataColumnLen = colCount;

                                data.HeadRowLen = 3;
                                data.Head = new ICell[data.HeadRowLen][];

                                data.Head[0] = new ICell[colCount];
                                data.Head[1] = new ICell[colCount];
                                data.Head[2] = new ICell[colCount];

                                //Property comment can be empty.
                                bool isDescribeFilled = headRow[ExcelConfig.DESCRIBES_ROW_INDEX_I] != null;
                                for (int j = 0; j < colCount; j++)
                                {
                                    data.Head[0][j] = headRow[0].GetCell(j);
                                    data.Head[1][j] = headRow[1].GetCell(j);
                                    data.Head[2][j] = isDescribeFilled ? headRow[2].GetCell(j) : null;
                                }

                                if (rowCount > ExcelConfig.CONTENT_START_ROW_INDEX_I)
                                {
                                    int length = rowCount - ExcelConfig.CONTENT_START_ROW_INDEX_I;
                                    List<ICell[]> content = new List<ICell[]>(length);
                                    for (int j = 0, m = ExcelConfig.CONTENT_START_ROW_INDEX_I; j < length; ++j, ++m)
                                    {
                                        var row = sheet.GetRow(m);

                                        if (null == row) continue;

                                        ICell[] cells = new ICell[colCount];
                                        for (int k = 0; k < colCount; ++k)
                                        {
                                            cells[k] = row.GetCell(k);
                                        }

                                        content.Add(cells);
                                    }

                                    data.Body = content.ToArray();
                                    data.BodyRowLen = content.Count;
                                }

                                if (data.CheckData()) dataList.Add(data);
                            }
                            else
                            {
                                Logger.L(LogType.Error,"",info.Name + "-" + sheet.SheetName +
                                         " property name and type number does not match.");
                            }


                            //else if(0 != rowCount) EditorUtility.DisplayDialog("Error", info.Name + "-" + sheet.SheetName + " missing basic configuration information, property name and type.", "Ok");
                        }

                        book.Close();
                        stream.Close();

                        return dataList.ToArray();
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error",
                    "Failed to find excel file in this directory.\nPath = " + InExcelPath, "Ok");
            }

            return null;
        }

        public static List<List<ICell>> GetOriginalExcelData(string InExcelPath, int InSheetIndex)
        {
            if (File.Exists(InExcelPath))
            {
                FileInfo info = new FileInfo(InExcelPath);
                if (info.Exists && info.Name[0] != '~'
                                && (info.Extension.Equals(".xlsx") || info.Extension.Equals(".xls")))
                {
                    using (FileStream stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IWorkbook workbook;

                        if (info.Extension.Equals(".xlsx")) workbook = new XSSFWorkbook(stream);
                        else workbook = new HSSFWorkbook(stream);

                        ISheet sheet = workbook.GetSheetAt(InSheetIndex);

                        if (null == sheet)
                        {
                            EditorUtility.DisplayDialog("Error",
                                "Failed to find sheet at " + InSheetIndex + " in this Excel.\nPath = " + InExcelPath,
                                "Ok");
                        }
                        else
                        {
                            int rowCount = sheet.LastRowNum + 1;
                            List<List<ICell>> result = new List<List<ICell>>(rowCount);

                            for (int i = 0; i < rowCount; i++)
                            {
                                IRow row = sheet.GetRow(i);
                                int columnCount = row.LastCellNum + 1;
                                List<ICell> columnCells = new List<ICell>(columnCount);

                                for (int j = 0; j < columnCount; j++)
                                {
                                    columnCells.Add(row.GetCell(j));
                                }

                                result.Add(columnCells);
                            }

                            workbook.Close();
                            stream.Close();

                            return result;
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Error",
                        "Failed to find excel file in this directory.\nPath = " + InExcelPath, "Ok");
                }
            }
            return null;
        }
    }
}