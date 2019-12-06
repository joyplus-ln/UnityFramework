using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FastBundle.Editor
{
    public class AddressablesRules
    {
        private List<AddressablesRuleData> rules = new List<AddressablesRuleData>();
        /// <summary>
        /// 获取bundle配置文件，每个AssetBundleBuild一个bundle
        /// </summary>
        /// <param name="manifestPath"></param>
        /// <returns></returns>
        public Dictionary<string,BuildAddressablesData> GetBuilds()
        {
            Dictionary<string,BuildAddressablesData> buildAddressablesDatas = new Dictionary<string, BuildAddressablesData>();
            const string rulesini = FrameWorkConst.addreesablesAssetsRuletxt;
            if (File.Exists(rulesini))
            {
                LoadRules(rulesini);
            }
            else
            {
                Debug.LogError("不存在rule文件");
            }
            foreach (var item in rules)
            {
               List<string> files = GetFilesWithoutDirectories(item.searchPath, item.searchPattern, item.searchOption);
               BuildAddressablesData group = new BuildAddressablesData();
               group.GroupName = item.GroupName;
               group.Lable = item.Lable;
               group.ResType = item.resType;
               group.packageType = item.packageType;
               group.canUpdate = item.canUpdate;
               for (int i = 0; i < files.Count; i++)
               {
                   if (files[i].Contains('/'))
                   {
                       int start = files[i].LastIndexOf('/') + 1;
                       group.entitys.Add(files[i].Substring(start,files[i].Length - start),files[i]);
                   }
                   else
                   {
                       group.entitys.Add(files[i],files[i]);
                   }
                   
               }
               buildAddressablesDatas.Add(item.ID,group);
            }
            UnityEditor.EditorUtility.ClearProgressBar();
            return buildAddressablesDatas;
        }

        void LoadRules(string rulesini)
        {
            using (var s = new StreamReader(rulesini))
            {
                rules.Clear();

                string line = null;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty || line.StartsWith("#", StringComparison.CurrentCulture) || line.StartsWith("//", StringComparison.CurrentCulture))
                    {
                        continue;
                    }
                    if (line.Length > 2 && line[0] == '[' && line[line.Length - 1] == ']')
                    {
                        var name = line.Substring(1, line.Length - 2);
                        var ID = s.ReadLine().Split('=')[1];
                        var searchPath = s.ReadLine().Split('=')[1];
                        var searchPattern = s.ReadLine().Split('=')[1];
                        var searchOption = s.ReadLine().Split('=')[1];
                        var GroupName = s.ReadLine().Split('=')[1];
                        var lable = s.ReadLine().Split('=')[1];
                        var restype = s.ReadLine().Split('=')[1];
                        var PackageType = s.ReadLine().Split('=')[1];
                        var canupdate = s.ReadLine().Split('=')[1];
                        var type = typeof(AddressablesRules).Assembly.GetType("FastBundle.Editor." + name);
                        if (type != null)
                        {
                            AddressablesRuleData rule = Activator.CreateInstance(type) as AddressablesRuleData;
                            rule.ID = ID;
                            rule.searchPath = searchPath;
                            rule.searchPattern = searchPattern;
                            rule.searchOption = (SearchOption)Enum.Parse(typeof(SearchOption), searchOption);
                            rule.GroupName = GroupName;
                            rule.Lable = lable.Split('|');
                            rule.resType = restype;
                            rule.packageType = PackageType;
                            rule.canUpdate = bool.Parse(canupdate);
                            rules.Add(rule);
                        }
                    }
                }
            }
        }

        static List<string> GetFilesWithoutDirectories(string prefabPath, string searchPattern, SearchOption searchOption)
        {
            var files = Directory.GetFiles(prefabPath, searchPattern, searchOption);
            List<string> items = new List<string>();
            int i = 0;
            int MaxCount = files.Count();
            foreach (var item in files)
            {
                i++;
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, MaxCount), item, i * 1f / MaxCount))
                {
                    break;
                }
                var assetPath = item.Replace('\\', '/');
                if (!Directory.Exists(assetPath))
                {
                    items.Add(assetPath.Replace(Application.dataPath,"Assets/"));
                }
            }
            
            return items;
        }
        

        

    }

    public class AddressablesRuleData
    {
        public string ID;
        public string searchPath;
        public string searchPattern;
        public SearchOption searchOption = SearchOption.AllDirectories;
        public string GroupName;
        public string[] Lable;
        public string resType;
        public string packageType;
        public bool canUpdate;

        public AddressablesRuleData(){}
    }

}