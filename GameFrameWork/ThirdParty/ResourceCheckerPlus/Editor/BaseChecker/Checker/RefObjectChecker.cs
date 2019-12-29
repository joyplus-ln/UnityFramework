using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class RefObjectChecker : ObjectChecker
    {
        public class RefObjectDetail : ObjectDetail
        {
            public RefObjectDetail(Object obj, RefObjectChecker checker) : base(obj, checker)
            {
                checkMap.Add(checker.refType, GetObjectType(obj));
            }

            private string GetObjectType(Object obj)
            {
                string type = obj == null ? "Null" : obj.GetType().ToString();
                if (type.Contains(unityEngineHead))
                    type = type.TrimStart(unityEngineHead.ToCharArray());
                return type;
            }

            private string unityEngineHead = "UnityEngine.";
        }

        CheckItem refType;

        public override void InitCheckItem()
        {
            checkerName = "RefObj";
            isSpecialChecker = true;
            refType = new CheckItem(this, "类型", 150);
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            ObjectDetail c = null;
            //先查缓存
            foreach (var checker in CheckList)
            {
                if (checker.checkObject == obj)
                    c = checker;
            }
            if (c == null)
            {
                c = new RefObjectDetail(obj, this);
            }
            if (refObj != null)
            {
                c.foundInReference.Add(refObj);
            }
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            Object[] objs = EditorUtility.CollectDependencies(new Object[] { rootObj });
            foreach (var o in objs)
            {
                if (checkModule is SceneResCheckModule)
                    AddObjectDetail(o, null, null);
                else if (checkModule is ReferenceResCheckModule)
                    AddObjectDetail(o, rootObj, null);
            }
        }
    }
}