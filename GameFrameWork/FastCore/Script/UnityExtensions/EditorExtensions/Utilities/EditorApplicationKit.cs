#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// 编辑器 Application 工具箱
    /// </summary>
    public struct EditorApplicationKit
    {
        static float _deltaTime;
        static double _lastTimeSinceStartup;


        [InitializeOnLoadMethod]
        static void Init()
        {
            EditorApplication.update += () =>
            {
                _deltaTime = (float)(EditorApplication.timeSinceStartup - _lastTimeSinceStartup);
                _lastTimeSinceStartup = EditorApplication.timeSinceStartup;
            };
        }


        public static float deltaTime
        {
            get { return _deltaTime; }
        }


        /// <summary>
        /// 打开指定路径的文件夹（可以使用文件路径，可以使用相对路径）
        /// </summary>
        public static void OpenFolder(string path)
        {
#if UNITY_EDITOR_WIN
            path = path.Replace('/', '\\');

            while (!Directory.Exists(path))
                path = path.Substring(0, path.LastIndexOf('\\'));

            System.Diagnostics.Process.Start("explorer.exe", path);
#endif
        }

    } // struct EditorApplicationKit

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR