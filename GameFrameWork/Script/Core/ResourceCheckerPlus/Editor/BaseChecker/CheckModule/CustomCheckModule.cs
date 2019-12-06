using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class CustomCheckModule : ResCheckModuleBase
    {
        public override void ShowCommonSideBarContent()
        {
            if (GUILayout.Button("检查资源"))
            {
                CheckerInterface.CheckResource(Selection.objects);
                CheckerInterface.ApplyCheckFilter();
                CheckerInterface.ExportCheckResult();
            }
        }

        public override void CheckResource(Object[] objects)
        {
            Clear();
            for(int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("正在检查资源", "已完成：" + i + "/" + objects.Length, (float)i / objects.Length);
                Object root = objects[i];
                if (root == null)
                    continue;
                activeCheckerList.ForEach(x => x.MixCheckDirectAndRefRes(root));
            }
            EditorUtility.ClearProgressBar();
            Refresh();
        }
    }
}
