// ========================================
// Copyright (c) 2017 KingSoft, All rights reserved.
// http://www.kingsoft.com
// 
// Framwork
// 
// Filename: AssetUtility.cs
// Date:     2017/06/09
// Author:   xiangjinbao
// Email:    xiangjinbao@kingsoft.com
// ========================================

using UnityEngine;
using System.IO;

namespace xsj.framework
{
    /// <summary>
    /// 资源加载辅助类
    /// </summary>
    public static class AssetUtility
    {

        /// <summary>
        /// 获取本地资源路径
        /// </summary>
        /// <returns></returns>
        private static string GetAssetLocalPath()
        {
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    return Application.dataPath + "/../../../Build/AssetBundle/";

                case RuntimePlatform.WindowsPlayer:
                    return Application.dataPath + "/../AssetBundle/";

                case RuntimePlatform.Android:
                    return Application.persistentDataPath + "/Data/";

                case RuntimePlatform.IPhonePlayer:
                    return Application.persistentDataPath + "/Raw/";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 获取StreamingAssets资源路径
        /// </summary>
        /// <returns></returns>
        private static string GetAssetStreamingAssetsPath()
        {
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    return "file://" + Application.dataPath + "/../../../Build/AssetBundle/";

                case RuntimePlatform.WindowsPlayer:
                    return "file://" + Application.dataPath + "/../AssetBundle/";

                case RuntimePlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/Data/";

                case RuntimePlatform.IPhonePlayer:
                    return "file://" + Application.dataPath + "/Raw/";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 获取本地StreamingAssets资源路径
        /// </summary>
        /// <returns></returns>
        private static string GetAssetLocalStreamingAssetsPath()
        {
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    return "file://" + Application.dataPath + "/../../../Build/AssetBundle/";

                case RuntimePlatform.WindowsPlayer:
                    return "file://" + Application.dataPath + "/../AssetBundle/";

                case RuntimePlatform.Android:
                    return "file://" + Application.persistentDataPath + "/Data/";

                case RuntimePlatform.IPhonePlayer:
                    return "file://" + Application.persistentDataPath + "/Data/";

                default:
                    return string.Empty;
            }
        }
    }
}
