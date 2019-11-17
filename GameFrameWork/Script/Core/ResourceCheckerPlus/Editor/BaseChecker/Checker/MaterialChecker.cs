using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class MaterialChecker : ObjectChecker
    {
        public class MaterialDetail : ObjectDetail
        {
            public MaterialDetail(Object obj, MaterialChecker checker) : base(obj, checker)
            {
               
            }

            public override void InitDetailCheckObject(Object obj)
            {
                MaterialChecker checker = currentChecker as MaterialChecker;
                Material material = obj as Material;
                checkMap.Add(checker.matShaderName, material.shader == null ? "null" : material.shader.name);
                checkMap.Add(checker.matRenderQueue, material.renderQueue);
                checkMap.Add(checker.matPassCount, material.passCount);
                Texture tex = null;
                if (material.HasProperty("_MainTex") && material.mainTexture != null)
                {
                    tex = material.mainTexture;
                    checkMap[checker.previewItem] = tex;
                }
            }
        }

        CheckItem matShaderName;
        CheckItem matRenderQueue;
        CheckItem matPassCount;

        public override void InitCheckItem()
        {
            checkerName = "Material";
            checkerFilter = "t:Material";
            postfix = ".mat";
            enableReloadCheckItem = true;
            matShaderName = new CheckItem(this, "Shader", 350);
            matRenderQueue = new CheckItem(this, "渲染队列", 80, CheckType.Int);
            matPassCount = new CheckItem(this, "Pass数", 80, CheckType.Int);
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            if (obj is Material)
            {
                ObjectDetail detail = null;
                foreach (var v in CheckList)
                {
                    if (v.checkObject == obj)
                        detail = v;
                }
                if (detail == null)
                {
                    detail = new MaterialDetail(obj, this);
                }
                detail.AddObjectReference(refObj, detailRefObj);
            }
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (checkModule is SceneResCheckModule)
                        AddObjectDetail(mat, r.gameObject, null);
                    else if (checkModule is ReferenceResCheckModule)
                        AddObjectDetail(mat, rootObj, r.gameObject);
                }
            }
        }
    }
}