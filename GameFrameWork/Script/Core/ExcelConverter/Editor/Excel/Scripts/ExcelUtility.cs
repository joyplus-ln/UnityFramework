using System;
using NPOI.SS.UserModel;
using UnityEngine;

namespace ExcelConverter.Excel.Editor
{
    public static class ExcelUtility
    {
        public static bool IsNullOrEmpty(ICell InCell)
        {
            if (InCell == null) return false;
            if (InCell.CellType == CellType.String) return string.IsNullOrEmpty(InCell.StringCellValue);
            return InCell.CellType == CellType.Numeric;
        }
    }
}