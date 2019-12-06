using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ResourceCheckerPlus
{
    public class GameObjectChecker : ObjectChecker
    {
        public class GameObjectDetail : ObjectDetail
        {
            public GameObjectDetail(Object obj, GameObjectChecker checker) : base(obj, checker)
            {
                
            }

            public override void InitDetailCheckObject(Object obj)
            {
                GameObject go = obj as GameObject;
                GameObjectChecker checker = currentChecker as GameObjectChecker;
                bool isStatic = go.isStatic;
                StaticEditorFlags flag = GameObjectUtility.GetStaticEditorFlags(go);
                bool batchStatic = (flag & StaticEditorFlags.BatchingStatic) == StaticEditorFlags.BatchingStatic;
                bool lightmapStatic = (flag & StaticEditorFlags.ContributeGI) == StaticEditorFlags.ContributeGI;
                bool navigationStatic = (flag & StaticEditorFlags.NavigationStatic) == StaticEditorFlags.NavigationStatic;
                checkMap.Add(checker.goTag, go.tag);
                checkMap.Add(checker.goLayer, LayerMask.LayerToName(go.layer));
                checkMap.Add(checker.isStatic, isStatic.ToString());
                checkMap.Add(checker.batchStatic, batchStatic.ToString());
                checkMap.Add(checker.lightmapStatic, lightmapStatic.ToString());
                checkMap.Add(checker.navigaionStatic, navigationStatic.ToString());
                checkMap.Add(checker.staticFlag, (int)flag);
                CheckIsRefObjectActive(go);
            }
        }

        CheckItem goTag;
        CheckItem goLayer;
        CheckItem isStatic;
        CheckItem batchStatic;
        CheckItem lightmapStatic;
        CheckItem navigaionStatic;
        CheckItem staticFlag;

        public override void InitCheckItem()
        {
            checkerName = "GameObject";
            checkerFilter = "t:Prefab";
            enableReloadCheckItem = true;
            goTag = new CheckItem(this, "Tag", 100);
            goLayer = new CheckItem(this, "Layer", 100);
            isStatic = new CheckItem(this, "IsStatic", 100);
            batchStatic = new CheckItem(this, "BactcStatic", 100);
            lightmapStatic = new CheckItem(this, "LightMapStatic", 100);
            navigaionStatic = new CheckItem(this, "NavigationStatic", 100);
            staticFlag = new CheckItem(this, "StaticFlag", 100, CheckType.Int);
        }

        public override void AddObjectDetail(Object rootObj)
        {
            if (rootObj is GameObject)
            {
                new GameObjectDetail(rootObj, this);
            }
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            GameObject[] gos = rootObj.GetComponentsInChildren<Transform>(true).Select(x => x.gameObject).ToArray();
            foreach (var go in gos)
            {
                AddObjectDetail(go);
            }
        }
    }
}
