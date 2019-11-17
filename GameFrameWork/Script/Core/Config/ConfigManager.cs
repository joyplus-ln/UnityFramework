﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FrameWork;
using System.Text;
using System;
using Newtonsoft.Json;

/// <summary>
/// 配置管理器，只读
/// </summary>
public static class ConfigManager 
{
    public const string c_directoryName = "Config";
    public const string c_expandName    = "json";

    /// <summary>
    /// 配置缓存
    /// </summary>
    static Dictionary<string, Dictionary<string, SingleField>> s_configCache = new Dictionary<string,Dictionary<string, SingleField>>();

    public static bool GetIsExistConfig(string ConfigName)
    {
        Debug.LogError("xxx");
        return false;
        //return ResourceManager.GetResourceIsExist(ConfigName);
    }

    public static Dictionary<string, SingleField> GetData(string ConfigName)
    {
        if (s_configCache.ContainsKey(ConfigName))
        {
            return s_configCache[ConfigName];
        }

        string dataJson = "";

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            dataJson = ResourceIOTool.ReadStringByResource(
                PathTool.GetRelativelyPath(c_directoryName,
                            ConfigName,
                            c_expandName));
        }
        else
        {
            Debug.LogError("xxx");
            //dataJson = ResourceManager.ReadTextFile(ConfigName);
        }
#else
                //dataJson = ResourceManager.ReadTextFile(ConfigName);
#endif

        if (dataJson == "")
        {
            throw new Exception("ConfigManager GetData not find " + ConfigName);
        }
        else
        {
            Dictionary<string, SingleField> config =
                JsonConvert.DeserializeObject<Dictionary<string, SingleField>>(dataJson);

            s_configCache.Add(ConfigName, config);
            return config;
        }
    }

    public static void CleanCache()
    {
        s_configCache.Clear();
    }
}