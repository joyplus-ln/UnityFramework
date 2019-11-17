using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 批量移动资源功能
    /// </summary>
    public class MoveAssetEditor : CheckerPluginEditor
    {
        public static Object targetPath = null;

        public static void Init(List<Object> objects)
        {
            GetWindow<MoveAssetEditor>();
            objectList = objects;
        }

        void OnGUI()
        {
            targetPath = EditorGUILayout.ObjectField(targetPath, typeof(UnityEngine.Object), true);

            if (GUILayout.Button("移动到上面槽中目录"))
            {
                MoveAsset();
            }

            ShowList();
        }

        public static void MoveAsset()
        {
            if (objectList == null || objectList.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "当前无选中内容", "OK");
                return;
            }
            if (targetPath == null || !ResourceCheckerHelper.isFolder(AssetDatabase.GetAssetPath(targetPath)))
            {
                EditorUtility.DisplayDialog("提示", "请将要目标文件夹拖入槽内", "OK");
                return;
            }
            string path = AssetDatabase.GetAssetPath(targetPath);
            if (!EditorUtility.DisplayDialog("提示", "选中的文件将被移动到" + path + "文件夹内，请确认", "OK", "Cancle"))
            {
                return;
            }
            List<string> errorList = new List<string>();
            foreach (var obj in objectList)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string assetName = System.IO.Path.GetFileName(assetPath);
                string newAssetPath = path + "/" + assetName;
                if (assetPath == newAssetPath)
                {
                    Debug.Log("Dest path has the same name resource : " + assetPath);
                    errorList.Add(assetPath);
                    continue;
                }
                AssetDatabase.MoveAsset(assetPath, newAssetPath);
            }
            if (errorList.Count > 0)
            {
                string errorLog = "文件：";
                foreach (var s in errorList)
                {
                    errorLog += s + "\n";
                }
                errorLog += "移动失败，目标路径已包含同名文件，请手动确认并移动";
                EditorUtility.DisplayDialog("提示", errorLog, "OK");
            }
            AssetDatabase.Refresh();
        }
    }
}
