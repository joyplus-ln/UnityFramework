using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 场景检查功能
    /// </summary>
    public class SceneResCheckModule : ResCheckModuleBase
    {
        public bool completeRefCheck = false;
        private GUIContent completeRefCheckContent = new GUIContent("场景全面引用资源检查", "默认检查模式只能检查基本类型的引用关系，如Renderer上引用的贴图资源等，检查速度快，引用关系仅包含自身；全面检查可以检查全场景所有类型的资源引用，包括一些脚本上引用的资源，检查速度慢，引用关系会包含父物体（切换该属性需要重新检查）");
        public override void SetCheckerConfig()
        {
            ShowRefCheckItem(true, false, !completeRefCheck);
        }

        public override void ShowCommonSideBarContent()
        {
            if (GUILayout.Button("检查全场景资源"))
            {
                CheckCurrentSceneTotalRes();
            }

            if (GUILayout.Button("检查Hierarchy选中节点下资源"))
            {
                CheckResource(null);
            }
            EditorGUI.BeginChangeCheck();
            completeRefCheck = GUILayout.Toggle(completeRefCheck, completeRefCheckContent);
            if (EditorGUI.EndChangeCheck())
            {
                ShowRefCheckItem(true, false, !completeRefCheck);
            }
        }

        public override void CheckResource(Object[] resources)
        {
            Clear();
            GameObject[] rootObjects = null;
            if (resources == null)
                rootObjects = Selection.gameObjects;
            else
                rootObjects = resources.Cast<GameObject>().ToArray();
            CheckResInternal(rootObjects);
            Refresh();
        }

        private void CheckResInternal(GameObject[] rootObjects)
        {
            foreach (var go in rootObjects)
            {
                if (go == null)
                    continue;
                activeCheckerList.ForEach(x => {
                    if (completeRefCheck)
                        x.SceneResCheck(go);
                    else
                        x.AddObjectDetailRef(go);
                });
            }
        }

        void CheckCurrentSceneTotalRes()
        {
            Clear();
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = scene.GetRootGameObjects();
            CheckResInternal(rootObjects);
            //加入天空盒的资源
            Material skyMat = RenderSettings.skybox;
            Object[] skyTex = EditorUtility.CollectDependencies(new Object[] { skyMat });
            activeCheckerList.ForEach(x => { 
               foreach (var obj in skyTex)
               {
                   x.AddObjectDetail(obj, null, null);
               }
           });
            Refresh();
        }
    }
}
