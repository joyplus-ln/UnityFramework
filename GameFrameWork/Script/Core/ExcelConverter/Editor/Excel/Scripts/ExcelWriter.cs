using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ExcelConverter.Tools;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;

namespace ExcelConverter.Excel.Editor
{
    public class ExcelWriter
    {
        public static void WriteExcelData(string path, ExcelData[] dataList)
        {
            //创建工作薄  
            IWorkbook book;
            string extension = Path.GetExtension(path);
            //根据指定的文件格式创建对应的类
            if (extension.Equals(".xls"))
            {
                book = new HSSFWorkbook();
            }
            else
            {
                book = new XSSFWorkbook();
            }

            for (int i = 0; i < dataList.Length; i++)
            {
                ExcelData data = dataList[i];
                ISheet sheet = book.GetSheet(data.SheetName);
                if (sheet == null)
                {
                    sheet = book.CreateSheet(data.SheetName);
                }

                for (int j = 0; j < data.HeadRowLen; j++)
                {
                    IRow row = sheet.CreateRow(j);
                    for (int k = 0; k < data.DataColumnLen; k++)
                    {
                        row.CreateCell(k).SetCellValue(data.Head[j][k].ToString());
                    }
                }

                for (int j = data.HeadRowLen; j < data.HeadRowLen + data.BodyRowLen; j++)
                {
                    IRow row = sheet.CreateRow(j);
                    for (int k = 0; k < data.DataColumnLen; k++)
                    {
                        row.CreateCell(k).SetCellValue(data.Body[j - data.HeadRowLen][k].ToString());
                    }
                }
            }

            try
            {
                FileStream fs = File.OpenWrite(path);
                book.Write(fs);//向打开的这个Excel文件中写入表单并保存。  
                fs.Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }



        }
    }
}
