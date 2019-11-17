using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace FastBundle
{
    public class Manifest
    {

        
        List<ABundleInfo> aBundleInfos = new List<ABundleInfo>();
        static Dictionary<string,ABundleInfo> keyBundleNameInfos = new Dictionary<string, ABundleInfo>();
        static Dictionary<string,ABundleInfo> keyShortNameInfos = new Dictionary<string, ABundleInfo>();

        public string FindBundleName(string shortName)
        {
            shortName = shortName.ToLower();
           return keyShortNameInfos[shortName].assetBundleName;
        }
        
        public string GetAssetName(string assetPath)
        {
            return Path.GetFileName(assetPath);
        }

        public string GetFullName(string shortName)
        {
            return keyShortNameInfos[shortName].fullName;
        }
        
        void Init()
        {
            keyBundleNameInfos.Clear();
            keyShortNameInfos.Clear();
        } 
        
        public void LoadJson(TextReader reader)
        {
            Init();       
            string assetText = reader.ReadToEnd();
            aBundleInfos = JsonConvert.DeserializeObject<List<ABundleInfo>>(assetText);
            for (int i = 0; i < aBundleInfos.Count; i++)
            {
                keyBundleNameInfos.Add(aBundleInfos[i].assetBundleName,aBundleInfos[i]);
                keyShortNameInfos.Add(aBundleInfos[i].shortName.ToLower(),aBundleInfos[i]);
            }

        } 
        
        public void LoadJson(string assetText)
        {
            Init();       
            aBundleInfos = JsonConvert.DeserializeObject<List<ABundleInfo>>(assetText);
            for (int i = 0; i < aBundleInfos.Count; i++)
            {
                keyBundleNameInfos.Add(aBundleInfos[i].assetBundleName,aBundleInfos[i]);
                keyShortNameInfos.Add(aBundleInfos[i].shortName.ToLower(),aBundleInfos[i]);
            }

        } 
    }

}