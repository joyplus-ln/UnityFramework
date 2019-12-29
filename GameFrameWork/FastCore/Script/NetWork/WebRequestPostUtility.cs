using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class WebRequestPostUtility : MonoBehaviour
{

    public static WebRequestPostUtility Instance;

    enum RequestType
    {
        TEXT_GET,
        TEXTUREE_GET,
        ASSETBUNDEL,
        POST_FORM,
        POST_URLENCODED,
        POST_JSON,
        POST_XML
    }

    class PostContent
    {
        public WWWForm formData;
        public string stringContent;

        public PostContent(WWWForm formData)
        {
            this.formData = formData;
        }

        public PostContent(string text)
        {
            this.stringContent = text;
        }
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

    public void Post(string url, Action<UnityWebRequest> action, WWWForm formData)
    {
        StartCoroutine(Request(url, action, RequestType.POST_FORM, new PostContent(formData)));
    }

    public void PostUrlEncoded(string url,Action<UnityWebRequest> action,string json)
    {
        StartCoroutine(Request(url, action, RequestType.POST_URLENCODED,new PostContent(json)));
    }

    public void PostJson(string url, Action<UnityWebRequest> action, string json)
    {
        StartCoroutine(Request(url, action, RequestType.POST_JSON, new PostContent(json)));
    }

    public void PostXml(string url, Action<UnityWebRequest> action, string json)
    {
        StartCoroutine(Request(url, action, RequestType.POST_XML, new PostContent(json)));
    }

    IEnumerator Request(string url,Action<UnityWebRequest> action,RequestType type, PostContent postContent =null)
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
            case RequestType.POST_FORM:
                webRequest = UnityWebRequest.Post(url, postContent.formData); 
                break;
            case RequestType.POST_URLENCODED:
                webRequest = UnityWebRequest.Post(url, postContent.stringContent);
                //可以不进行设置，此时默认为urlencoded
                webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                break;
            case RequestType.POST_JSON:
                webRequest = UnityWebRequest.Post(url, postContent.stringContent);
                webRequest.SetRequestHeader("Content-Type", "application/json");
                break;
            case RequestType.POST_XML:
                webRequest = UnityWebRequest.Post(url, postContent.stringContent);
                webRequest.SetRequestHeader("Content-Type", "text/xml");//注:text/plain为纯文本
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

        action = null;
        webRequest.Dispose();
        webRequest = null;
        Resources.UnloadUnusedAssets();
    }

    private void Awake()
    {
        Instance = this;
    }
}