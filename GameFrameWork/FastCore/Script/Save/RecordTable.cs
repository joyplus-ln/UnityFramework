using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BetaFramework
{
    public class RecordTable<T>
    {
        public delegate T GetTValue(string key);

        public delegate void SetTValue(string key, T usefulValue);

        private GetTValue getTValue;
        private SetTValue setTValue;
        private string typeString;

        public RecordTable(GetTValue getTValue, SetTValue setTValue)
        {
            this.getTValue = getTValue;
            this.setTValue = setTValue;
            this.typeString = typeof(T).ToString();
        }

        private string CacheKey = "UF_RT_Cache";
        private const char cacheCut = '|';
        private static Dictionary<string, T> valueDictionary = new Dictionary<string, T>();
        
        private bool needSave = false;

        public T GetValue(string key, T defaultValue)
        {
            if (valueDictionary.ContainsKey(key))
            {
                return valueDictionary[key];
            }
            else
            {
                if (PlayerPrefs.HasKey(key))
                {
                    T needValue = getTValue(key);
                    valueDictionary.Add(key, needValue);
                    return needValue;
                }
            }
            return defaultValue;
        }

        public void SetValue(string key, T usefulValue)
        {
            if (valueDictionary.ContainsKey(key))
            {
                valueDictionary[key] = usefulValue;
            }
            else
            {
                valueDictionary.Add(key, usefulValue);
                needSave = true;
            }
            setTValue(key, usefulValue);
        }

        public void LoadCache()
        {
            List<string> keyList = new List<string>();
            string cache_key = PlayerPrefs.GetString(CacheKey + typeString);
            if (!string.IsNullOrEmpty(cache_key))
            {
                string[] keys = cache_key.Split(cacheCut);
                for (int i = 0; i < keys.Length; i++)
                {
                    if(!string.IsNullOrEmpty(keys[i]))
                        keyList.Add(keys[i]);
                }
            }
            if (keyList == null)
            {
                keyList = new List<string>();
            }

            for (int i = 0; i < keyList.Count; i++)
            {
                AddCache(keyList[i]);
            }
        }

        private void AddCache(string key)
        {
            if(string.IsNullOrEmpty(key))
                return;
            if (!valueDictionary.ContainsKey(key))
            {
                valueDictionary.Add(key, getTValue(key));
            }
            else
            {
                FastLog.Error("Double Key:" + key);
            }
        }

        public void SaveCacheKey()
        {
            if (!needSave) return;
            StringBuilder builder = new StringBuilder();
            foreach (string valueDictionaryKey in valueDictionary.Keys)
            {
                builder.Append(valueDictionaryKey);
                builder.Append(cacheCut);
            }
            PlayerPrefs.SetString(CacheKey + typeString, builder.ToString());
        }

        public bool HasKey(string key)
        {
            if (valueDictionary.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        public void DeleteKey(string key)
        {
            if (valueDictionary.ContainsKey(key))
                valueDictionary.Remove(key);
        }
    }
}