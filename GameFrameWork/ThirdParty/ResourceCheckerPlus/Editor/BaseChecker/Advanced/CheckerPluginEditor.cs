using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class CheckerPluginEditor : EditorWindow
    {
        public static List<Object> objectList = null;
        public static Vector2 pos = Vector2.zero;

        public void ShowList()
        {
            pos = EditorGUILayout.BeginScrollView(pos);
            if (objectList != null)
            {
                foreach (var v in objectList)
                {
                    if (v == null)
                        continue;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(v.name, GUILayout.Width(250));
                    GUILayout.Label(AssetDatabase.GetAssetPath(v));
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}