using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetsTest : MonoBehaviour
{
    public AssetReference _asset;
     void Start()
    {
        //_asset.InstantiateAsync();
        //Instantiate(await Addressables.LoadAssetAsync<GameObject>("ToastWindow.prefab"));
        CreatInfo();
        Debug.LogError("4");
        // Addressables.LoadAssetAsync<GameObject>("ToastWindow.prefab").Completed += LoadComplete;
//        GameObject obj = Instantiate( Addressables.LoadAsset<GameObject>("ToastWindow.prefab").Result);

    }

    public async void CreatInfo()
    {
        Instantiate(await Load<GameObject>("ToastWindow.prefab"));
        Debug.LogError("2");
        Instantiate(await Load<GameObject>("ToastWindow.prefab"));
        Debug.LogError("3");
    }

    public async Task<T> Load<T>(string resName)
    {
        Debug.LogError(resName);
        return await Addressables.LoadAssetAsync<T>(resName);
    }
    void LoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        GameObject obj = Instantiate(asyncOperationHandle.Result);
    }
}
public static class Extension{
    public static TaskAwaiter<T> GetAwaiter<T>(this AsyncOperationHandle<T> ap)
    {
        var tcs = new TaskCompletionSource<T>();
        ap.Completed += op => tcs.TrySetResult(op.Result);
        return tcs.Task.GetAwaiter();
    }
}
