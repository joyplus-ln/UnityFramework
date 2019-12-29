using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class WebRequestGetUtility : MonoBehaviour
{
    public static WebRequestGetUtility Instance;

    enum RequestType
    {
        TEXT_GET,
        TEXTUREE_GET,
        ASSETBUNDEL,
        POST
    }

    public void Get(string url,Action<UnityWebRequest> action)
    {
        StartCoroutine(Request(url,action,RequestType.TEXT_GET));
    }

    public void GetTexture(string url,Action<UnityWebRequest> action)
    {
        StartCoroutine(Request(url, action, RequestType.TEXTUREE_GET));
    }

    public void GetAssetBundle(string url,Action<UnityWebRequest> action)
    {
        StartCoroutine(Request(url, action, RequestType.ASSETBUNDEL));
    }

    public void Post(string url, Action<UnityWebRequest> action, List<IMultipartFormSection> formData)
    {
        StartCoroutine(Request(url, action, RequestType.POST, formData));
    }

    IEnumerator Request(string url,Action<UnityWebRequest> action,RequestType type, List<IMultipartFormSection> formData=null)
    {
        UnityWebRequest webRequest = null;

        switch (type)
        {
            case RequestType.TEXT_GET:
                webRequest = UnityWebRequest.Get(url);
                break;
            case RequestType.TEXTUREE_GET:
                webRequest = UnityWebRequestTexture.GetTexture(url);
                break;
            case RequestType.ASSETBUNDEL:
                webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
                break;
            case RequestType.POST:
                webRequest = UnityWebRequest.Post(url, formData); 
                break;
            default:
                break;
        }

        if(webRequest==null)
        {
            Debug.Log("WebRequest initialise error");
            yield break;
        }

        yield return webRequest.SendWebRequest();

        action?.Invoke(webRequest);

        webRequest.Dispose();
        webRequest = null;
        Resources.UnloadUnusedAssets();
    }

    
    private void Awake()
    {
        Instance = this;        
    }
}