using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;


public class ExcelConvertUtility
{
    public static bool IsNullOrEmpty(ICell InCell)
    {
        if (InCell == null) return false;
        if (InCell.CellType == CellType.String) return string.IsNullOrEmpty(InCell.StringCellValue);
        return InCell.CellType == CellType.Numeric;
    }

    public static object GetCellValue(ICell cell)
    {
        if (cell == null)
            return "";
        else
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula:
                case CellType.Numeric:
                    if (HSSFDateUtil.IsCellDateFormatted(cell))
                        return cell.DateCellValue;
                    else
                        return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Unknown:
                    return cell.RichStringCellValue;
                default:
                    return cell.StringCellValue;
            }
    }

    public static string GetConvertTypeString(string type)
    {
        switch (type)
        {
            case "byte":
                return "Convert.ToByte";
            case "int":
                return "Convert.ToInt32";
            case "short":
                return "Convert.ToInt16";
            case "long":
                return "Convert.ToInt64";
            case "bool":
                return "Convert.ToBoolean";
            case "string":
                return "Convert.ToString";
            case "decimal":
                return "Convert.ToDecimal";
            case "double":
                return "Convert.ToDouble";
            case "float":
                return "Convert.ToSingle";
            case "DateTime":
                return "Convert.ToDateTime";
            case "int[]":
                return "ObjectConvertUtility.ConvertIntArraryValue";
            case "string[]":
                return "ObjectConvertUtility.ConvertStringArraryValue";
            default:
                return "Convert.ToString";
        }
    }

    public static object GetConvertType(string type, object value)
    {
        switch (type)
        {
            case "byte":
                return Convert.ToByte(value);
            case "int":
                return Convert.ToInt32(value);
            case "short":
                return Convert.ToInt16(value);
            case "long":
                return Convert.ToInt64(value);
            case "bool":
                return Convert.ToBoolean(value);
            case "string":
                return Convert.ToString(value);
            case "decimal":
                return Convert.ToDecimal(value);
            case "double":
                return Convert.ToDouble(value);
            case "float":
                return Convert.ToSingle(value);
            case "DateTime":
                return Convert.ToDateTime(value);
            case "int[]":
                return ObjectConvertUtility.ConvertIntArraryValue(value);
            case "string[]":
                return ObjectConvertUtility.ConvertStringArraryValue(value);
            default:
                return Convert.ToString(value);
        }
    }

    
}