using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public static class ResourceManager
{
    
    public static void LoadAsync<T>(string assetAddress,Action<T> callback)
    {
        AssetLoadInfo<T> loadInfo = new AssetLoadInfo<T>(assetAddress,callback);
        loadInfo.StartLoad();
    }

    public static void LoadAsync<T>(List<string> resList,Action<Dictionary<string,T>> callBack)
    {
        AssetsLoadInfo<T> info = new AssetsLoadInfo<T>(resList,callBack);
        info.StartLoad();
    }
    
    public static void LoadAsync<T>(List<string> resList,Action<Dictionary<string,AsyncOperationHandle<T>>> callBack)
    {
        AssetsLoadInfo<T> info = new AssetsLoadInfo<T>(resList,callBack);
        info.StartLoadHandler();
    }
    
    public static AsyncOperationHandle<T> LoadAsync<T>(string assetAddress)
    {
       return Addressables.LoadAssetAsync<T>(assetAddress);
    }

    public static T LoadAsyncResult<T>(string assetAddress)
    {
        return Addressables.LoadAssetAsync<T>(assetAddress).Result;
    }
}
