using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class RefFilterItem : FilterItem
    {
        public List<Object> checkObjList = null;
        public RefFilterItem(ObjectChecker c) : base(c)
        {

        }

        //Linq有没有类似的实现?.....
        public override List<ObjectDetail> CustomDoFilter(List<ObjectDetail> inList)
        {
            List<ObjectDetail> objDetail = new List<ObjectDetail>();
            foreach (var obj in inList)
            {
                bool need = false;
                foreach (var v in checkObjList)
                {
                    if (obj.foundInReference.Contains(v))
                    {
                        need = true;
                    }
                }
                if (positive ? need : !need)
                    objDetail.Add(obj);
            }
            return objDetail;
        }

        public override void CustomShowFilter()
        {
            GUILayout.BeginHorizontal();
            if (checkObjList != null && checkObjList.Count > 0)
            {
                string label = checkObjList[0].ToString() + "引用的资源";
                GUILayout.Label(label, GUILayout.Width(450));
            }
            EditorGUI.BeginChangeCheck();
            positive = GUILayout.Toggle(positive, positive ? "正向" : "反向", GUILayout.Width(40));
            if (EditorGUI.EndChangeCheck())
            {
                checker.RefreshCheckResult();
            }

            if (parentFilterNode == null)
            {
                if (GUILayout.Button("增加筛选", GUILayout.Width(60)))
                {
                    AddFilterNode(new FilterItem(checker));
                }
            }
            else
            {
                if (GUILayout.Button("删除筛选", GUILayout.Width(60)))
                {
                    RemoveFilterNode();
                    checker.RefreshCheckResult();
                }
            }
            GUILayout.EndHorizontal();
        }

    }
}