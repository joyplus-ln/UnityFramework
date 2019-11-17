using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

public class RecordTable : Dictionary<string, string>
{
    #region 解析
    public static RecordTable Load(string data)
    {
        RecordTable result = new RecordTable();
        Dictionary<string, string> tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        List<string> keys = new List<string>(tmp.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            result.Add(keys[i],tmp[keys[i]]);
        }
        return result;
    }

    public static string Serialize(RecordTable table)
    {
        return JsonConvert.SerializeObject(table);
    }

    #endregion

    #region 取值封装



    public string GetRecord(string key, string defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key] ;
        }
        else
        {
            return defaultValue;
        }
    }

    public bool GetRecord(string key, bool defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return bool.Parse(this[key]);
        }
        else
        {
            return defaultValue;
        }
    }

    public int GetRecord(string key, int defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return int.Parse(this[key]);
        }
        else
        {
            return defaultValue;
        }
    }

    public float GetRecord(string key, float defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return float.Parse(this[key]);
        }
        else
        {
            return defaultValue;
        }
    }









    #endregion

    #region 存值封装

    public void SetRecord(string key,string value)
    {
        if(this.ContainsKey(key))
        {
            this[key] = value;
        }
        else
        {
            this.Add(key, value);
        }
    }

    public void SetRecord(string key, int value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = value.ToString();
        }
        else
        {
            this.Add(key, value.ToString());
        }
    }

    public void SetRecord(string key, bool value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = value.ToString();
        }
        else
        {
            this.Add(key, value.ToString());
        }
    }

    public void SetRecord(string key, float value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = value.ToString();
        }
        else
        {
            this.Add(key, value.ToString());
        }
    }


    #endregion
}
