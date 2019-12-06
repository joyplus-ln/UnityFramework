using UnityEngine;

public class RecordDelegateFun
{
    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static void SetString(string key, string usefulValue)
    {
        PlayerPrefs.SetString(key, usefulValue);
    }

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static void SetInt(string key, int usefulValue)
    {
        PlayerPrefs.SetInt(key, usefulValue);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static void SetFloat(string key, float usefulValue)
    {
        PlayerPrefs.SetFloat(key, usefulValue);
    }
    
}