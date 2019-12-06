using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLoadInfo<T>
{
    public string assetAddress;
    public Action<T> callBack;
    public Action<string,T> callBackT;
    public Action<string,AsyncOperationHandle<T>> callBackHandler;
    public AssetLoadInfo(string assetAddress,Action<T> callBack)
    {
        this.assetAddress = assetAddress;
        this.callBack = callBack;
    }
    
    public AssetLoadInfo(string assetAddress,Action<string,AsyncOperationHandle<T>> callBackHandler)
    {
        this.assetAddress = assetAddress;
        this.callBackHandler = callBackHandler;
    }

    public AssetLoadInfo(string assetAddress,Action<string,T> callBack)
    {
        this.assetAddress = assetAddress;
        this.callBackT = callBack;
        
    }
    public void StartLoad()
    {
        Addressables.LoadAssetAsync<T>(assetAddress).Completed += LoadCompleted;
    }

    private void LoadCompleted(AsyncOperationHandle<T> handler)
    {
        if (callBack != null)
        {
            callBack.Invoke(handler.Result);
        }

        if (callBackT != null)
        {
            callBackT.Invoke(assetAddress,handler.Result);
        }

        if (callBackHandler != null)
        {
            callBackHandler.Invoke(assetAddress,handler);
        }
    }
}
