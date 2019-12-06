using System;
using System.Collections.Generic;
using BetaFramework;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetsLoadInfo<T>
{
    public List<string> addressList;
    public Action<Dictionary<string, T>> callBack;
    private Action<Dictionary<string, AsyncOperationHandle<T>>> callBackHandler;
    private Dictionary<string, T> infos = new Dictionary<string, T>();
    Dictionary<string, AsyncOperationHandle<T>> handleInfo = new Dictionary<string, AsyncOperationHandle<T>>();
    private int index = 0;

    public AssetsLoadInfo(List<string> addressList, Action<Dictionary<string, T>> callBack)
    {
        this.addressList = addressList;
        this.callBack = callBack;
    }

    public AssetsLoadInfo(List<string> addressList, Action<Dictionary<string, AsyncOperationHandle<T>>> callBackHandler)
    {
        this.addressList = addressList;
        this.callBackHandler = callBackHandler;
    }

    public void StartLoad()
    {
        for (int i = 0; i < addressList.Count; i++)
        {
            AssetLoadInfo<T> info = new AssetLoadInfo<T>(addressList[i], LoadCompleted);
            info.StartLoad();
        }
    }


    public void StartLoadHandler()
    {
        for (int i = 0; i < addressList.Count; i++)
        {
            AssetLoadInfo<T> info = new AssetLoadInfo<T>(addressList[i], LoadHandlerCompleted);
            info.StartLoad();
        }
        
    }

    private void LoadHandlerCompleted(string address, AsyncOperationHandle<T> obj)
    {
        index++;
        handleInfo.Add(address, obj);
        if (index >= addressList.Count)
        {
            if (callBackHandler != null)
            {
                callBackHandler.Invoke(handleInfo);
            }
        }
    }

    private void LoadCompleted(string address, T obj)
    {
        index++;
        infos.Add(address, obj);
        if (index >= addressList.Count)
        {
            if (callBack != null)
            {
                callBack.Invoke(infos);
            }
        }
    }
}