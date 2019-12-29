using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ConfigMenu
{
    [MenuItem ("Fast/Config/OpenConfigWindow")]
    static void OpenConfigWindow()
    {
        EditorWindow.GetWindow<ExcelWindow>();
    }
}
