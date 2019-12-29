using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableExtensions 
{
    public static TaskAwaiter<T> GetAwaiter<T>(this AsyncOperationHandle<T> ap)
    {
        var tcs = new TaskCompletionSource<T>();
        ap.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                tcs.TrySetResult(op.Result);
            }
            else
            {
                tcs.TrySetResult(default);
            }
            
        };
        return tcs.Task.GetAwaiter();
    }
}
