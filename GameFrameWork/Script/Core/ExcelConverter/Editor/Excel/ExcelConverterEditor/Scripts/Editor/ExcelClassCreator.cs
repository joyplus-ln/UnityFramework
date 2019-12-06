using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;


public class ExcelClassCreator
{
    private string m_ExcelSheetTypeAttribute = "";
    private string m_ManagerAttribute = "";
    private string m_ManagerGetData = "";
    private string m_ManagerInitData = "";
    private string m_ExcelDataTemplate;
    private string m_SavePath;
    private string m_TableClassName;
    private string m_TableNoteName;
    private string m_SheetName;
    private List<List<ICell>> m_ExcelData;

    private void CreateSheetElements(int sheetNum)
    {
        string templete =
            @"
    /// <summary>
    /// {0}
    /// </summary>
    public static int {1}_{2} = {3};
";
        m_ExcelSheetTypeAttribute += string.Format(templete, m_TableNoteName + "表", m_TableClassName, m_SheetName,
            sheetNum.ToString());
    }

    private void CreateManagerElements(int sheetNum)
    {
        string attributeTemplete =
            @"
    /// <summary>
    /// {0}
    /// </summary>
    private {1} {1}_{2};
    private Dictionary<string, {1}_Data> {1}_{2}_Dic = new Dictionary<string, {1}_Data>();
";
        m_ManagerAttribute += string.Format(attributeTemplete, m_TableNoteName + "表", m_TableClassName, m_SheetName);

        string getDataTemplete =
            @"
            case ""asset_{0}_{1}"":
                if (dataId == """")
                <-
                    o = {0}_{1};
                ->
                else
                <-
                    o = {0}_{1}_Dic[dataId];
                ->
                break;
";
        // @"
        //             case ""asset_{0}_{1}"":
        //                 if (dataId == -1)
        //                 <-
        //                     o = {0}_{1};
        //                 ->
        //                 else
        //                 <-
        //                     if (dataId >= 0 && dataId < {0}_{1}.dataList.Count)
        //                     <-
        //                         o = {0}_{1}.dataList[dataId];
        //                     ->
        //                     else
        //                     <-
        //                         Debug.LogWarning(""{0}_{1} is not contains id :"" + dataId);
        //                     ->
        //                 ->
        //                 break;
        // ";
        m_ManagerGetData += string.Format(getDataTemplete, m_TableClassName, m_SheetName);

        string initDataTemplete =
            @"
            case ""asset_{0}_{1}"":
                {0}_{1} = ({0})sheet;
                for (int i = 0; i < {0}_{1}.dataList.Count; i++)
                <-
                    if (!{0}_{1}_Dic.ContainsKey({0}_{1}.dataList[i].ID))
                    <-
                        {0}_{1}_Dic.Add({0}_{1}.dataList[i].ID, {0}_{1}.dataList[i]);
                    ->
                ->
                break;
";
        m_ManagerInitData += string.Format(initDataTemplete, m_TableClassName, m_SheetName);
    }

    private void CreateAttributes(List<List<ICell>> excelData)
    {
        List<ICell> attributes = excelData[0];
        List<ICell> types = excelData[1];
        List<ICell> notes = excelData[2];
        string templete =
            @"
    /// <summary>
    /// {0}
    /// </summary>
    public {1} {2};
";
        string attribute = "";
        for (int i = 0; i < attributes.Count; i++)
        {
            // object value = ExcelUtility.GetCellValue(attributes[i]);
            attribute += string.Format(templete, notes[i], types[i], attributes[i]);
        }

        m_ExcelDataTemplate = m_ExcelDataTemplate.Replace("__ATTRIBUTES__", attribute);
    }

    private void CreateInitialize(List<List<ICell>> excelData)
    {
        List<ICell> attributes = excelData[0];
        List<ICell> types = excelData[1];
        string templete =
            @"
        {0} = {1}(data[""{0}""]);";
        string attribute = "";
        for (int i = 0; i < attributes.Count; i++)
        {
            attribute += string.Format(templete, attributes[i],
                ExcelConvertUtility.GetConvertTypeString(types[i].StringCellValue));
        }

        m_ExcelDataTemplate = m_ExcelDataTemplate.Replace("__INITIALIZE__", attribute);
    }
    
    private void CreateProject(List<List<ICell>> excelData)
    {
        List<ICell> attributes = excelData[0];
        List<ICell> types = excelData[1];
        string templete =
            @"
        {0} = {0},";
        string attribute = "";
        for (int i = 0; i < attributes.Count; i++)
        {
            attribute += string.Format(templete, attributes[i],
                ExcelConvertUtility.GetConvertTypeString(types[i].StringCellValue));
        }

        m_ExcelDataTemplate = m_ExcelDataTemplate.Replace("__Project__", attribute);
    }

    private void CreateToString(List<List<ICell>> excelData)
    {
        List<ICell> attributes = excelData[0];
        string attributeStr = "\"";
        string values = "";
        for (int i = 0; i < attributes.Count; i++)
        {
            attributeStr += attributes[i] + "={" + i + "}" + (i == attributes.Count - 1 ? "\"," : ",");
            values += " " + attributes[i] + (i == attributes.Count - 1 ? "" : ",");
        }

        m_ExcelDataTemplate = m_ExcelDataTemplate.Replace("__TOSTRING__", attributeStr + values);
    }

    private void CreateBaseSheetDataClass()
    {
        string templete =
            @"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the FrameWork Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
//  Build Time：{0}
// ------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class BaseSheet<T> : ScriptableObject
<-
    public List<T> dataList = new List<T>();
->";
        templete = string.Format(templete, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        templete = templete.Replace("<-", "{");
        templete = templete.Replace("->", "}");

        EditorUtil.CreateFile(m_SavePath + "BaseSheetData.cs", Encoding.UTF8.GetBytes(templete));
    }

    private void CreateSheetTypeClass()
    {
        m_ExcelSheetTypeAttribute = "";
        string templete =
            @"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the FrameWork Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
//  Build Time：{0}
// ------------------------------------------------------------------------------
public class ExcelSheetType
<-
    __SHEET_TYPE__
->";
        templete = string.Format(templete, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        templete = templete.Replace("__SHEET_TYPE__", m_ExcelSheetTypeAttribute);
        templete = templete.Replace("<-", "{");
        templete = templete.Replace("->", "}");

        EditorUtil.CreateFile(m_SavePath + "ExcelSheetType.cs", Encoding.UTF8.GetBytes(templete));
    }

    private void CreateManagerClass(List<string> excelPaths, List<string> excelSelectedPaths, ExcelReader excelReader)
    {
        if (File.Exists(m_SavePath + "ExcelDataManager.cs"))
        {
            m_ManagerAttribute = "";
            m_ManagerGetData = "";
            m_ManagerInitData = "";

            Assembly assembly = Assembly.Load("Assembly-CSharp");
            Type classType = assembly.GetType("ExcelDataManager");
            MemberInfo[] memberInfos =
                classType.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            List<string> memberList = new List<string>();
            //反射的成员变量名数组
            for (int i = 0; i < memberInfos.Length; i++)
            {
                memberList.Add(memberInfos[i].Name);
            }

            //根据文件路径生成 成员变量名:注释信息 的键值对，方便后续还原操作
            Dictionary<string, string> pathNotePair = new Dictionary<string, string>();
            Dictionary<string, string> selectPathNotePair = new Dictionary<string, string>();
            for (int i = 0; i < excelPaths.Count; i++)
            {
                string path = excelPaths[i];
                string tableClassName = "";
                string tableNoteName = "";
                string[] tableName = Path.GetFileNameWithoutExtension(path).Split('_');
                tableClassName = tableName[0];
                if (tableName.Length > 1)
                {
                    tableNoteName = tableName[1];
                }

                Dictionary<string, List<List<ICell>>> sheets = excelReader.Load(path);
                foreach (KeyValuePair<string, List<List<ICell>>> keyPair in sheets)
                {
                    string sheetName = keyPair.Key;
                    pathNotePair[tableClassName + "_" + sheetName] = tableNoteName;
                    if (excelSelectedPaths.IndexOf(path) > -1)
                    {
                        selectPathNotePair[tableClassName + "_" + sheetName] = tableNoteName;
                    }
                }
            }

            int memberListCount = memberList.Count;
            if (memberListCount > pathNotePair.Count)
            {
                for (int i = memberListCount - 1; i >= 0; i--)
                {
                    bool needDelete = true;
                    foreach (KeyValuePair<string, string> keyPair in pathNotePair)
                    {
                        if (memberList[i].IndexOf(keyPair.Key) > -1)
                        {
                            needDelete = false;
                            break;
                        }
                    }

                    if (needDelete)
                        memberList.RemoveAt(i);
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> keyPair in pathNotePair)
                {
                    if (memberListCount > 0)
                    {
                        bool needAdd = false;
                        for (int i = memberListCount - 1; i >= 0; i--)
                        {
                            if (memberList.IndexOf(keyPair.Key) < 0 && selectPathNotePair.ContainsKey(keyPair.Key))
                            {
                                needAdd = true;
                                break;
                            }
                        }

                        if (needAdd)
                            memberList.Add(keyPair.Key);
                    }
                    else
                        memberList.Add(keyPair.Key);
                }
            }

            for (int i = 0; i < memberList.Count; i += 2)
            {
                string[] tableName = memberList[i].Split('_');
                m_TableClassName = tableName[0];
                if (tableName.Length > 1)
                {
                    m_SheetName = tableName[1];
                }

                m_TableNoteName = pathNotePair[m_TableClassName + "_" + m_SheetName];
                CreateManagerElements(i);
            }
        }

        string templete =
            @"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the FrameWork Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
//  Build Time：{0}
// ------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class ExcelDataManager
<-
    __ATTRIBUTES__
    public object GetData(string sheetType, string dataId = """")
    <-
        object o = null;
        switch (sheetType)
        <-
            __GET_DATA__
        ->
        return o;
    ->

    public void Init(string sheetType, object sheet)
    <-
        switch (sheetType)
        <-
            __INIT_DATA__
        ->
    ->
->";
        templete = string.Format(templete, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        templete = templete.Replace("__ATTRIBUTES__", m_ManagerAttribute);
        templete = templete.Replace("__GET_DATA__", m_ManagerGetData);
        templete = templete.Replace("__INIT_DATA__", m_ManagerInitData);
        templete = templete.Replace("<-", "{");
        templete = templete.Replace("->", "}");

        m_ManagerAttribute = "";
        m_ManagerGetData = "";
        m_ManagerInitData = "";
        EditorUtil.CreateFile(m_SavePath + "ExcelDataManager.cs", Encoding.UTF8.GetBytes(templete));
    }

    private void CreateDataClass()
    {
        m_ExcelDataTemplate =
            @"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the FrameWork Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
//  Build Time：{0}
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// {1}
/// </summary>
public partial class {2}_Data : ScriptableObject
<-
    __ATTRIBUTES__
    public int GetRowsCount()
    <-
        return {3};
    ->
    public int GetColumnsCount()
    <-
        return {4};
    ->
    public void Init(Dictionary<string, object> data)
    <-
        __INITIALIZE__
    ->
    override public string ToString()
    <-
        return string.Format(__TOSTRING__);
    ->
    public {5}_Data Project()
    <-
        return new {6}_Data
        <-
            __Project__
        ->;
    ->
->";
        CreateAttributes(m_ExcelData);
        CreateInitialize(m_ExcelData);
        CreateProject(m_ExcelData);
        m_ExcelDataTemplate = string.Format(m_ExcelDataTemplate, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            m_TableNoteName + "表数据", m_TableClassName, m_ExcelData.Count, m_ExcelData[0].Count,m_TableClassName,m_TableClassName);
        m_ExcelDataTemplate = m_ExcelDataTemplate.Replace("<-", "{");
        m_ExcelDataTemplate = m_ExcelDataTemplate.Replace("->", "}");
        CreateToString(m_ExcelData);

        EditorUtil.CreateFile(m_SavePath + "ExcelDatas/" + m_TableClassName + "_Data.cs",
            Encoding.UTF8.GetBytes(m_ExcelDataTemplate));
    }

    private void CreatePartDataClass()
    {
      string  partClass =
            @"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the FrameWork Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
//  Build Time：{0}
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// {1}
/// </summary>
public partial class {2}_Data
<-
   
->";
      partClass = string.Format(partClass, DateTime.Now, m_TableClassName,m_TableClassName);
      partClass = partClass.Replace("<-", "{");
      partClass = partClass.Replace("->", "}");
      if (!EditorUtil.FileExit(m_SavePath + "PartDatas/" + m_TableClassName + "_Data.Extra.cs"))
      {
          EditorUtil.CreateFile(m_SavePath + "PartDatas/" + m_TableClassName + "_Data.Extra.cs",
              Encoding.UTF8.GetBytes(partClass));
      }

    }
    
    private void CreateSheetClass()
    {
        string templete =
            @"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the FrameWork Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
//  Build Time：{0}
// ------------------------------------------------------------------------------

using System.Collections.Generic;

/// <summary>
/// {1}
/// </summary>
public class {2} : BaseSheet<{2}_Data>
<-
->";

        templete = string.Format(templete, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), m_TableNoteName + "表",
            m_TableClassName);
        templete = templete.Replace("<-", "{");
        templete = templete.Replace("->", "}");

        EditorUtil.CreateFile(m_SavePath + "ExcelDatas/" + m_TableClassName + ".cs", Encoding.UTF8.GetBytes(templete));
    }

    public void CreateManager(List<string> excelPaths, List<string> excelSelectedPaths, ExcelReader excelReader)
    {
        CreateBaseSheetDataClass();
        // CreateSheetTypeClass();
        CreateManagerClass(excelPaths, excelSelectedPaths, excelReader);
    }

    public void Create(int sheetNum, string savePath, string tableClassName, string tableNoteName, string sheetName,
        List<List<ICell>> excelData)
    {
        m_SavePath = savePath;
        m_TableClassName = tableClassName;
        m_TableNoteName = tableNoteName;
        m_SheetName = sheetName;
        m_ExcelData = excelData;
        CreateSheetElements(sheetNum);
        CreateManagerElements(sheetNum);
        CreatePartDataClass();
        CreateDataClass();
        CreateSheetClass();
    }
}