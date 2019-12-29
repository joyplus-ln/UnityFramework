
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

    public class ExcelReader
    {
        public Dictionary<string, List<List<ICell>>> Load(string InExcelPath)
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
                        
                        Dictionary<string, List<List<ICell>>> sheetDic = new Dictionary<string, List<List<ICell>>>();
                        for (int i = 0; i < workbook.NumberOfSheets; i++)
                        {
                            ISheet sheet = workbook.GetSheetAt(i);
                            int rowCount = sheet.LastRowNum + 1;
                            List<List<ICell>> rowCells = new List<List<ICell>>();
                            for (int j = 0; j < rowCount; j++)
                            {
                                IRow row = sheet.GetRow(j);
                                int columnCount = row.LastCellNum;
                                List<ICell> columnCells = new List<ICell>();

                                for (int k = 0; k < columnCount; k++)
                                {
                                    columnCells.Add(row.GetCell(k));
                                }
                                rowCells.Add(columnCells);
                            }
                            sheetDic.Add(sheet.SheetName, rowCells);
                        }
                        workbook.Close();
                        stream.Close();
                        return sheetDic;
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
    }
