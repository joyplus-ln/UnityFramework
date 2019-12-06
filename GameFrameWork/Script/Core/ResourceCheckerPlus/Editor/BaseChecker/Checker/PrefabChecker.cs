using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class PrefabChecker : ObjectChecker
    {
        public class PrefabDetail : ObjectDetail
        {
            public PrefabDetail(Object obj, PrefabChecker checker) : base(obj, checker)
            {
                checkMap[checker.totalRefItem] = totalRef;
            }
        }

        public override void InitCheckItem()
        {
            checkerName = "Prefab";
            checkerFilter = "t:Prefab";
            postfix = ".prefab";
            base.InitCheckItem();
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            GameObject go = obj as GameObject;
            if (go == null)
                return;
            Object prefab = PrefabUtility.FindPrefabRoot(go);
            //剔除prefab自身
            if (checkModule is ReferenceResCheckModule && prefab == refObj)
                return;
            PrefabDetail detail = null;
            foreach (var d in CheckList)
            {
                if (d.checkObject == prefab)
                    detail = d as PrefabDetail;
            }
            if (detail == null)
            {
                detail = new PrefabDetail(prefab, this);
            }
            detail.AddObjectReference(refObj, go);
        }
    }
}