using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ResourceCheckerPlus
{

    public class ShaderProperty
    {
        public string name;
        public string description;
        public string type;
    }

    /// <summary>
    /// Shader类型检查
    /// </summary>
    public class ShaderChecker : ObjectChecker
    {
        public class ShaderDetail : ObjectDetail
        {
            public ShaderDetail(Object obj, ShaderChecker checker) : base(obj, checker)
            {
               
            }

            public override void InitDetailCheckObject(Object obj)
            {
                Shader shader = obj as Shader;
                ShaderChecker checker = currentChecker as ShaderChecker;
                checkMap.Add(checker.shaderMaxLod, shader.maximumLOD);
                checkMap.Add(checker.shaderRenderQueue, shader.renderQueue);
                int propertyCount = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < propertyCount; i++)
                {
                    GetShaderProperty(shader, i);
                }
                checkMap.Add(checker.shaderPropertyCount, propertyCount);
            }

            private void GetShaderProperty(Shader shader, int index)
            {
                ShaderProperty property = new ShaderProperty();
                property.name = ShaderUtil.GetPropertyName(shader, index);
                property.description = ShaderUtil.GetPropertyDescription(shader, index);
                property.type = ShaderUtil.GetPropertyType(shader, index).ToString();
                propertyList.Add(property);
            }

            public List<ShaderProperty> propertyList = new List<ShaderProperty>();
            public bool showShaderProperty = false;
        }

        CheckItem shaderMaxLod;
        CheckItem shaderRenderQueue;
        CheckItem shaderPropertyCount;

        public override void InitCheckItem()
        {
            checkerName = "Shader";
            checkerFilter = "t:Shader";
            enableReloadCheckItem = true;
            shaderPropertyCount = new CheckItem(this, "PropertyCount", 100, CheckType.Int, OnButtonShowPropertyClick);
            shaderMaxLod = new CheckItem(this, "MaximumLOD", 100, CheckType.Int);
            shaderRenderQueue = new CheckItem(this, "RenderQueue", 100, CheckType.Int);
            nameItem.width = 350;
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            Shader shader = obj as Shader;
            if (shader == null)
                return;
            ObjectDetail detail = null;
            foreach (var v in CheckList)
            {
                if (v.checkObject == obj)
                    detail= v;
            }
            if (detail == null)
            {
                detail = new ShaderDetail(obj, this);
            }
            detail.AddObjectReference(refObj, detailRefObj);
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat != null && mat.shader != null)
                    {
                        if (checkModule is SceneResCheckModule)
                            AddObjectDetail(mat.shader, r.gameObject, null);
                        else if (checkModule is ReferenceResCheckModule)
                            AddObjectDetail(mat.shader, rootObj, r.gameObject);
                    }
                }
            }
        }

        public override void ShowChildDetail(ObjectDetail detail)
        {
            ShaderDetail shaderDetail = detail as ShaderDetail;
            if (!shaderDetail.showShaderProperty)
                return;
            for (int i = 0; i < shaderDetail.propertyList.Count; i++)
            {
                ShaderProperty property = shaderDetail.propertyList[i];
                if (property == null)
                    continue;
                GUILayout.BeginHorizontal();
                GUILayout.Space(720);
                GUILayout.Label(property.name, GUILayout.Width(100));
                GUILayout.Label(property.type, GUILayout.Width(100));
                GUILayout.Label(property.description, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
        }

        private void OnButtonShowPropertyClick(ObjectDetail detail)
        {
            ShaderDetail sd = detail as ShaderDetail;
            sd.showShaderProperty = !sd.showShaderProperty;
        }
    }
}
