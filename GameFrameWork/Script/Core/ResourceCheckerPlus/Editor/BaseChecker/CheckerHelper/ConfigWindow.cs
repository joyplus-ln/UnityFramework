using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 配置窗口类
    /// </summary>
    public class ConfigWindow : EditorWindow
    {
        public static string[] checkDragTypeStr = new string[] { "拖入检查，将Project中目录拖入槽内进行检查", "选中检查,选中Project面板内资源进行检查，支持多选" };
        public static string[] checkBatchOptionStr = new string[] { "处理列表中全部内容", "仅处理列表中选中的内容" };
        public static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow<ConfigWindow>();
            window.maxSize = new Vector2(500, 400);
            exportResultPath = AssetDatabase.LoadAssetAtPath<Object>(CheckerConfigManager.checkerConfig.checkResultExportPath);
        }

        private static Object exportResultPath = null;

        void OnGUI()
        {
            CheckerCommonConfig config = CheckerConfigManager.checkerConfig;
            if (config != null)
            {
                config.highlightTextColor = EditorGUILayout.ColorField("选中文字高亮颜色", config.highlightTextColor);
                config.warningItemColor = EditorGUILayout.ColorField("警告条目高亮颜色", config.warningItemColor);
                config.sideBarWidth = EditorGUILayout.IntSlider("侧边栏宽度", config.sideBarWidth, 180, 300);
                config.autoFilterOnSideBarButtonClick = GUILayout.Toggle(config.autoFilterOnSideBarButtonClick, "点击侧边栏对象自动进行引用筛选", GUILayout.Width(300));
                config.inputType = (CheckInputMode)EditorGUILayout.Popup("Porject资源检查方式", (int)config.inputType, checkDragTypeStr);
                config.batchOptionType = (BatchOptionSelection)EditorGUILayout.Popup("批量处理功能处理范围", (int)config.batchOptionType, checkBatchOptionStr);
                config.maxCheckRecordCount = EditorGUILayout.IntSlider("常用查询记录最大值", config.maxCheckRecordCount, 5, 20);
                EditorGUI.BeginChangeCheck();
                exportResultPath = EditorGUILayout.ObjectField("导出检查结果Excel路径", exportResultPath, typeof(Object), false);
                if (EditorGUI.EndChangeCheck())
                {
                    config.checkResultExportPath = AssetDatabase.GetAssetPath(exportResultPath);
                    if (!ResourceCheckerHelper.isFolder(config.checkResultExportPath))
                    {
                        config.checkResultExportPath = "";
                        exportResultPath = null;
                    }
                }
            }
        }
    }
}
