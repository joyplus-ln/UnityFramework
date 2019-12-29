using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BetaFramework;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.Experimental.AssetBundlePatching;

namespace FastBundle.Editor
{
    public static class AddressablesBundleBuildScript
    {
        public static void CreatConfig()
        {
            AddressablesRules rules = new AddressablesRules();
            Dictionary<string, BuildAddressablesData> bundles = rules.GetBuilds();
            SaveJsonManifest(FrameWorkConst.addressAssetsManifesttxt, bundles);
            AssetDatabase.ImportAsset(FrameWorkConst.addressAssetsManifesttxt, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
        

        public static void BuildBundle()
        {
            CreatConfig();
            AddressableAssetSettings.CleanPlayerContent();
            BuildCache.PurgeCache(false);
            AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = 3;
            AddressableAssetSettings.BuildPlayerContent();
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
                    aaSettings.RemoveGroup(group);
                    group = null;
                }
            }

            foreach (string key in bundles.Keys)
            {
                group = aaSettings.groups.Find(x => x.Name == bundles[key].GroupName);
                if (group == null)
                {
                    if (bundles[key].ResType == "online")
                    {
                        group = aaSettings.CreateGroup(bundles[key].GroupName, false, false, false, null);
                        BundledAssetGroupSchema schema =  group.AddSchema<BundledAssetGroupSchema>();
                        schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                        if (bundles[key].packageType == "PackSeparately")
                        {
                            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately; 
                        }else if (bundles[key].packageType == "PackTogether")
                        {
                            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether; 
                        }else if (bundles[key].packageType == "PackTogetherByLabel")
                        {
                            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel; 
                        }
                        schema.BuildPath.SetVariableByName(group.Settings, AddressableAssetSettings.kRemoteBuildPath);
                        schema.LoadPath.SetVariableByName(group.Settings, AddressableAssetSettings.kRemoteLoadPath);
                        schema.UseAssetBundleCache = true;
                        schema.UseAssetBundleCrc = true;
                        ContentUpdateGroupSchema contentUpdateGroupSchema =  group.AddSchema<ContentUpdateGroupSchema>();
                        contentUpdateGroupSchema.StaticContent = false;
                        group = null;
                    }
                    else
                    {
                        group = aaSettings.CreateGroup(bundles[key].GroupName, false, false, false, null);
                        BundledAssetGroupSchema schema =  group.AddSchema<BundledAssetGroupSchema>();
                        schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.Uncompressed;
                        if (bundles[key].packageType == "PackSeparately")
                        {
                            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately; 
                        }else if (bundles[key].packageType == "PackTogether")
                        {
                            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether; 
                        }else if (bundles[key].packageType == "PackTogetherByLabel")
                        {
                            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel; 
                        }

                        if (bundles[key].canUpdate)
                        {
                            ContentUpdateGroupSchema contentUpdateGroupSchema =  group.AddSchema<ContentUpdateGroupSchema>();
                            contentUpdateGroupSchema.StaticContent = false;
                        }
                        
                        schema.UseAssetBundleCache = true;
                        schema.UseAssetBundleCrc = true;
                        group = null;
                    }

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
                        aaSettings.AddLabel(bundles[key].Lable[i]);
                        entity.SetLabel(bundles[key].Lable[i], true);
                    }
                }
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }

		internal static void UpdateWordLibrary()
		{
			string groupName = "WordLibrary";
			UnityEditor.EditorUtility.DisplayCancelableProgressBar("更新词库...", "", 0);
			AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
			AddressableAssetGroup group = null;
			group = aaSettings.groups.Find(x => x.Name == groupName);
			if (group != null) {
				aaSettings.RemoveGroup(group);
				group = null;
			}
			group = aaSettings.CreateGroup(groupName, false, false, false, null);
			group.AddSchema<BundledAssetGroupSchema>();

			string levelDir = string.Format("{0}/AssetsPackage/AnsycLoad/CodyLevel", Application.dataPath);
			if (!Directory.Exists(levelDir)) {
				Debug.LogError("路径不存在 " + levelDir);
				Directory.CreateDirectory(levelDir);
			} else {
				var files = Directory.GetFiles(levelDir);
				var index = 0;
				foreach (var item in files) {
					index++;
					UnityEditor.EditorUtility.DisplayCancelableProgressBar("更新词库...", Path.GetFileName(item), index/(float)files.Length);
					var extention = Path.GetExtension(item);
					if (extention.Equals(".txt")) {
						string guid = AssetDatabase.AssetPathToGUID(string.Format("Assets/AssetsPackage/AnsycLoad/CodyLevel/{0}", Path.GetFileName(item)));
						AddressableAssetEntry entity = aaSettings.CreateOrMoveEntry(guid, group);
						entity.SetAddress(Path.GetFileName(item));
						bool result = entity.SetLabel("WordLibrary", true);
						Debug.Log("set label result " + result);
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

                string des = string.Format("//Group：{0} ;Label:{1}", bundles[bundlesKey].GroupName, labs);
                foreach (string entitysKey in bundles[bundlesKey].entitys.Keys)
                {
                    resSpit = entitysKey.Split('.');
                    build.AppendLine(des);
                    build.AppendLine(string.Format("     public const string {0} = {2}{1}{2};",
                        string.Format("{0}_{1}", resSpit[1], resSpit[0]), entitysKey, "\""));
                }
            }

            temp = string.Format(temp, "{", build.ToString(), "}");
            if (File.Exists(Application.dataPath + "/ViewConst.cs"))
            {
                File.Delete(Application.dataPath + "/ViewConst.cs");
            }
            FilesUtils.CreateTextFile(Application.dataPath + "/ViewConst.cs", temp);
        }


    }
    
}