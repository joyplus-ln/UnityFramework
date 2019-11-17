using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

    public sealed partial class ExcelEditorStyleUtils
    {
        private static bool CanBuild(string filePath, Dictionary<string, string> filters)
        {
            if (filePath.Contains(".meta") || filePath.Contains(".DS_Store") || filePath.Contains("~")) return false;
            if (filters == null) return true;

            string ext = Path.GetExtension(filePath);
            foreach (KeyValuePair<string, string> filter in filters)
            {
                if (filePath.IndexOf(filter.Key) > -1)
                {
                    if (ext != "" && filter.Value.IndexOf(ext) > -1)
                    {
                        EditorUtility.DisplayProgressBar("读取目录", string.Format("正在读取{0}", Path.GetFileName(filePath)), 100);
                        return true;
                    }
                    else
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        public static void RecursivePath(string path, Dictionary<string, string> filter, List<string> fileList)
        {
            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path);
                string[] paths = Directory.GetDirectories(path);
                foreach (string filePath in filePaths)
                {
                    if (CanBuild(filePath, filter))
                    {
                        fileList.Add(filePath);
                    }
                }
                foreach (string dir in paths)
                {
                    RecursivePath(dir, filter, fileList);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }
        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }
        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="telephone"></param>
        /// <returns></returns>
        public static bool IsTelephone(string telephone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(telephone, @"^(\d{3,4}-)?\d{6,8}$");
        }
        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="handset"></param>
        /// <returns></returns>
        public static bool IsHandset(string handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(handset, @"^[1]+[3,5]+\d{9}");
        }
        /// <summary>
        /// 验证身份证号
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public static bool IsIDcard(string idcard)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(idcard, @"(^\d{18}$)|(^\d{15}$)");
        }
        /// <summary>
        /// 验证输入为数字
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsNumber(string number)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(number, @"^[0-9]*$");
        }
        /// <summary>
        /// 验证邮编
        /// </summary>
        /// <param name="postalcode"></param>
        /// <returns></returns>
        public static bool IsPostalcode(string postalcode)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(postalcode, @"^\d{6}$");
        }
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
    }