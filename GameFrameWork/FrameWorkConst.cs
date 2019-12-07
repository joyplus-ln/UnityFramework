using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameWorkConst
{
    public const string c_ResourceFolder = "AssetsPackage";
    public const string c_ConfigFolder = "UFConfig";
    //assets目录下的这个文件夹定义为bundle文件夹
    public const string c_ResourceParentPath = "/AssetsPackage/";
    public const string UICreatPath= "AssetPackage/UI";
    public const string UIPath = "UI/UIManager.prefab";
    
    /// <summary>
    ///bundle 清单文件存放位置
    /// </summary>
    public const string assetsManifesttxt = "Assets/AssetsPackage/UFConfig/Manifest.txt";
    public const string assetsRuletxt = "Assets/AssetsPackage/UFConfig/Rules.txt";
    public const string OnLineAssetsRuletxt = "Assets/SkinResource/Rules.txt";
    public const string assetsManifesttxtFolder = "Assets/AssetsPackage/UFConfig";
    
    
    public const string addressAssetsManifesttxt = "Assets/AssetsPackage/AnsycLoad/UFConfig/addressAssetsConfig.txt";
    public const string addreesablesAssetsRuletxt = "Assets/AssetsPackage/AnsycLoad/UFConfig/AddressablesRules.txt";

    /// <summary>
    /// 只要路径下包含这个名称，这个bundle就会被单独打出来
    /// </summary>
    public const string singleBundle = "singlebundle";

    public const string sharedBundle = "shared/";
    
    public const string AssetBundlesOutputPath = "AssetBundles";

    public const string SpriteAtlas = "spriteatlas";
}
