//Resource Checker Plus接口文件
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class CheckerInterface
    {
        public static void InitResourceChecker()
        {
            ResourceCheckerPlus.Init();
        }

        public static void SetCheckModule(string checkModuleName)
        {
            ResourceCheckerPlus.instance.SetCurrentActiveCheckModule(checkModuleName);
        }

        public static void CheckResource(Object[] objects = null)
        {
            ResourceCheckerPlus.instance.CurrentCheckModule().CheckResource(objects);
        }

        public static void ApplyCheckFilter()
        {
            ResourceCheckerPlus.instance.CurrentCheckModule().LoadPredefineCheckFilterCfg();
        }

        public static void ExportCheckResult()
        {
            ResourceCheckerPlus.instance.CurrentCheckModule().ExportAllActiveCheckerResult();
        }
    }
}


