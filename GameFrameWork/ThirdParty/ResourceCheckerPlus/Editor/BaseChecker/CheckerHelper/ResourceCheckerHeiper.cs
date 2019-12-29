using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 工具函数类
    /// </summary>
    public class ResourceCheckerHelper
    {
        public static bool isFolder(string path)
        {
            string asset = "Asset";
            string head = Application.dataPath.TrimEnd(asset.ToCharArray());
            return Directory.Exists(head + path);
        }

        public bool HasChineseCharInString(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        private string andSymbol = "&&";
        private string unSymbol = "!";

        private bool ComplexFilter(string filter, string str)
        {

            string[] detailFilter = filter.Split(andSymbol.ToCharArray());
            foreach (string detail in detailFilter)
            {
                if (!CheckFilterInternal(detail, str))
                    return false;
            }
            return true;
        }

        private bool CheckFilterInternal(string filter, string str)
        {
            bool doesNotContain = filter.StartsWith(unSymbol);
            if (doesNotContain)
            {
                filter = filter.TrimStart(unSymbol.ToCharArray());
                return !str.Contains(filter);
            }
            else
            {
                return str.Contains(filter);
            }
        }

        public static string GetAssetPostfix(string assetPath)
        {
            return assetPath.Contains('.') ? assetPath.Substring(assetPath.LastIndexOf('.')) : "Unknown";
        }

        /// <summary>
        /// 无意中发现了一下，assetdatabase.GetAll查结尾的时候，能查到失效的prefab，而FindAllAssets是找不出来失效的prefab的
        /// </summary>
        public static void PrintAllInvalidPrefab()
        {
            string[] assetPath = AssetDatabase.GetAllAssetPaths();
            string[] prefabPath1 = assetPath.Where(x => x.Contains(".prefab")).ToArray();
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            string[] prefabPath2 = guids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            string[] prefabPath3 = prefabPath1.Where(x => !prefabPath2.Contains(x)).ToArray();
            foreach (var v in prefabPath3)
            {
                Debug.Log(v);
            }
        }

        public static string GenericExportFolderName()
        {
            string folderString = CheckerConfigManager.checkerConfig.checkResultExportPath;
            if (string.IsNullOrEmpty(folderString))
            {
                folderString = CheckerConfigManager.defaultExportResultPath;
            }
            string head = "Assets";
            folderString = folderString.TrimStart(head.ToCharArray());
            string dateString = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
            string timeString = System.DateTime.Now.ToLongTimeString().Replace(":", ".").Replace(" ", "");
            return Application.dataPath + folderString + "/ResourceCheckResult/" + dateString + "_" +timeString;
        }
    }
}