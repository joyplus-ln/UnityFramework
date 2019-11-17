using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 引用资源检查功能
    /// </summary>
    public class ReferenceResCheckModule : ResCheckModuleBase
    {
        private GUIContent checkPrefabDetailRefContent = new GUIContent("检查资源被Prefab下子节点的引用", "开启该选项后，在检查Prefab引用的资源时，会将资源具体被哪些子物体引用了也统计出来（切换该属性需要重新检查）");
        private GUIContent mixCheckResourceContent = new GUIContent("检查目录下全部资源及其引用", "开启该选项后，检查时不仅会检查路径下资源的引用，路径下包含的裸资源也会一并进行检查（切换该属性需要重新检查）");
        private bool showRefCheckInputConfig = true;
        public bool refCheckPrefab = true;
        public bool refCheckScene = false;
        public bool refCheckMaterial = false;
        public bool checkPrefabDetailRef = false;
        public bool mixCheckResource = false;

        public override void SetCheckerConfig()
        {
            ShowRefCheckItem(true, checkPrefabDetailRef, checkPrefabDetailRef);
        }

        public override void ShowCommonSideBarContent()
        {
            CheckerCommonConfig cfg = CheckerConfigManager.checkerConfig;
            if (cfg.inputType == CheckInputMode.DragMode)
            {
                ShowObjectDragSlot();
            }
            if (GUILayout.Button("检查资源"))
            {
                if (mixCheckResource)
                    MixResCheck();
                else
                    CheckResource(null);
            }
            mixCheckResource = GUILayout.Toggle(mixCheckResource, mixCheckResourceContent);
            if (refCheckPrefab && !mixCheckResource)
            {
                EditorGUI.BeginChangeCheck();
                checkPrefabDetailRef = GUILayout.Toggle(checkPrefabDetailRef, checkPrefabDetailRefContent);
                if (EditorGUI.EndChangeCheck())
                {
                    ShowRefCheckItem(true, checkPrefabDetailRef, checkPrefabDetailRef);
                }
            }
            if (!mixCheckResource)
            {
                showRefCheckInputConfig = EditorGUILayout.Foldout(showRefCheckInputConfig, new GUIContent("     引用资源检查范围    "));
                if (showRefCheckInputConfig)
                {
                    refCheckPrefab = GUILayout.Toggle(refCheckPrefab, "Prefab", GUILayout.Width(cfg.sideBarWidth));
                    refCheckScene = GUILayout.Toggle(refCheckScene, "Scene", GUILayout.Width(cfg.sideBarWidth));
                    refCheckMaterial = GUILayout.Toggle(refCheckMaterial, "Material", GUILayout.Width(cfg.sideBarWidth));
                }
            }
        }

        public override void CheckResource(Object[] resources)
        {
            Clear();
            Object[] selection = GetAllObjectInSelection();
            string filter = refCheckPrefab ? "t:Prefab" : "";
            filter += refCheckScene ? " t:Scene" : "";
            filter += refCheckMaterial ? " t:Material" : "";
            activeCheckerList.ForEach(x => x.ReferenceResCheck(selection, filter, checkPrefabDetailRef));
            Refresh();
        }

        public void MixResCheck()
        {
            Clear();
            Object[] selection = GetAllObjectInSelection();
            List<Object> checkObjects = ObjectChecker.GetAllObjectFromInput<Object>(selection, "t:Object");
            foreach(var v in checkObjects)
            {
                activeCheckerList.ForEach(x => x.MixCheckDirectAndRefRes(v));
            }
            Refresh();
        }
    }
}
