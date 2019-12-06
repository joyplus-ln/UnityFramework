using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastLog
{
    public static void Log(object message, UnityEngine.Object context = null)
    {
#if LOG_CONSOLE
            if (context == null)
            {
                UnityEngine.Debug.Log(message);
            }
            else
            {
                UnityEngine.Debug.Log(message, context);
            }
#endif
    }
    
    public static void Error(string message, UnityEngine.Object context = null)
    {
#if LOG_CONSOLE
            if (context == null)
            {
                UnityEngine.Debug.LogError(message);
            }
            else
            {
                UnityEngine.Debug.LogError(message, context);
            }
#endif
    }
}