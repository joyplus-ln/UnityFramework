using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ResourceCheckerPlus
{
    public class ComponentChecker : ObjectChecker
    {
        public class ComponentChildDetail
        {
            public Component component;
            public bool enabled;
        }

        public class ComponentDetail : ObjectDetail
        {
            public ComponentDetail(Object obj, ComponentChecker checker) : base(obj, checker)
            {
                
            }

            public override void InitDetailCheckObject(Object obj)
            {
                ComponentChecker checker = currentChecker as ComponentChecker;
                if (obj != null)
                {
                    assetName = obj is MonoScript ? obj.name : obj.GetType().ToString();
                }
                else
                {
                    assetName = "MissingComponent";
                }
                checkMap[checker.nameItem] = assetName;
                checkMap[checker.totalRefItem] = totalRef;
                string path = buildInType;
                if (obj is MonoScript)
                    path = AssetDatabase.GetAssetPath(obj);
                else if (obj is MonoBehaviour)
                    path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(obj as MonoBehaviour));
                checkMap[checker.pathItem] = path;
                checkMap[checker.comEnabledItem] = "true";
            }

            public bool allRefIsEnabled = true;
            public List<ComponentChildDetail> childList = new List<ComponentChildDetail>();
        }

        public CheckItem comEnabledItem;

        public override void InitCheckItem()
        {
            checkerName = "Component";
            checkerFilter = "t:Script";
            comEnabledItem = new CheckItem(this, "Enabled", 80, CheckType.String);
        }

        private bool IsComponentEnabled(Object obj)
        {
            Component com = obj as Component;
            if (com == null)
                return true;
            PropertyInfo info = com.GetType().GetProperty("enabled");
            if (info == null)
                return true;
            return (bool)info.GetValue(com, null);
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            Component com = obj as Component;
            if (com == null)
            {
                MonoScript script = obj as MonoScript;
                if (script == null)
                    return;
            }
            ComponentDetail detail = null;
            foreach (var d in CheckList)
            {
                if (d == null)
                    continue;
                if (d.checkObject is Component)
                {
                    if (d.checkObject.GetType() == obj.GetType())
                        detail = d as ComponentDetail;
                }
                else if (d.checkObject is MonoScript)
                {
                    if (d.checkObject == obj)
                        detail = d as ComponentDetail;
                }
            }
            if (detail == null)
            {
                detail = new ComponentDetail(obj, this);
            }
            detail.AddObjectReference(refObj, com);
            detail.allRefIsEnabled &= IsComponentEnabled(com);
            detail.checkMap[comEnabledItem] = detail.allRefIsEnabled.ToString(); 
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            AddObjectDetailRefInternal(rootObj, rootObj);
        }

        private void AddObjectDetailRefInternal(GameObject rootObj, GameObject checkObject)
        {
            Transform rootTran = checkObject.transform;
            Component[] coms = rootTran.GetComponents<Component>();
            foreach (var com in coms)
            {
                if (com != null)
                    AddObjectDetail(com, com.gameObject, null);
                else
                    AddNullComponentDetail(rootObj, checkObject);
            }
            for (int i = 0; i < rootTran.childCount; i++)
            {
                AddObjectDetailRefInternal(rootObj, rootTran.GetChild(i).gameObject);
            }
        }

        private void AddNullComponentDetail(GameObject rootGo, GameObject refGo)
        {
            ComponentDetail detail = null;
            foreach (var v in CheckList)
            {
                if (v.checkObject == null)
                    detail = v as ComponentDetail;
            }
            if (detail == null)
            {
                detail = new ComponentDetail(null, this);
                detail.AddObjectReference(rootGo, refGo);
                detail.isWarningDetail = true;
                detail.flag |= ObjectDetailFlag.Warning;
            }
        }
    }

    //TODO...
    //#region 检查任意组件任意属性字段的功能
    ////public class ComponentFiledEditor : EditorWindow
    ////{
    ////    private static ComponentDetails componentDetail = null;
    ////    public static void Init(ComponentDetails detail)
    ////    {
    ////        EditorWindow.GetWindow(typeof(ComponentFiledEditor));
    ////        componentDetail = detail;
    ////    }

    ////    public void OnGUI()
    ////    {

    ////        foreach (Component com in componentDetail.ComponentInstance)
    ////        {
    ////            PropertyInfo[] infoList = com.GetType().GetProperties();
    ////            FieldInfo[] fieldList = com.GetType().GetFields();
    ////            GUILayout.BeginHorizontal();
    ////            foreach (FieldInfo info in fieldList)
    ////            {
    ////                GUILayout.Label(info.Name);
    ////                PropertyInfo propertyInfo = com.GetType().GetProperty(info.Name);
    ////                GUILayout.Label(propertyInfo.GetValue(com, null).ToString());
    ////            }

    ////            GUILayout.EndHorizontal();
    ////        }
    ////    }
    ////}
    //#endregion
}
