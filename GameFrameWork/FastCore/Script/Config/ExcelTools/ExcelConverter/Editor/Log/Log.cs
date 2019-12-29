using UnityEditor;

namespace ExcelConverter.Tools
{
    public static class Log
    {
        public static void Info(string InMsg)
        {
            EditorUtility.DisplayDialog("Info", InMsg, "Ok");
        }

        public static void Warning(string InMsg)
        {
            EditorUtility.DisplayDialog("Warning", InMsg, "Ok");
        }

        public static void Error(string InMsg)
        {
            EditorUtility.DisplayDialog("Error", InMsg, "Ok");
        }
    }
}

