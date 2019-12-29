using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MenuTools
{
    #region Open folder

    [MenuItem("FastFramework/Tools/Open Files/PersistentData Folder", priority = 50)]
    static void OpenPersistentData()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("FastFramework/Tools/Open Files/Assets Folder", priority = 53)]
    static void OpenAssets()
    {
        System.Diagnostics.Process.Start(Application.dataPath);
    }

    [MenuItem("FastFramework/Tools/Open Files/StreamingAssets Folder", priority = 55)]
    static void OpenStreamingAssets()
    {
        System.Diagnostics.Process.Start(Application.streamingAssetsPath);
    }

    #endregion

    #region Local data

    [MenuItem("FastFramework/Tools/Clear PlayerPrefs", priority = 25)]
    static void ClearPlayPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion

    #region Run Mode

    private static string[] debugMacroDef = {"AD_SDK", "FB_SDK", "FTDSdk", "IAP_SDK", "DEBUG_MODE"};
    // private static string[] releaseMacroDef = { "AD_SDK", "FB_SDK", "FTDSdk", "IAP_SDK" };
    // private static string editorMacroDef = string.Empty;

    [MenuItem("FastFramework/Tools/Runtime Mode/Debug", true, priority = 70)]
    static bool IsDebugRuntimeMode()
    {
        string[] def = EditorUserBuildSettings.activeScriptCompilationDefines;
        int len = def.Length;
        if (len == debugMacroDef.Length)
        {
            for (int i = 0; i < len; i++)
            {
                bool exist = false;
                for (int j = 0; j < len; j++)
                {
                    if (def[i] == debugMacroDef[j])
                    {
                        exist = true;
                        break;
                    }

                    if (!exist) return true;
                }
            }

            return false;
        }

        return true;
    }

    [MenuItem("FastFramework/Tools/Runtime Mode/Debug", false, priority = 70)]
    static void SetDebugRuntimeMode()
    {
        //EditorUserSettings.
        //EditorUserBuildSettings.activeScriptCompilationDefines = debugMacroDef;
    }

    [MenuItem("FastFramework/Tools/Runtime Mode/Release", priority = 73)]
    static void SetReleaseRuntimeMode()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("FastFramework/Tools/Runtime Mode/Debug", priority = 75)]
    static void SwitchRuntimeMode()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion

    #region Test

    [MenuItem("FastFramework/Tools/TransLocal", priority = 50)]
    static void TransLocal()
    {
//        LocalAssets LocalAssets = AssetDatabase.LoadAssetAtPath<LocalAssets>(
//            AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:" + typeof(LocalAssets).FullName)[0]));
//
//
//        var buffer = new StringBuilder();
//        //buffer.Append("KEY,");
//        for (var i = 0; i < LocalAssets.localItems.Count; i++)
//        {
//            buffer.Append("\"");
//            var localeItem = LocalAssets.localItems[i];
//            buffer.Append(localeItem.name);
//            buffer.Append("\"");
//            buffer.Append("\n");
//            //buffer.Append(i == LocalAssets.localItems.Count - 1 ? "\n" : ",");
//        }
//        buffer.Append("\n");
//        Localization.Instance.CurrentLanguage = SystemLanguage.English;
//        for (var i = 0; i < LocalAssets.localItems.Count; i++)
//        {
//            buffer.Append("\"");
//            var localeItem = LocalAssets.localItems[i];
//            buffer.Append(localeItem.Value);
//            buffer.Append("\"");
//            buffer.Append("\n");
//            //buffer.Append(i == LocalAssets.localItems.Count - 1 ? "\n" : ",");
//        }
//        buffer.Append("\n");
//        Localization.Instance.CurrentLanguage = SystemLanguage.ChineseSimplified;
//        for (var i = 0; i < LocalAssets.localItems.Count; i++)
//        {
//            buffer.Append("\"");
//            var localeItem = LocalAssets.localItems[i];
//            buffer.Append(localeItem.Value);
//            buffer.Append("\"");
//            buffer.Append("\n");
//            //buffer.Append(i == LocalAssets.localItems.Count - 1 ? "\n" : ",");
//        }


//        System.IO.File.WriteAllText(Application.dataPath + "/language.csv", buffer.ToString(),
//            System.Text.Encoding.UTF8);
    }

    #endregion
}