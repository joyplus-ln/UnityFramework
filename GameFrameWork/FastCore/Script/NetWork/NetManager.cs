using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Debug = System.Diagnostics.Debug;

public class NetManager : MonoBehaviour
{
    static Queue<RequestQueue> queues = new Queue<RequestQueue>();

    public static void Get(string url, Action<RequestInfo> callback)
    {
        RequestQueue requestqueue = new RequestQueue();
        requestqueue.url = url;
        requestqueue.type = "Get";
        requestqueue.callback = callback;
        queues.Enqueue(requestqueue);
    }
    public static void Post(string url,Dictionary<string,string> postDict, Action<RequestInfo> callback)
    {
        RequestQueue requestqueue = new RequestQueue();
        requestqueue.url = url;
        requestqueue.type = "Post";
        requestqueue.postDict = postDict;
        requestqueue.callback = callback;
        queues.Enqueue(requestqueue);
    }

    IEnumerator GetQueue(RequestQueue requsetInfo)
    {
        UnityWebRequest request = UnityWebRequest.Get(requsetInfo.url);
        yield return request.SendWebRequest();
        RequestInfo info = new RequestInfo();
        if (request.isDone)
        {
            if (request.isHttpError || request.isNetworkError)
            {
                info.error = true;
            }
            else
            {
                info.requsetStr = request.downloadHandler.text;
            }
        }

        if (requsetInfo.callback != null)
        {
            requsetInfo.callback.Invoke(info);
        }
    }
    
    IEnumerator PostQueue(RequestQueue requsetInfo)
    {
        UnityWebRequest request = UnityWebRequest.Post(requsetInfo.url,requsetInfo.postDict);
        yield return request.SendWebRequest();
        RequestInfo info = new RequestInfo();
        if (request.isDone)
        {
            if (request.isHttpError || request.isNetworkError)
            {
                info.error = true;
            }
            else
            {
                info.requsetStr = request.downloadHandler.text;
            }
        }

        if (requsetInfo.callback != null)
        {
            requsetInfo.callback.Invoke(info);
        }
    }

    private void Update()
    {
        if (queues.Count > 0)
        {
            RequestQueue requestqueue = queues.Dequeue();
            switch (requestqueue.type)
            {
                case "Get":
                    StartCoroutine(GetQueue(requestqueue));
                    break;
                case "Post":
                    StartCoroutine(PostQueue(requestqueue));
                    break;
            }
            
        }
    }
}

public class RequestInfo
{
    public string url;
    public bool error;
    public string requsetStr;
}

class RequestQueue
{
    public string url;
    public string type;
    public Dictionary<string, string> postDict;
    public Action<RequestInfo> callback;
}