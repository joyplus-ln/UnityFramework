using System.Collections;
using System.Collections.Generic;
using FastBundle.Editor;
using UnityEditor;
using UnityEngine;

public class BundleMenuTool
{
    [MenuItem ("Fast/Bundle/ReInit")]
    static void CopyAssetPath ()
    {
        //生成资源根路径
        DirectoryTool.CreatIfNotExists(Application.dataPath + FrameWorkConst.FastBundleResFolder);
        DirectoryTool.CreatIfNotExists(Application.dataPath + FrameWorkConst.FastBundleResConfigFolder);
        AssetDatabase.Refresh();
    }
    [MenuItem ("Fast/Bundle/ReBuildViewConst")]
    static void BuildViewConst()
    {
        AddressablesBundleBuildScript.CreatConfig();
    }
    
    
    [MenuItem ("Fast/Bundle/AddFileToBundle")]
    static void AddFileToBundle()
    {
        AddressablesBundleBuildScript.AddFileToAddressables();
    }
    [MenuItem ("Fast/Bundle/BuildBundle")]
    static void BuildBundle()
    {
        AddressablesBundleBuildScript.BuildBundle();
    }
}
