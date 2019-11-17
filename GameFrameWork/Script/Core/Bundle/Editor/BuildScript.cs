using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HDJ.Framework.Utils;
using MiniJSON;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace FastBundle.Editor
{
    public static class BuildScript
    {
        [InitializeOnLoadMethod]
        public static void Clear()
        {
            UnityEditor.EditorUtility.ClearProgressBar();
        }

        static public string CreateAssetBundleDirectory()
        {
            // Choose the output path according to the build target.
            string outputPath = Path.Combine(AssetBundleUtility.AssetBundlesOutputPath, AssetBundleUtility.GetPlatformName());
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            return outputPath;
        }

        public static void BuildAssetBundles(List<AssetBundleBuild> builds)
        {
            // Choose the output path according to the build target.
            string outputPath = CreateAssetBundleDirectory();

            var options = BuildAssetBundleOptions.None;

            bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#if UNITY_TVOS
			shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
#endif
            if (shouldCheckODR)
            {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
				if (PlayerSettings.iOS.useOnDemandResources)
				options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
#if ENABLE_IOS_APP_SLICING
				options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
            }

            if (builds == null || builds.Count == 0)
            {
                //@TODO: use append hash... (Make sure pipeline works correctly with it.)
                BuildPipeline.BuildAssetBundles(outputPath, options, EditorUserBuildSettings.activeBuildTarget);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), options,
                    EditorUserBuildSettings.activeBuildTarget);
            }
        }

        static public void CopyAssetBundlesTo(string outputPath)
        {
            // Clear streaming assets folder.
            //            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string outputFolder = AssetBundleUtility.GetPlatformName();

            // Setup the source folder for assetbundles.
            var source =
                Path.Combine(Path.Combine(System.Environment.CurrentDirectory, AssetBundleUtility.AssetBundlesOutputPath),
                    outputFolder);
            if (!System.IO.Directory.Exists(source))
                Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

            // Setup the destination folder for assetbundles.
            var destination = System.IO.Path.Combine(outputPath, outputFolder);
            if (System.IO.Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(destination);

            FileUtil.CopyFileOrDirectory(source, destination);
        }

        static string[] GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        static string GetAssetBundleManifestFilePath()
        {
            var relativeAssetBundlesOutputPathForPlatform =
                Path.Combine(AssetBundleUtility.AssetBundlesOutputPath, AssetBundleUtility.GetPlatformName());
            return Path.Combine(relativeAssetBundlesOutputPathForPlatform, AssetBundleUtility.GetPlatformName()) +
                   ".manifest";
        }

        static void SaveManifest(string path, List<AssetBundleBuild> builds)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var writer = new StreamWriter(path))
            {
                foreach (var item in builds)
                {
                    writer.WriteLine(item.assetBundleName + ":");
                    foreach (var asset in item.assetNames)
                    {
                        writer.WriteLine(string.Format("\t{0}", asset));
                    }

                    writer.WriteLine();
                }

                writer.Flush();
                writer.Close();
            }
        }

        static void SaveJsonManifest(string path, List<AssetBundleBuild> builds)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            List<ABundleInfo> aBundleInfos = new List<ABundleInfo>();
            for (int i = 0; i < builds.Count; i++)
            {
                ABundleInfo abinfo =  new ABundleInfo();
                abinfo.assetBundleName = builds[i].assetBundleName.ToLower();
                abinfo.assetBundleVariant = builds[i].assetBundleVariant;
                abinfo.assetNames = builds[i].assetNames;
//                for (int j = 0; j < abinfo.assetNames.Length; j++)
//                {
//                    if (abinfo.assetNames[j].ToLower().Contains(abinfo.assetBundleName))
//                    {
//                        abinfo.fullName = abinfo.assetNames[j];
//                        break;
//                    }
//                }
                abinfo.fullName = abinfo.assetBundleName.ToLower();
                abinfo.addressableNames = builds[i].addressableNames;
                if (abinfo.assetBundleName.Contains("/"))
                {
                    abinfo.shortName = abinfo.assetBundleName.Substring(abinfo.assetBundleName.LastIndexOf('/') + 1);
                }
                else
                {
                    abinfo.shortName = abinfo.assetBundleName;
                }
                aBundleInfos.Add(abinfo);
            }

            ABundleInfo info =
                aBundleInfos.Where((x, i) => aBundleInfos.FindIndex(z => z.shortName == x.shortName) == i).GetEnumerator().Current;
            if (info != null)
            {
                Debug.LogError("有重复简称的bundle：" + info.shortName);
            }
            using (var writer = new StreamWriter(path))
            {
                writer.Write(JsonConvert.SerializeObject(aBundleInfos));
                writer.Flush();
                writer.Close();
            }

            CreatAssetViewConst(builds);
        }

        public static void BuildManifestJson(string path, List<AssetBundleBuild> builds, bool forceRebuild = false)
        {
            SaveJsonManifest(path, builds);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        public static void CreatAssetViewConst(List<AssetBundleBuild> builds)
        {
            string[] names;
            string temp = "public class ViewConst{0}{1}{2}";
            StringBuilder build = new StringBuilder();
            build.AppendLine("");
            for (int i = 0; i < builds.Count; i++)
            {
                build.AppendLine(string.Format("//{0}.bundle",i));
                build.AppendLine(string.Format("    //{0}",builds[i].assetBundleName));
                names =  GetAssetType_Name(builds[i].assetBundleName);
                build.AppendLine(string.Format("     public const string {0} = {2}{1}{2};",names[0],names[1],"\""));
                build.AppendLine(string.Format("    //{0}.1.assetBundleVariant",i));
                if (!string.IsNullOrEmpty(builds[i].assetBundleVariant))
                {
                    build.AppendLine(string.Format("    //{0}",builds[i].assetBundleVariant));
                    names =  GetAssetType_Name(builds[i].assetBundleVariant);
                    build.AppendLine(string.Format("     public const string {0} = {2}{1}{2};",names[0],names[1],"\""));
                }
                else
                {
                    build.AppendLine("    //assetBundleVariant null");
                }

                build.AppendLine(string.Format("    //{0}.2.assetNames",i));
                if (builds[i].assetNames != null)
                {
                    for (int j = 0; j < builds[i].assetNames.Length; j++)
                    {
                        build.AppendLine(string.Format("    //{0}",builds[i].assetNames[j]));
                        names =  GetAssetType_Name(builds[i].assetNames[j]);
                        build.AppendLine(string.Format("    //不可直接viewconst引用加载"));
                        build.AppendLine(string.Format("    public const string cant_load_{0} = {2}{1}{2};",names[0],names[1],"\""));
                    }
                }
                else
                {
                    build.AppendLine(string.Format("//assetNames null"));
                }
                build.AppendLine(string.Format("    //{0}.3.addressableNames",i));
                if (builds[i].addressableNames != null)
                {
                    for (int m = 0; m < builds[i].addressableNames.Length; m++)
                    {
                        build.AppendLine(string.Format("    //{0}",builds[i].addressableNames[m]));
                        names =  GetAssetType_Name(builds[i].addressableNames[m]);
                        build.AppendLine(string.Format("    public const string {0} = {2}{1}{2};",names[0],names[1],"\""));
                    }
                }else
                {
                    build.AppendLine(string.Format("    //addressableNames null"));
                }

            }

            temp = string.Format(temp, "{",build.ToString(),"}");
            if(File.Exists(Application.dataPath + "/ViewConst.cs"))
            {
                File.Delete(Application.dataPath + "/ViewConst.cs");
            }
            FileUtils.CreateTextFile(Application.dataPath + "/ViewConst.cs", temp);
        }

        static string[] GetAssetType_Name(string name)
        {
            if (name.Contains('/'))
            {
                string[] names = name.Split('/');
                if (names[names.Length - 1].Contains('.'))
                {
                    string[] type_name = names[names.Length - 1].Split('.');
                    //string[] back = new[] {string.Format("{0}_{1}",type_name[1],type_name[0]), type_name[0]};
                    string[] back = new[] {string.Format("{0}_{1}",type_name[1],type_name[0]), names[names.Length - 1]};
                    return back;
                }
                else
                {
                    string[] back = new[] {string.Format("{0}_{1}","unknown",names[names.Length - 1]), names[names.Length - 1]};
                    return back;
                }
            }
            else
            {
                if (name.Contains('.'))
                {
                    string[] type_name = name.Split('.');
                    string[] back = new[] {string.Format("{0}_{1}",type_name[1],type_name[0]), type_name[0]};
                    return back;
                }
                else
                {
                    string[] back = new[] {string.Format("{0}_{1}","bundle",name), name};
                    return back;
                }
            }
        }
    }
}