using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelConverter.Excel.Editor
{
    public class ExcelData
    {
        /// <summary>
        /// name of excel or sheet.
        /// </summary>
        public string SheetName;

        /// <summary>
        /// The number of rows in the data header, always 3.
        /// 0. Property definitions
        /// 1. Property types
        /// 2. Property comments
        /// </summary>
        public int HeadRowLen;

        /// <summary>
        /// The number of rows in the data body.
        /// </summary>
        public int BodyRowLen;

        /// <summary>
        /// The number of column in the data.
        /// </summary>
        public int DataColumnLen;

        /// <summary>
        /// Property definitions, types, and comments.
        /// </summary>
        public ICell[][] Head;

        /// <summary>
        /// Data array.
        /// </summary>
        public ICell[][] Body;

        /// <summary>
        /// Check and correct data.
        /// </summary>
        /// <returns>Is the data available?</returns>
        public bool CheckData()
        {
            if (string.IsNullOrEmpty(SheetName)) return false;

            if (HeadRowLen == 0 || DataColumnLen == 0) return false;

            if (Head == null || Head[0] == null || Head[1] == null) return false;

            //head data
            List<List<ICell>> correctHead = new List<List<ICell>>(HeadRowLen)
            {
                new List<ICell>(DataColumnLen),         //Property definitions
                new List<ICell>(DataColumnLen),         //Property types
                new List<ICell>(DataColumnLen)          //Property comments
            };

            List<int> correctIndex = new List<int>(DataColumnLen);

            for (int i = 0; i < DataColumnLen; i++)
            {
                if (Head[0][i] == null || Head[0][i].CellType != CellType.String) continue;
                if (Head[1][i] == null || Head[1][i].CellType != CellType.String) continue;

                correctHead[0].Add(Head[0][i]);
                correctHead[1].Add(Head[1][i]);
                correctHead[2].Add(Head[2][i]);

                correctIndex.Add(i);
            }

            Head[0] = correctHead[0].ToArray();
            Head[1] = correctHead[1].ToArray();
            Head[2] = correctHead[2].ToArray();
            if (Head[0].Length != correctHead[0].Count)
            {
                Debug.LogError("Some columns are ignored because of missing names or types. Please compare the generated data with Excel to view specific information.");
            }

            List<List<ICell>> correctBody = new List<List<ICell>>(BodyRowLen);
            int count = correctIndex.Count;
            for (int i = 0; i < BodyRowLen; i++)
            {
                if (Body[i] == null) continue;
                List<ICell> rowBody = new List<ICell>(count);
                int invalidRowCount = -1;
                for (int j = 0; j < count; j++)
                {
                    int index = correctIndex[j];
                    if (Body[i][index] == null) ++invalidRowCount;
                    else
                    {
                        switch (Body[i][index].CellType)
                        {
                            case CellType.Numeric:
                            case CellType.Formula:
                                invalidRowCount += Math.Abs(Body[i][index].NumericCellValue - 0) < double.Epsilon ? 1 : 0;
                                break;
                            case CellType.String:
                                invalidRowCount += string.IsNullOrEmpty(Body[i][index].StringCellValue) ? 1 : 0;
                                break;
                            default:
                                invalidRowCount += 1;
                                break;
                        }
                    }

                    rowBody.Add(Body[i][index]);
                }

                if (invalidRowCount != count) correctBody.Add(rowBody);
            }

            int correctCount = correctBody.Count;
            if (correctCount != BodyRowLen)
                Debug.LogError("Some blank lines are ignored, please compare the generated data with Excel to view specific information!");

            Body = new ICell[correctCount][];

            for (int i = 0; i < correctCount; i++)
            {
                Body[i] = correctBody[i].ToArray();
            }

            return true;
        }

        public override string ToString()
        {
            string info = "Excel or Sheet name = " + SheetName;

            for (int j = 0; j < HeadRowLen; ++j)
            {
                string headStr = string.Empty;
                for (int k = 0; k < DataColumnLen; ++k)
                {
                    headStr += Head[j][k] + ", ";
                }
                info += "\n" + headStr;
            }

            for (int j = 0; j < BodyRowLen; j++)
            {
                string bodyStr = string.Empty;
                for (int k = 0; k < DataColumnLen; k++)
                {
                    bodyStr += Body[j][k] + ", ";
                }
                info += "\n" + bodyStr;
            }
            return info;
        }
    }
}


