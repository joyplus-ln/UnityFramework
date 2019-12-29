using NPOI.SS.UserModel;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System;

public class ExcelWindow : EditorWindow
{
    private string m_ExcelOpenPathKey = "ExcelOpenPathKey";
    private string m_ExcelOpenPath;
    private string m_ExcelDataPathKey = "ExcelDataPathKey";
    private string m_ExcelDataPath;
    private string m_SelectedClearFolderKey = "SelectedClearFolderKey";
    private bool m_SelectedClearFolder;
    private string m_ExcelClassPath;
    private Vector2 m_SheetDataScrollPos;
    private ExcelReader m_ExcelReader;
    private ExcelClassCreator m_ExcelClassCreator;
    private List<string> m_ExcelPaths;
    private List<string> m_ExcelSelectedPaths;
    private Dictionary<string, string> m_FilterRes;

    private void OnEnable()
    {
        m_ExcelClassPath = Application.dataPath + "/GenExcelData/Scripts/";
        m_ExcelOpenPath = EditorPrefs.GetString(m_ExcelOpenPathKey);
        m_ExcelOpenPath = m_ExcelOpenPath == "" ? Application.dataPath : m_ExcelOpenPath;
        m_ExcelDataPath = EditorPrefs.GetString(m_ExcelDataPathKey);
        m_ExcelDataPath = m_ExcelDataPath == "" ? Application.dataPath + "/AssetPackage/Config/" : m_ExcelDataPath;
        m_SelectedClearFolder = EditorPrefs.GetBool(m_SelectedClearFolderKey);
        m_FilterRes = new Dictionary<string, string>
        {
            {Application.dataPath.Remove(Application.dataPath.LastIndexOf("/")), ".xlsx,.xls"}
        };

        m_ExcelReader = new ExcelReader();
        m_ExcelClassCreator = new ExcelClassCreator();
        m_ExcelPaths = new List<string>();
        m_ExcelSelectedPaths = new List<string>();
        OpenExcelFolder();
    }

    private void OpenExcelFolder()
    {
        ExcelEditorUtils.RecursivePath(m_ExcelOpenPath, m_FilterRes, m_ExcelPaths);
        for (int i = 0; i < m_ExcelPaths.Count; i++)
        {
            if (EditorPrefs.GetBool(m_ExcelPaths[i]))
            {
                m_ExcelSelectedPaths.Add(m_ExcelPaths[i]);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private bool Filter(string path)
    {
        if (path.Contains(".meta") || path.Contains("~")) return false;
        if (path.Contains(".xlsx") || path.Contains(".xls"))
        {
            string excelName = path.Replace(m_ExcelOpenPath, "");
            EditorUtility.DisplayProgressBar("Load Excel", string.Format("Loading ... {0}", excelName), 100);
            return true;
        }

        return false;
    }

    private void ShowDirectoryArea()
    {
        GUIStyle helpBoxStyle = EditorStyleUtils.GetHelpBoxStyle();
        EditorGUILayout.BeginVertical(helpBoxStyle);
        {
            GUIStyle textFieldStyle = EditorStyleUtils.GetTextFieldStyle();
            textFieldStyle.fontSize = 13;
            textFieldStyle.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Export Class : " + m_ExcelClassPath.Replace(Application.dataPath, "Assets"),
                    textFieldStyle, GUILayout.Height(30));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.TextField(m_ExcelOpenPath, textFieldStyle, GUILayout.Height(30));
                // GUI.color = Color.green;
                if (GUILayout.Button("Load Excel Path", GUILayout.Width(120), GUILayout.Height(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Select Excel Path", "", "");
                    if (path != "")
                    {
                        m_ExcelOpenPath = path + "/";
                        EditorPrefs.SetString(m_ExcelOpenPathKey, m_ExcelOpenPath);
                        m_ExcelSelectedPaths.Clear();
                        m_ExcelPaths.Clear();
                        OpenExcelFolder();
                        EditorGUIUtility.ExitGUI();
                    }
                }

                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.TextField(m_ExcelDataPath, textFieldStyle, GUILayout.Height(30));
                // GUI.color = Color.green;
                if (GUILayout.Button("Export Data Path", GUILayout.Width(120), GUILayout.Height(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Select Data Path", "", "");
                    if (path != "")
                    {
                        m_ExcelDataPath = path + "/";
                        EditorPrefs.SetString(m_ExcelDataPathKey, m_ExcelDataPath);
                    }
                }

                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                // GUI.color = Color.green;
                if (m_SelectedClearFolder !=
                    EditorGUILayout.ToggleLeft("Clear Folder", m_SelectedClearFolder, GUILayout.Height(20)))
                {
                    m_SelectedClearFolder = !m_SelectedClearFolder;
                    EditorPrefs.SetBool(m_SelectedClearFolderKey, m_SelectedClearFolder);
                }

                if (GUILayout.Button(EditorGUIUtility.FindTexture("Refresh"), GUILayout.Width(30),
                    GUILayout.Height(30)))
                {
                    m_ExcelSelectedPaths.Clear();
                    m_ExcelPaths.Clear();
                    OpenExcelFolder();
                    EditorGUIUtility.ExitGUI();
                }

                if (GUILayout.Button("Select All", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    for (int i = 0; i < m_ExcelPaths.Count; i++)
                    {
                        string path = m_ExcelPaths[i];
                        m_ExcelSelectedPaths.Add(path);
                        EditorPrefs.SetBool(path, true);
                    }
                }

                if (GUILayout.Button("Unselect All", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    for (int i = 0; i < m_ExcelPaths.Count; i++)
                    {
                        string path = m_ExcelPaths[i];
                        m_ExcelSelectedPaths.Remove(path);
                        EditorPrefs.SetBool(path, false);
                    }
                }

                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private void ShowExcelListArea()
    {
        m_SheetDataScrollPos = EditorGUILayout.BeginScrollView(m_SheetDataScrollPos);
        GUIStyle helpBoxStyle = EditorStyleUtils.GetHelpBoxStyle();
        EditorGUILayout.BeginVertical(helpBoxStyle);
        {
            for (int i = 0; i < m_ExcelPaths.Count; i++)
            {
                string path = m_ExcelPaths[i];
                string excelName = m_ExcelPaths[i].Replace(m_ExcelOpenPath, "");
                bool selected = m_ExcelSelectedPaths.Contains(path);
                // GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                {
                    if (selected != EditorGUILayout.ToggleLeft(excelName, selected, GUILayout.Height(20)))
                    {
                        if (selected)
                        {
                            m_ExcelSelectedPaths.Remove(path);
                        }
                        else
                        {
                            m_ExcelSelectedPaths.Add(path);
                        }

                        EditorPrefs.SetBool(path, !selected);
                    }

                    if (GUILayout.Button("Review", GUILayout.Width(60), GUILayout.Height(20)))
                    {
                        EditorWindow.GetWindow<ExcelExporter>().titleContent =
                            new GUIContent(Path.GetFileNameWithoutExtension(path));
                        EditorWindow.GetWindow<ExcelExporter>().sheets = m_ExcelReader.Load(path);
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void ShowExecuteArea()
    {
        GUIStyle helpBoxStyle = EditorStyleUtils.GetHelpBoxStyle();
        GUIStyle textFieldStyle = EditorStyleUtils.GetTextFieldStyle();
        textFieldStyle.fontSize = 13;
        textFieldStyle.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.BeginVertical(helpBoxStyle);
        {
            // GUI.color = Color.green;
            EditorGUILayout.BeginHorizontal();
            {
                // GUI.color = Color.green;
                if (GUILayout.Button("Create Excel Class", GUILayout.Height(30)))
                {
                    if (m_SelectedClearFolder)
                        EditorUtils.DeleteDirectory(m_ExcelClassPath);

                    int sheetIndex = 0;
                    int count = m_ExcelSelectedPaths.Count;
                    for (int i = 0; i < count; i++)
                    {
                        string path = m_ExcelSelectedPaths[i];
                        string tableClassName = "";
                        string tableNoteName = "";
                        string[] tableName = Path.GetFileNameWithoutExtension(path).Split('_');
                        tableClassName = tableName[0];
                        if (tableName.Length > 1)
                        {
                            tableNoteName = tableName[1];
                        }

                        Dictionary<string, List<List<ICell>>> sheets = m_ExcelReader.Load(path);
                        foreach (KeyValuePair<string, List<List<ICell>>> keyPair in sheets)
                        {
                            string sheetName = keyPair.Key;
                            EditorUtility.DisplayProgressBar("Info",
                                "Create Excel Class : " + (i + 1) + "/" + count + " " + tableClassName + ".cs",
                                ((i + 1) / count) * 100);
                            m_ExcelClassCreator.Create(sheetIndex, m_ExcelClassPath, tableClassName, tableNoteName,
                                sheetName, keyPair.Value);
                            sheetIndex++;
                        }
                    }

                    if (count > 0)
                    {
                        m_ExcelClassCreator.CreateManager(m_ExcelPaths, m_ExcelSelectedPaths, m_ExcelReader);
                        AssetDatabase.Refresh();
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Success", "Create excel class successfully .", "OK");
                        EditorGUIUtility.ExitGUI();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Worning", "Select some excel files(.xls or .xlsx) , please .",
                            "OK");
                    }
                }

                if (GUILayout.Button("Create Excel Data", GUILayout.Height(30)))
                {
                    if (m_SelectedClearFolder)
                        EditorUtils.DeleteDirectory(m_ExcelDataPath);

                    Directory.CreateDirectory(m_ExcelDataPath);
                    Assembly assembly = Assembly.Load("Assembly-CSharp");
                    for (int i = 0; i < m_ExcelSelectedPaths.Count; i++)
                    {
                        string path = m_ExcelSelectedPaths[i];
                        string[] tableName = Path.GetFileNameWithoutExtension(path).Split('_');
                        string tableClassName = tableName[0];
                        Dictionary<string, List<List<ICell>>> sheets = m_ExcelReader.Load(path);
                        foreach (KeyValuePair<string, List<List<ICell>>> keyPair in sheets)
                        {
                            string sheetName = keyPair.Key;

                            Type tableType = assembly.GetType(tableClassName);
                            UnityEngine.Object container = (UnityEngine.Object) Activator.CreateInstance(tableType);
                            FieldInfo dataList = tableType.GetField("dataList");

                            Type dataType = assembly.GetType(tableClassName + "_Data");
                            var modelList =
                                Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] {dataType}));

                            AssetDatabase.CreateAsset(container,
                                m_ExcelDataPath.Replace(Application.dataPath, "Assets") + tableClassName + "_" +
                                sheetName + ".asset");

                            List<ICell> nameRow = keyPair.Value[0];
                            List<ICell> typeRow = keyPair.Value[1];
                            List<ICell> noteRow = keyPair.Value[2];
                            for (int row = 3; row < keyPair.Value.Count; row++)
                            {
                                UnityEngine.Object data = (UnityEngine.Object) Activator.CreateInstance(dataType);
                                List<ICell> contentRow = keyPair.Value[row];
                                for (int column = 0; column < nameRow.Count; column++)
                                {
                                    FieldInfo fieldInfo = dataType.GetField(nameRow[column].StringCellValue);
                                    object value = ExcelConvertUtility.GetCellValue(contentRow[column]);
                                    Debug.Log(tableClassName + "_" + sheetName + "_" + row + "_" + column + "_" +
                                              typeRow[column] + "_" + typeRow[column].StringCellValue);
                                    fieldInfo.SetValue(data,
                                        ExcelConvertUtility.GetConvertType(typeRow[column].StringCellValue, value));
                                }

                                var addMethod = modelList.GetType().GetMethod("Add");
                                addMethod.Invoke(modelList, new object[] {data});
                                dataList.SetValue(container, modelList);

                                data.name = dataType.ToString();
                                AssetDatabase.AddObjectToAsset(data,
                                    m_ExcelDataPath.Replace(Application.dataPath, "Assets") + tableClassName + "_" +
                                    sheetName + ".asset");
                            }

                            EditorUtility.SetDirty(container);
                            EditorUtility.DisplayProgressBar("Info",
                                "Create Excel Data : " + (i + 1) + "/" + m_ExcelSelectedPaths.Count + " " +
                                tableClassName + "_" + sheetName, (i / m_ExcelSelectedPaths.Count) * 100);
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Success", "Create excel data successfully .", "OK");
                    EditorGUIUtility.ExitGUI();
                }

                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    void OnGUI()
    {
        titleContent = new GUIContent("ExcelConverter");
        // maxSize = new Vector2(400,1000);
        // bool value = false;

        // GUILayout.Label("Select Platform");
        // GUILayout.SelectionGrid(1, new string[] { "43", "d" }, 1);
        // GUILayout.Space(20);

        // //GUILayout.BeginArea(new Rect(10, 200, 300, 600));

        // GUILayout.BeginHorizontal();
        // // CheckBox("CreateExcelClass", "Create Excel Class");

        EditorGUILayout.BeginVertical();
        {
            ShowDirectoryArea();
            ShowExcelListArea();
            ShowExecuteArea();
        }
        EditorGUILayout.EndVertical();
    }
}