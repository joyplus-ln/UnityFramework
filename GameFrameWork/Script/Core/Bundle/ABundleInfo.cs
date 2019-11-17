using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABundleInfo
{
    /// <summary>
    ///   <para>AssetBundle name.</para>
    /// </summary>
    public string assetBundleName;

    /// <summary>
    ///   <para>AssetBundle variant.</para>
    /// </summary>
    public string assetBundleVariant;

    /// <summary>
    ///   <para>Asset names which belong to the given AssetBundle.</para>
    /// </summary>
    public string[] assetNames;

    /// <summary>
    ///   <para>Addressable name used to load an asset.</para>
    /// </summary>
    public string[] addressableNames;

    public string shortName;
    
    public string fullName;
}