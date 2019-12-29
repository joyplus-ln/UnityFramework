using UnityEngine;

public class RecordDelegateFun
{
    public static string GetString(string key,string defaultValue)
    {
        return ES3.Load(key, defaultValue);
    }

    public static void SetString(string key, string usefulValue)
    {
        ES3.Save<string>(key,usefulValue);
    }

    public static int GetInt(string key,int defalutValue)
    {
        return ES3.Load(key, defalutValue);
    }

    public static void SetInt(string key, int usefulValue)
    {
       ES3.Save<int>(key,usefulValue);
    }

    public static float GetFloat(string key,float defaultFloat)
    {
        return ES3.Load(key, defaultFloat);
    }

    public static void SetFloat(string key, float usefulValue)
    {
        ES3.Save<float>(key,usefulValue);
    }
    
}