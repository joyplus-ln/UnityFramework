using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastBundle
{
    public class AssetBundleUtility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

#if UNITY_ANDROID
        public const string AssetBundleStandardMainfest = "AssetBundles/Android/Android";
#elif UNITY_IOS
    public const string AssetBundleStandardMainfest = "AssetBundles/Android/Android";
#elif UNITY_STANDALONE_WIN
    public const string AssetBundleStandardMainfest = "AssetBundles/Android/Android";
#endif
        public static string GetStreamingPath()
        {
            return Application.streamingAssetsPath;
        }

        public static string GetPersistentpath()
        {
            return Application.persistentDataPath;
        }

        /// <summary>
        /// 获取平台下的标准bundle清单文件
        /// </summary>
        /// <returns></returns>
        public static string GetStreamingStandardMainfestPath()
        {
            return Path.Combine(GetStreamingPath(), AssetBundleStandardMainfest);
        }
    
        /// <summary>
        /// 获取平台下的标准bundle清单文件
        /// </summary>
        /// <returns></returns>
        public static string GetPersistentStandardMainfestPath()
        {
            return Path.Combine(GetPersistentpath(), AssetBundleStandardMainfest);
        }

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }

        public static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            if (platform == RuntimePlatform.Android)
            {
                return "android";
            }
            if (platform == RuntimePlatform.IPhonePlayer)
            {
                return "ios";
            }
            if (platform == RuntimePlatform.tvOS)
            {
                return "tvos";
            }
            if (platform == RuntimePlatform.WebGLPlayer)
            {
                return "webgl";
            }
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
            {
                return "windows";
            }
            if (platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.OSXEditor)
            {
                return "osx";
            }
            return null;
        }

#if UNITY_EDITOR
        static string GetPlatformForAssetBundles(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                return "android";
            }
            if (target == BuildTarget.tvOS)
            {
                return "tvos";
            }
            if (target == BuildTarget.iOS)
            {
                return "ios";
            }
            if (target == BuildTarget.WebGL)
            {
                return "webgl";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return "windows";
            }
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target ==  BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSX)
#endif
            {
                return "osx";
            }
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            return null;
        }
    #endif 
    } 
}