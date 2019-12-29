using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class CheckerConfigManager 
    {
        //配置文件路径
        public static string configRootPath = "Assets/ThirdParty/ResourceCheckerPlus/Editor/CheckerConfig";
        public static string commonConfigName = "/CommonConfig.asset";
        public static string initConfigName = "/InitConfig.asset";
        public static string predefineFilterCfgPath = "/FilterCfg";
        public static string checkerCfgPath = "/CheckerCfg";
        public static string checkModuleCfgPath = "/CheckModuleCfg";
        public static string defaultExportResultPath = "Assets";

        public static CheckerCommonConfig checkerConfig = null;
        public static Dictionary<string, CheckerConfig> checkerConfigDic = new Dictionary<string, CheckerConfig>();
        public static List<CheckModuleConfig> checkModuleConfigList = new List<CheckModuleConfig>();
        public static int spriteBarWidth = 4;
        public static int sideBarWidth = 180;
        public static Color defaultTextColor;
        public static Color defaultBackgroundColor;

        public void InitConfig()
        {
            defaultTextColor = GUI.color;
            defaultBackgroundColor = GUI.backgroundColor;
            InitCheckModuleConfig();
            InitCheckerCommonConfig();
        }

        public void InitCheckerCommonConfig()
        {
            if (checkerConfig == null)
            {
                string path = configRootPath + commonConfigName;
                checkerConfig = AssetDatabase.LoadAssetAtPath<CheckerCommonConfig>(path);
                if (checkerConfig == null)
                {
                    checkerConfig = ScriptableObject.CreateInstance<CheckerCommonConfig>();
                    AssetDatabase.CreateAsset(checkerConfig, path);
                }
            }
        }

        public CheckerConfig GetCheckerConfig(string checkerName)
        {
            CheckerConfig cfg = null;
            checkerConfigDic.TryGetValue(checkerName, out cfg);
            if (cfg == null)
            {
                string path = configRootPath + checkerCfgPath + "/" + checkerName + ".asset";
                cfg = AssetDatabase.LoadAssetAtPath<CheckerConfig>(path);
                if (cfg == null)
                {
                    cfg = ScriptableObject.CreateInstance<CheckerConfig>();
                    AssetDatabase.CreateAsset(cfg, path);
                }
            }
            return cfg;
        }

        public void InitCheckModuleConfig()
        {
            checkModuleConfigList.Clear();
            string configPath = configRootPath + checkModuleCfgPath;
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { configPath });
            if (guids == null)
                return;
            foreach(var v in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(v);
                CheckModuleConfig cfg = AssetDatabase.LoadAssetAtPath<CheckModuleConfig>(path);
                if (cfg == null)
                    continue;
                checkModuleConfigList.Add(cfg);
            }
        }

        public void SaveCheckerConfig()
        {
            EditorUtility.SetDirty(checkerConfig);
            checkModuleConfigList.ForEach(x => EditorUtility.SetDirty(x));
            var v = checkerConfigDic.GetEnumerator();
            while(v.MoveNext())
            {
                EditorUtility.SetDirty(v.Current.Value);
            }
            AssetDatabase.SaveAssets();
        }

    }
}

