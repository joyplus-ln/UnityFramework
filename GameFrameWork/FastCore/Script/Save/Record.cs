using System;
using System.IO;
using UnityEngine;

namespace BetaFramework
{
    /// <summary>
    /// 数据持久化
    /// 由于原生方法只提供了string，int，float三种类型，所以只提供这三种类型，其余的根据已有类型进行判断
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Record
    {
        private static RecordTable<string> stringTable =
            new RecordTable<string>(RecordDelegateFun.GetString, RecordDelegateFun.SetString);

        private static RecordTable<int> intTable =
            new RecordTable<int>(RecordDelegateFun.GetInt, RecordDelegateFun.SetInt);

        private static RecordTable<float> floatTable =
            new RecordTable<float>(RecordDelegateFun.GetFloat, RecordDelegateFun.SetFloat);
        

        public static string GetString(string key, string defaultValue = "")
        {
            return stringTable.GetValue(key, defaultValue);
        }

        public static void SetString(string key, string usefulValue)
        {
            stringTable.SetValue(key, usefulValue);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return intTable.GetValue(key, defaultValue);
        }

        public static void SetInt(string key, int usefulValue)
        {
            intTable.SetValue(key, usefulValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return intTable.GetValue(key, defaultValue ? 1 : 0) == 1;
        }

        public static void SetBool(string key, bool usefulValue)
        {
            intTable.SetValue(key, usefulValue ? 1 : 0);
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            return floatTable.GetValue(key, defaultValue);
        }

        public static void SetFloat(string key, float usefulValue)
        {
            floatTable.SetValue(key, usefulValue);
        }

        public static double GetDouble(string key, double defaultValue = 0f)
        {
            return double.Parse(stringTable.GetValue(key, defaultValue.ToString()));
        }

        public static void SetDouble(string key, double usefulValue)
        {
            stringTable.SetValue(key, usefulValue.ToString());
        }
        

        public static T GetObject<T>(string key, T defVal) where T : new()
        {
            T obj = defVal;
            try
            {
                string objData = stringTable.GetValue(key, "");
                obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objData);
                if (obj == null)
                    obj = defVal;
            }
            catch (Exception ex)
            {
                FastLog.Log(ex);
            }

            return obj;
        }

        public static void SetObject(string key, object obj)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (obj != null)
                {
                    stringTable.SetValue(key, Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                }
                else
                {
                    stringTable.SetValue(key, "");
                }
            }
        }
        

        
    }
}