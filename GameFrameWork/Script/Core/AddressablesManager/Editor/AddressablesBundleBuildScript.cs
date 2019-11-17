using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HDJ.Framework.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace FastBundle.Editor
{
    public static class AddressablesBundleBuildScript
    {
        //private static Dictionary<string,AddressableAssetGroup> groupDict = new Dictionary<string, AddressableAssetGroup>();

        public static void CreatConfig()
        {
            AddressablesRules rules = new AddressablesRules();
            Dictionary<string, BuildAddressablesData> bundles = rules.GetBuilds();
            SaveJsonManifest(ConstPath.addressAssetsManifesttxt, bundles);
            AssetDatabase.ImportAsset(ConstPath.addressAssetsManifesttxt, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        public static void AddFileToAddressablesInDevelop()
        {
            AddressablesRules rules = new AddressablesRules();
            Dictionary<string, BuildAddressablesData> bundles = rules.GetBuilds();
            AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            AddressableAssetGroup group = null;

            foreach (string key in bundles.Keys)
            {
                group = aaSettings.groups.Find(x => x.Name == bundles[key].GroupName);
                if (group == null)
                {
                    group = aaSettings.CreateGroup(bundles[key].GroupName, false, false, false, null);
                    group.AddSchema<BundledAssetGroupSchema>();
                    //aaSettings.groups.Add(group);
                    group = null;
                }
            }


            foreach (string key in bundles.Keys)
            {
                int count = 0;
                int MaxCount = bundles[key].entitys.Count;
                group = aaSettings.groups.Find(x => x.Name == bundles[key].GroupName);
                foreach (string entitysKey in bundles[key].entitys.Keys)
                {
                    count++;
                    if (count % 3 == 0)
                        if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(
                            string.Format("Collecting... [{0}/{1}]", count, MaxCount), entitysKey,
                            count * 1f / MaxCount))
                        {
                            break;
                        }

                    string guid = AssetDatabase.AssetPathToGUID(bundles[key].entitys[entitysKey]);
                    AddressableAssetEntry entity = aaSettings.CreateOrMoveEntry(guid, group);
                    entity.SetAddress(entitysKey);
                    for (int i = 0; i < bundles[key].Lable.Length; i++)
                    {
                        entity.SetLabel(bundles[key].Lable[i], true);
                    }
                    
                }
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }
        public static void AddFileToAddressables()
        {
            AddressablesRules rules = new AddressablesRules();
            Dictionary<string, BuildAddressablesData> bundles = rules.GetBuilds();
            AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            AddressableAssetGroup group = null;
            //清理重名group
            foreach (string key in bundles.Keys)
            {
                group = aaSettings.groups.Find(x => x.Name == bundles[key].GroupName);
                if (group != null)
                {
                    //group.Name += "_Remove";
                    aaSettings.RemoveGroup(group);
                    group = null;
                }
            }
            foreach (string key in bundles.Keys)
            {
                group = aaSettings.groups.Find(x => x.Name == bundles[key].GroupName);
                if (group == null)
                {
                    group = aaSettings.CreateGroup(bundles[key].GroupName, false, false, false, null);
                    group.AddSchema<BundledAssetGroupSchema>();
                    //aaSettings.groups.Add(group);
                    group = null;
                }
            }


            foreach (string key in bundles.Keys)
            {
                int count = 0;
                int MaxCount = bundles[key].entitys.Count;
                group = aaSettings.groups.Find(x => x.Name == bundles[key].GroupName);
                foreach (string entitysKey in bundles[key].entitys.Keys)
                {
                    count++;
                    if (count % 3 == 0)
                        if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(
                            string.Format("Collecting... [{0}/{1}]", count, MaxCount), entitysKey,
                            count * 1f / MaxCount))
                        {
                            break;
                        }

                    string guid = AssetDatabase.AssetPathToGUID(bundles[key].entitys[entitysKey]);
                    AddressableAssetEntry entity = aaSettings.CreateOrMoveEntry(guid, group);
                    entity.SetAddress(entitysKey);
                    for (int i = 0; i < bundles[key].Lable.Length; i++)
                    {
                        entity.SetLabel(bundles[key].Lable[i], true);
                    }
                    
                }
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }

        static void SaveJsonManifest(string path, Dictionary<string, BuildAddressablesData> bundles)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            AddressablesConfig config = new AddressablesConfig();

            foreach (string bundlesKey in bundles.Keys)
            {
                foreach (string entitysKey in bundles[bundlesKey].entitys.Keys)
                {
                    config.AddGroup(bundles[bundlesKey].GroupName, entitysKey);
                    for (int i = 0; i < bundles[bundlesKey].Lable.Length; i++)
                    {
                        config.AddLable(bundles[bundlesKey].Lable[i], entitysKey);
                    }
                    
                }
            }

            using (var writer = new StreamWriter(path))
            {
                writer.Write(JsonConvert.SerializeObject(config));
                writer.Flush();
                writer.Close();
            }

            CreatAssetViewConst(bundles);
        }
        
        public static void CreatAssetViewConst(Dictionary<string, BuildAddressablesData> bundles)
        {
            string[] resSpit;
            string labs;
            string temp = "public class ViewConst{0}{1}{2}";
            StringBuilder build = new StringBuilder();
            build.AppendLine("");
            foreach (string bundlesKey in bundles.Keys)
            {
                labs = "";
                for (int i = 0; i < bundles[bundlesKey].Lable.Length; i++)
                {
                    labs += bundles[bundlesKey].Lable[i];
                    labs += ";";
                }
                string des = string.Format("//Group：{0} ;Label:{1}", bundles[bundlesKey].GroupName,labs);
                foreach (string entitysKey in bundles[bundlesKey].entitys.Keys)
                {
                    resSpit = entitysKey.Split('.');
                    build.AppendLine(des);
                    build.AppendLine(string.Format("     public const string {0} = {2}{1}{2};",string.Format("{0}_{1}",resSpit[1],resSpit[0]),entitysKey,"\""));
                }
            }
            temp = string.Format(temp, "{",build.ToString(),"}");
            if(File.Exists(Application.dataPath + "/ViewConst.cs"))
            {
                File.Delete(Application.dataPath + "/ViewConst.cs");
            }
            FileUtils.CreateTextFile(Application.dataPath + "/ViewConst.cs", temp);
        }
    }
}