using System.Collections;
using System.Collections.Generic;
using FastBundle;
using UnityEngine;

public class StandardMainfest
{
    private AssetBundleManifest _assetBundleManifest;
    

    public void Load()
    {
        AssetBundle mainfestBundle = AssetBundle.LoadFromFile(AssetBundleUtility.GetStreamingStandardMainfestPath());
        _assetBundleManifest = mainfestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }
}
