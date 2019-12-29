using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable

class ProjectBuildService : Editor
{
    #region 参数解析

    public static string ChannelName
    {
        get
        {
#if UNITY_ANDROID
            //这里遍历所有参数，找到 ChannelName 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("ChannelName"))
                {
                    return arg.Split("-"[0])[1];
                }
            }
            return "Android";
#elif UNITY_IOS
            return "IOS";
#else
            return "General";
#endif
        }
    }

    public static string ExportPath
    {
        get
        {
            string path = Application.dataPath.Substring(0,Application.dataPath.LastIndexOf('/'));

            //这里遍历所有参数，找到 ExportPath 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("ExportPath"))
                {
                    path = arg.Split("-"[0])[1];
                }
            }


#if UNITY_IOS
            return path += "/" + ChannelName;
#else
            return path += "/" + ChannelName  + "/";
#endif
        }
    }
    

    public static bool IsUseAssetsBundle
    {
        get
        {
            //这里遍历所有参数，找到 UseAssetsBundle 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("UseAssetsBundle"))
                {
                    return bool.Parse(arg.Split("-"[0])[1]);
                }
            }
            return false;
        }
    }

    public static bool IsUseLua
    {
        get
        {
            //这里遍历所有参数，找到 UseLua 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("UseLua"))
                {
                    return bool.Parse(arg.Split("-"[0])[1]);
                }
            }
            return false;
        }
    }

    public static string Version
    {
        get
        {
            return Application.version;// + "." + VersionService.LargeVersion + "." + VersionService.SmallVersion;
        }
    }

    #endregion

    #region 打包函数

    #region 通用

    static void PrintDebug()
    {
        string debugString = "";

        debugString += ">>>============================================================自动打包输出============================================================<<<\n";

        foreach (string arg in Environment.GetCommandLineArgs())
        {
            debugString += "参数：" + arg + "\n";
        }

        debugString += "\n";

        debugString += "是否使用 Bundle 打包: " + IsUseAssetsBundle + "\n";
        debugString += "是否使用 Lua : " + IsUseLua + "\n";
        debugString += "渠道名: " + ChannelName + "\n";
        debugString += "导出路径: " + ExportPath + "\n";
        debugString += ">>>====================================================================================================================================<<<\n";

        Debug.Log(debugString);
    }
    

    /// <summary>
    /// 切换渠道
    /// </summary>
    static void ChangeChannel(string channelName)
    {
#if UNITY_ANDROID
        //SchemeDataService.ChangeScheme(channelName);
#endif
    }
    
    static void BundlePackage()
    {
        //自动增加小版本号
        //VersionService.SmallVersion++;
        //VersionService.CreateVersionFile();

        //打Bundle包
        //PackageService.Package(PackageEditorConfigService.RelyPackages, PackageEditorConfigService.Bundles);

        //删除 Resources 文件夹
        //FileTool.SafeDeleteDirectory(Application.dataPath + "/Resources");
    }

    #endregion

    #region Android

    [MenuItem("Fast/BuildScript/BuildAndroid")]
    static void BuildForAndroid()
    {
        SwitchPlatform(BuildTarget.Android);

        //输出日志
        PrintDebug();
        
        //切换渠道
        ChangeChannel(ChannelName);

        //设置编译指令
        ApplyScriptDefine();

        //设置签名
        //签名路径
        PlayerSettings.Android.keystoreName = "";
        //密钥密码
        PlayerSettings.Android.keystorePass = "";
        //密钥别名
        PlayerSettings.Android.keyaliasName = "";
        //别名密码
        PlayerSettings.Android.keyaliasPass = "";

        //打包
        string path = ExportPath + "/" + GetPackageName();

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
		EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
		BuildPlayerOptions bo = new BuildPlayerOptions();
		bo.scenes = GetBuildScenes();
		bo.target = BuildTarget.Android;
		bo.options = BuildOptions.None;
        bo.locationPathName = path;
		
		BuildPipeline.BuildPlayer(bo);



    }

    #endregion

    #region IOS

    [MenuItem("Fast/BuildScript/BuildIOS")]
    static void BuildForIOS()
    {
        SwitchPlatform(BuildTarget.iOS);

        //输出日志
        PrintDebug();

        //切换渠道
        ChangeChannel(ChannelName);

        //设置编译指令
        ApplyScriptDefine();

        //打包

        PlayerSettings.iOS.allowHTTPDownload=true;
        BuildPlayerOptions bo = new BuildPlayerOptions();
        bo.scenes = GetBuildScenes();
        bo.target = BuildTarget.iOS;
        bo.options = BuildOptions.None;
        bo.locationPathName = ExportPath;
        
        BuildPipeline.BuildPlayer(bo);

    }

#endregion



#endregion

#region 功能函数

    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static string GetPackageName()
    {
#if UNITY_WEBGL
        return Application.productName;
#else
        return Application.productName + "_" + Version + "_"+ ChannelName  + "_"+ GetTimeString();
#endif
    }

    static string GetTimeString()
    {
        DateTime date = DateTime.Now;

        return date.Year + string.Format("{0:d2}", date.Month) + string.Format("{0:d2}", date.Day) + "_" + string.Format("{0:d2}", date.Hour) + string.Format("{0:d2}", date.Minute);
    }
    

    public static void SetScriptDefine(string symbols)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
#if UNITY_ANDROID
        targetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#endif
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (!define.Contains(symbols))
        {
            define += ";" + symbols;
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, define);
    }

    static List<string> s_defines = new List<string>();
    static void AddScriptDefine(string symbols)
    {
        if(!s_defines.Contains(symbols))
        {
            s_defines.Add(symbols);
        }
    }

    static void ApplyScriptDefine()
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
#if UNITY_ANDROID
        targetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#elif UNITY_WEBGL
        targetGroup = BuildTargetGroup.WebGL;
#endif
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        for (int i = 0; i < s_defines.Count; i++)
        {
            if(!define.Contains(s_defines[i]))
            {
                define += ";" + s_defines[i];
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, define);
    }

    /// <summary>
    /// 切换平台
    /// </summary>
    /// <param name="target"></param>
    public static void SwitchPlatform(BuildTarget target)
    {
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        }
    }

#endregion
}
