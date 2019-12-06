using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class SceneChecker : ObjectChecker
    {
        public class SceneDetail : ObjectDetail
        {
            public SceneDetail(Object obj, ObjectChecker checker) : base(obj, checker)
            {

            }
        }

        public override void InitCheckItem()
        {
            checkerName = "Scene";
            checkerFilter = "t:Scene";
            postfix = ".unity";
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            ObjectDetail detail= null;
            foreach (var v in CheckList)
            {
                if (v.checkObject == obj)
                   detail = v;
            }
            if (detail == null)
            {
               detail = new SceneDetail(obj, this);
            }
            detail.AddObjectReference(refObj, detailRefObj);
        }
    }
}
