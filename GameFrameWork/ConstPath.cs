using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstPath
{
    
    public static string UIWindowClassTemplate = Application.dataPath + "/GameFrameWork/Script/Core/Editor/res/UIWindowClassTemplate.txt";
    public static string UILuaScriptTemplate = Application.dataPath + "/Script/Core/Editor/res/UILuaScriptTemplate.txt";
    public static string UILuaScriptSavePath = "/AssetPackage/Lua/UI/Lua";
    public static string UIScriptSavePath = Application.dataPath + "/UIWindow/Script/UI/{0}/{1}.cs";

    public const string c_ResourceFolder = "AssetPackage";
    public const string c_ConfigFolder = "UFConfig";
    //assets目录下的这个文件夹定义为bundle文件夹
    public const string c_ResourceParentPath = "/AssetPackage/";
    public const string UICreatPath= "AssetPackage/UI";
    public const string UIPath = "UI/UIManager.prefab";
    
    /// <summary>
    ///bundle 清单文件存放位置
    /// </summary>
    public const string assetsManifesttxt = "Assets/AssetPackage/UFConfig/Manifest.txt";
    public const string addressAssetsManifesttxt = "Assets/AssetPackage/UFConfig/addressAssetsConfig.txt";
    public const string assetsRuletxt = "Assets/AssetPackage/UFConfig/Rules.txt";
    public const string addreesablesAssetsRuletxt = "Assets/AssetPackage/UFConfig/AddressablesRules.txt";
    public const string assetsManifesttxtFolder = "Assets/AssetPackage/UFConfig";
    
    public const string LIGHTMAP_RESOURCE_PATH = "Assets/Resources/Lightmaps/";

    /// <summary>
    /// 只要路径下包含这个名称，这个bundle就会被单独打出来
    /// </summary>
    public const string singleBundle = "singlebundle";

    public const string sharedBundle = "shared/";
}
