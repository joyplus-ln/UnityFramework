
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace ResourceCheckerPlus
{
    public enum ObjectDetailFlag
    {
        None = 1,
        Warning = 2,
    }

    [System.Serializable]
    public class CheckerCfg
    {
        public string checkerName;
        public bool checkerEnabled;

        public CheckerCfg(string name, bool enabled = false)
        {
            checkerName = name;
            checkerEnabled = enabled;
        }
    }

    public class ObjectDetail
    {
        public Object checkObject;
        public string assetName;
        public string assetPath;
        public string assetGUID;
        public List<Object> foundInReference = new List<Object>();
        public List<Object> totalRef = new List<Object>();
        public Dictionary<CheckItem, object> checkMap = new Dictionary<CheckItem, object>();
        public bool isSelected = false;
        public bool isWarningDetail = false;
        public ObjectChecker currentChecker = null;
        public bool refObjectEnabled = true;
        public ObjectDetailFlag flag = ObjectDetailFlag.None;

        public static string buildInType = "BuildIn";

        public ObjectDetail(Object obj, ObjectChecker checker)
        {
            assetPath = AssetDatabase.GetAssetPath(obj);
            assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            checker.CheckList.Add(this);
            currentChecker = checker;
            InitCheckObject(obj);
        }

        public void AddObjectReference(Object refObj, Object detailRefObj)
        {
            if (refObj != null && !foundInReference.Contains(refObj))
            {
                foundInReference.Add(refObj);
            }
            if (detailRefObj != null && !totalRef.Contains(detailRefObj))
            {
                totalRef.Add(detailRefObj);
            }
            CheckIsRefObjectActive(refObj);
            CheckIsRefObjectActive(detailRefObj);
        }

        public bool CheckIsRefObjectActive(Object refObj)
        {
            GameObject go = null;
            bool result = true;
            if (refObj is GameObject)
                go = refObj as GameObject;
            else if (refObj is Component)
                go = (refObj as Component).gameObject;
            if (go != null)
            {
                result = (currentChecker.checkModule is SceneResCheckModule) ? go.activeInHierarchy : go.activeSelf;
                refObjectEnabled &= result;
                checkMap[currentChecker.activeItem] = refObjectEnabled.ToString();
            }
            return result;
        }

        public void InitCheckObject(Object obj)
        {
            ObjectChecker checker = currentChecker;
            checkMap.Clear();
            checkObject = obj;
            assetName = obj == null ? "Null" : obj.name;
            assetPath = AssetDatabase.GetAssetPath(obj);
            Texture preview = AssetPreview.GetMiniThumbnail(obj);
            checkMap.Add(checker.nameItem, assetName);
            checkMap.Add(checker.refItem, foundInReference);
            checkMap.Add(checker.totalRefItem, totalRef);
            checkMap.Add(checker.pathItem, assetPath);
            checkMap.Add(checker.previewItem, preview);
            checkMap.Add(checker.activeItem, refObjectEnabled.ToString());
            foreach (var v in checker.customCheckItems)
            {
                checkMap.Add(v, v.GetCheckValue(obj));
            }
            InitDetailCheckObject(obj);
        }

        public void ReloadCheckObject()
        {
            string path = AssetDatabase.GUIDToAssetPath(assetGUID);
            if (!path.Contains("unity_builtin_extra") && !path.Contains("unity default resources"))
            {
                InitCheckObject(AssetDatabase.LoadAssetAtPath<Object>(path));
            }
        }

        public virtual void InitDetailCheckObject(Object obj) { }
    }

    /// <summary>
    /// 检查基类
    /// </summary>
    public class ObjectChecker
    {
        public List<CheckItem> checkItem = new List<CheckItem>();
        public List<CheckItem> customCheckItems = new List<CheckItem>();
        public List<ObjectDetail> CheckList = new List<ObjectDetail>();
        public List<ObjectDetail> FilterList = new List<ObjectDetail>();
        public List<ObjectDetail> SelectList = new List<ObjectDetail>();
        public PredefineFilterGroup predefineFilterGroup = null;
        public FilterItem filterItem = null;
        public ResCheckModuleBase checkModule = null;
        public CheckerCfg config = null;
        public Vector2 viewListScrollPos = Vector2.zero;
        public bool ctrlPressed = false;
        public bool shiftPressed = false;
        public int checkItemHeight = 42;
        public string checkSummary = "";
        public string checkerName = "Object";
        public string checkerFilter = "t:Object";
        public string postfix = "";
        public static string platformIOS = "iPhone";
        public static string platformAndroid = "Android";
        public static string platformStandalone = "Standalone";
        public bool enableReloadCheckItem = false;
        protected bool isReloadCheckItem = false;
        public bool isSpecialChecker = false;

        private int firstVisible = 0;
        private int lastVisible = 0;
        private bool _checkerEnabled = false;
        public bool checkerEnabled
        {
            get { return _checkerEnabled; }
            set
            {
                _checkerEnabled = value;
                if (config != null)
                    config.checkerEnabled = value;
            }
        }

        public CheckItem previewItem;
        public CheckItem nameItem;
        public CheckItem refItem;
        public CheckItem totalRefItem;
        public CheckItem pathItem;
        public CheckItem activeItem;

        public ObjectChecker()
        {
            previewItem = new CheckItem(this, "预览", 40, CheckType.Texture); previewItem.order = -99; //previewItem.show = false;
            nameItem = new CheckItem(this, "名称", 200, CheckType.String, OnNameButtonClick);
            refItem = new CheckItem(this, "引用", 80, CheckType.List, OnRefButtonClick);
            totalRefItem = new CheckItem(this, "具体引用", 80, CheckType.List, OnDetailRefButton);
            activeItem = new CheckItem(this, "引用对象全部激活", 120); activeItem.order = 98;
            pathItem = new CheckItem(this, "路径", 999); pathItem.order = 99;

            InitCheckItem();
            //根据配置文件反射初始化的扩展项，暂时注掉
            InitCustomCheckItem();

            checkItem.Sort(delegate (CheckItem item1, CheckItem item2) { return item1.order - item2.order; });
            filterItem = new FilterItem(this);
        }

        #region 选中处理

        public List<ObjectDetail> GetSelectObjectDetails()
        {
            return SelectList;
        }

        public void SelectObjectDetail(ObjectDetail detail)
        {
            bool current = !detail.isSelected;
            foreach (var v in SelectList)
                v.isSelected = false;
            if (shiftPressed)
            {
                ShiftSelectObject(detail);
            }
            else
            {
                if (!ctrlPressed)
                    SelectList.Clear();
                if (current)
                    SelectList.Add(detail);
                else
                    SelectList.Remove(detail);
            }
            List<Object> list = new List<Object>();
            foreach (var v in SelectList)
            {
                list.Add(v.checkObject);
                v.isSelected = true;
            }
            Selection.objects = list.ToArray();
        }

        public void ShiftSelectObject(ObjectDetail detail)
        {
            if (SelectList.Count > 0)
            {
                int start = SelectList.Min(x => FilterList.IndexOf(x));
                int end = FilterList.IndexOf(detail);
                ClearSelect();
                if (end > start)
                {
                    SelectList.AddRange(FilterList.GetRange(start, end - start + 1));
                }
                else
                {
                    SelectList.AddRange(FilterList.GetRange(end, start - end + 1));
                }
            }
        }

        public void SelectAll()
        {
            SelectList.Clear();
            SelectList.AddRange(FilterList);
            foreach (var v in SelectList)
                v.isSelected = true;
            Selection.objects = SelectList.Select(x => x.checkObject).ToArray();
        }

        public void ClearSelect()
        {
            foreach (var v in SelectList)
                v.isSelected = false;
            SelectList.Clear();
        }

        public void SelectObject(Object selectedObject)
        {
            if (ctrlPressed)
            {
                List<Object> currentSelection = new List<Object>(Selection.objects);
                if (currentSelection.Contains(selectedObject))
                    currentSelection.Add(selectedObject);
                else
                    currentSelection.Remove(selectedObject);
                Selection.objects = currentSelection.ToArray();
            }
            else Selection.activeObject = selectedObject;
        }

        public void SelectObjects(List<Object> selectedObjects)
        {
            if (ctrlPressed)
            {
                List<Object> currentSelection = new List<Object>(Selection.objects);
                currentSelection.AddRange(selectedObjects);
                Selection.objects = currentSelection.ToArray();
            }
            else Selection.objects = selectedObjects.ToArray();
        }

        public void RemoveSelectObjFromFilterList()
        {
            List<ObjectDetail> selection = GetSelectObjectDetails();
            FilterList = FilterList.Where(x => !selection.Contains(x)).ToList();
            ClearSelect();
            RefreshCheckResult();
        }

        public List<Object> GetBatchOptionList()
        {
            List<ObjectDetail> list = CheckerConfigManager.checkerConfig.batchOptionType == BatchOptionSelection.CurrentSelect ? SelectList : FilterList;
            return list.Select(x => x.checkObject).ToList();
        }
        #endregion

        public virtual void InitCheckItem() { }

        public virtual void AddObjectDetail(Object rootObj) { }

        public virtual void ShowChildDetail(ObjectDetail detail) { }

        public virtual void AddObjectDetail(Object obj, Object refObj, Object detailRefObj) { }

        public virtual void AddObjectDetailRef(GameObject rootObj) { }

        public virtual void SceneResCheck(GameObject rootObj)
        {
            Component[] components = rootObj.GetComponentsInChildren<Component>(true);
            components = components.Where(x => !(x is Transform)).ToArray();
            GameObject[] gos = components.Select(x => x.gameObject).Distinct().ToArray();
            foreach (var v in gos)
            {
                Object[] dependency = EditorUtility.CollectDependencies(new Object[] { v });
                foreach (var dep in dependency)
                {
                    AddObjectDetail(dep, v.gameObject, null);
                }
            }
        }

        public virtual void DirectResCheck(Object[] selection)
        {
            List<Object> objects = GetAllDirectCheckObjectFromInput(selection, checkerFilter);
            if (objects != null && objects.Count > 0)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    Object o = objects[i];
                    if (EditorUtility.DisplayCancelableProgressBar("正在检查" + checkerName + "类型资源", "已完成：" + i + "/" + objects.Count, (float)i / objects.Count))
                        break;
                    AddObjectDetail(o);
                    AddObjectDetail(o, null, null);
                }
                EditorUtility.ClearProgressBar();
            }
        }

        public virtual void ReferenceResCheck(Object[] selection, string filter, bool checkPrefabDetailRef)
        {
            List<Object> objects = GetAllRefCheckObjectFromInput(selection, filter);
            if (objects != null && objects.Count > 0)
            {
                //加入全部查找列表
                checkModule.AddObjectToSideBarList(objects);
                //进行遍历检查
                for (int i = 0; i < objects.Count; i++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("正在检查" + filter + "引用的" + checkerName + "类型资源", "已完成：" + i + "/" + objects.Count, (float)i / objects.Count))
                        break;
                    Object root = objects[i];
                    if (root == null)
                        continue;
                    ReferenceResCheck(root, checkPrefabDetailRef);
                }
                EditorUtility.ClearProgressBar();
            }
        }

        public void ReferenceResCheck(Object root, bool checkPrefabDetailRef)
        {
            if (root is SceneAsset)
            {
                CheckSceneReference(root);
            }
            else if (root is Material)
            {
                CheckMaterialReference(root);
            }
            else if (root is GameObject)
            {
                CheckPrefabReference(root, checkPrefabDetailRef);
            }
        }

        public void CheckSceneReference(Object root)
        {
            string path = AssetDatabase.GetAssetPath(root);
            string[] dependency = AssetDatabase.GetDependencies(path);
            foreach (var dep in dependency)
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                AddObjectDetail(obj, root, null);
                if (dep.EndsWith(".FBX") || dep.EndsWith(".obj"))
                {
                    Object[] fbxDep = EditorUtility.CollectDependencies(new Object[] { obj });
                    foreach (var fDep in fbxDep)
                    {
                        if (fDep is Mesh || fDep is AnimationClip)
                        {
                            AddObjectDetail(fDep, root, null);
                        }
                    }
                }
            }
        }

        public void CheckMaterialReference(Object root)
        {
            Object[] dependencies = EditorUtility.CollectDependencies(new Object[] { root });
            AddObjectDetail(root);
            //检查每个prefab的dependencies
            foreach (Object depend in dependencies)
            {
                AddObjectDetail(depend, root, null);
            }
        }

        public void CheckPrefabReference(Object root, bool checkDetailRef)
        {
            if (checkDetailRef)
            {
                GameObject go = root as GameObject;
                if (go != null)
                {
                    AddObjectDetailRef(go);
                }
            }
            else
            {
                Object[] dependencies = EditorUtility.CollectDependencies(new Object[] { root });
                AddObjectDetail(root);
                //检查每个prefab的dependencies
                foreach (Object depend in dependencies)
                {
                    AddObjectDetail(depend, root, null);
                }
            }
        }

        public virtual void ReverseRefResCheck(Object[] selection)
        {
            List<Object> objects = GetAllRefCheckObjectFromInput(selection, "t:Object");
            if (objects == null || objects.Count == 0 || string.IsNullOrEmpty(postfix))
                return;
            //加入全部查找列表
            checkModule.AddObjectToSideBarList(objects);
            //获取当前所有后缀为当前checker的资源路径
            string[] checkAssetPath = AssetDatabase.GetAllAssetPaths().Where(x => x.EndsWith(postfix)).ToArray();
            for (int i = 0; i < checkAssetPath.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("正在反向检查位于" + checkerFilter + "类型资源引用", "已完成：" + i + "/" + checkAssetPath.Length, (float)i / checkAssetPath.Length))
                    break;
                string assetPath = checkAssetPath[i];
                if (string.IsNullOrEmpty(assetPath))
                    continue;
                string[] depends = AssetDatabase.GetDependencies(assetPath);
                foreach (var depend in depends)
                {
                    foreach (var obj in objects)
                    {
                        string tempPath = AssetDatabase.GetAssetPath(obj);
                        //排除自身
                        if (tempPath == depend && tempPath != assetPath)
                        {
                            //与正常的查询反过来，foundinreference是被查找的资源，前面的是引用被查找资源的资源
                            AddObjectDetail(AssetDatabase.LoadAssetAtPath<Object>(assetPath), obj, null);
                        }
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

        public void MixCheckDirectAndRefRes(Object root)
        {
            if (root is SceneAsset)
            {
                CheckSceneReference(root);
            }
            else if (root is Material)
            {
                CheckMaterialReference(root);
            }
            else if (root is GameObject)
            {
                CheckPrefabReference(root, false);
            }
            else
            {
                AddObjectDetail(root);
                AddObjectDetail(root, null, null);
            }
        }

        public void AddObjectDetailBatch(List<Object> objList)
        {
            foreach (var o in objList)
            {
                AddObjectDetail(o);
                AddObjectDetail(o, null, null);
            }
        }

        public void InitCustomCheckItem()
        {
            CheckerConfig cfg = ResourceCheckerPlus.instance.configManager.GetCheckerConfig(checkerName);
            if (cfg.checkItemCfg == null)
                return;
            foreach(var v in cfg.checkItemCfg)
            {
                CheckItem item = CheckItem.CreateCheckItemFromConfig(this,v);
                if (item != null)
                {
                    customCheckItems.Add(item);
                }
            }
        }

        public virtual void CheckDetailSummary()
        {
            checkSummary = FilterList.Count + " " + checkerName;
        }

        public virtual void BatchSetResConfig()
        {
            EditorUtility.DisplayDialog("提示", "该类型资源不支持批量修改，可以进行批量选中然后手动修改", "OK");
        }

        public string FormatSizeString(int memSizeKB)
        {
            if (memSizeKB < 1024) return "" + memSizeKB + "k";
            else
            {
                float memSizeMB = ((float)memSizeKB) / 1024.0f;
                return memSizeMB.ToString("0.00") + "Mb";
            }
        }

        public void Clear()
        {
            viewListScrollPos = Vector2.zero;
            ClearSelect();
            CheckList.Clear();
            FilterList.Clear();
            filterItem.Clear(true);
            CheckDetailSummary();
        }

        public void Recover()
        {
            ClearSelect();
            FilterList.Clear();
            FilterList.AddRange(CheckList);
            filterItem.Clear(true);
            CheckDetailSummary();
        }

        public void CheckDetailSort(CheckItem item, bool positive)
        {
            switch (item.type)
            {
                case CheckType.String:
                    FilterList.Sort(delegate (ObjectDetail check1, ObjectDetail check2)
                    {
                        if (positive)
                            return string.Compare((string)check2.checkMap[item], (string)check1.checkMap[item]);
                        return string.Compare((string)check1.checkMap[item], (string)check2.checkMap[item]);
                    });
                    break;
                case CheckType.Int:
                case CheckType.FormatSize:
                    FilterList.Sort(delegate (ObjectDetail check1, ObjectDetail check2)
                    {
                        if (positive)
                            return (int)check1.checkMap[item] - (int)check2.checkMap[item];
                        return (int)check2.checkMap[item] - (int)check1.checkMap[item];
                    });
                    break;
                case CheckType.Float:
                    FilterList.Sort(delegate (ObjectDetail check1, ObjectDetail check2)
                    {
                        //float排序,暂时木有找到什么好方法....
                        float val1 = (float)check1.checkMap[item] * 10000;
                        float val2 = (float)check2.checkMap[item] * 10000;
                        if (positive)
                            return (int)val1 - (int)val2;
                        return (int)val2 - (int)val1;
                    });
                    break;
                case CheckType.List:
                    FilterList.Sort(delegate (ObjectDetail check1, ObjectDetail check2)
                    {
                        List<Object> check1List = check1.checkMap[item] as List<Object>;
                        List<Object> check2List = check2.checkMap[item] as List<Object>;
                        if (positive)
                            return check1List.Count - check2List.Count;
                        return check2List.Count - check1List.Count;
                    });
                    break;
                default:
                    return;
            }
        }

        public void ShowCheckDetail(ObjectDetail tCheck, CheckItem item)
        {
            if (item.show == false || !tCheck.checkMap.ContainsKey(item))
                return;
            if (item.type == CheckType.Texture)
            {
                Texture tex = tCheck.checkMap[item] as Texture;
                if (tex == null)
                    GUILayout.Box("null", GUILayout.Width(item.width), GUILayout.Height(item.width));
                else
                {
                    GUILayout.Box(tex, GUILayout.Width(item.width), GUILayout.Height(item.width));
                }
            }
            else
            {
                string label = null;
                if (item.type == CheckType.FormatSize)
                {
                    label = FormatSizeString((int)tCheck.checkMap[item]);
                }
                else if (item.type == CheckType.List)
                {
                    List<Object> list = tCheck.checkMap[item] as List<Object>;
                    label = list.Count.ToString();
                }
                else
                {
                    label = tCheck.checkMap[item].ToString();
                }
                if (item.clickOption == null)
                {
                    GUILayout.Label(label, GUILayout.Width(item.width));
                }
                else
                {
                    if (GUILayout.Button(label, GUILayout.Width(item.width)))
                    {
                        SelectObjectDetail(tCheck);
                        item.clickOption(tCheck);
                    }
                }
            }
        }

        public void RefreshCheckResult()
        {
            FilterCheckResult();
            CheckDetailSummary();
            ClearSelect();
        }

        public virtual void ShowCheckerFliter()
        {
            filterItem.ShowFilter();
        }

        public void FilterCheckResult()
        {
            FilterList = filterItem.CheckDetailFilter(CheckList);
        }

        public virtual void ShowCheckerTitle()
        {
            GUILayout.BeginHorizontal();
            string title = CheckList.Count == 0 ? checkModule.checkModeName.tooltip : checkSummary;
            GUILayout.Label(title);
            if (GUILayout.Button("设置", GUILayout.Width(40)))
            {
                ConfigWindow.Init();
            }
            GUILayout.EndHorizontal();
        }

        public virtual void ShowCheckerSort()
        {
            viewListScrollPos.x = EditorGUILayout.BeginScrollView(new Vector2(viewListScrollPos.x, 0), GUILayout.Height(40)).x;
            GUILayout.BeginHorizontal();
            foreach (var item in checkItem)
            {
                if (item.show == false)
                    continue;
                if (GUILayout.Button(item.title, GUILayout.Width(item.width)))
                {
                    CheckDetailSort(item, item.sortSymbol);
                    //点击之后反向排序
                    item.sortSymbol = !item.sortSymbol;
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        public virtual void ShowCheckResult()
        {
            ShowCheckerFliter();
            ShowCheckerSort();
            ShowCheckDetailResult();
        }

        private void OnKeyBoardEvent()
        {
            Event evt = Event.current;
            //事件响应
            ctrlPressed = evt.control || evt.command;
            shiftPressed = evt.shift;
            if (evt.isKey && evt.type == EventType.KeyDown && !(this is SelfObjectChecker))
            {
                OnUpDownButtonSelect(evt.keyCode);
            }
        }

        private void OnUpDownButtonSelect(KeyCode key)
        {
            List<ObjectDetail> list = GetSelectObjectDetails();
            ObjectDetail nextSelect = null;
            if (list.Count > 0)
            {
                ObjectDetail detail = list[0];
                int index = FilterList.IndexOf(detail);
                if (key == KeyCode.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        nextSelect = FilterList[index - 1];
                        viewListScrollPos.y -= 41;
                    }
                }
                else if (key == KeyCode.DownArrow)
                {
                    if (index + 1 < FilterList.Count)
                    {
                        nextSelect = FilterList[index + 1];
                        viewListScrollPos.y += 41;
                    }
                }
                if (nextSelect != null)
                {
                    ClearSelect();
                    SelectObjectDetail(nextSelect);
                    ResourceCheckerPlus.instance.Repaint();
                }
            }
            else
            {
                if (FilterList.Count > 0)
                {
                    ObjectDetail detail = FilterList[0];
                    ClearSelect();
                    SelectObjectDetail(detail);
                    viewListScrollPos.y = 0;
                    ResourceCheckerPlus.instance.Repaint();
                }
            }
        }

        public void ShowCheckDetailResult()
        {
            OnKeyBoardEvent();
            //Unity在GUI中改变界面状态时，渲染界面会出错
            //http://answers.unity3d.com/questions/400454/argumentexception-getting-control-0s-position-in-a-1.html
            if (Event.current.type == EventType.Layout)
            {
                OptimizeCheckResult();
            }
            viewListScrollPos = EditorGUILayout.BeginScrollView(viewListScrollPos);
            {
                for (int i = 0; i < FilterList.Count; i++)
                {
                    ObjectDetail detail = FilterList[i];
                    GUILayout.BeginHorizontal();
                    if (IsItemVisible(i))
                    {
                        ShowCheckRowItem(detail);
                    }
                    else
                    {
                        GUILayout.Label(" ", GUILayout.Height(previewItem.width));
                    }
                    GUILayout.EndHorizontal();
                    ShowChildDetail(detail);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public void ShowCheckRowItem(ObjectDetail detail)
        {
            if (detail.isWarningDetail)
                GUI.color = CheckerConfigManager.checkerConfig.warningItemColor;
            if (detail.isSelected)
                GUI.color = CheckerConfigManager.checkerConfig.highlightTextColor;
            //通用CheckListUI
            checkItem.ForEach(item => ShowCheckDetail(detail, item));
            GUI.color = CheckerConfigManager.defaultTextColor;
        }

        public virtual List<Object> GetAllDirectCheckObjectFromInput(Object[] selection, string filter)
        {
            return GetAllObjectFromInput<Object>(selection, filter);
        }

        public virtual List<Object> GetAllRefCheckObjectFromInput(Object[] selection, string filter)
        {
            return GetAllObjectFromInput<Object>(selection, filter);
        }

        public static List<Object> GetAllObjectFromInput<T>(Object[] objects, string filter) where T : Object
        {
            List<string> pathFolderList = new List<string>();
            List<Object> objlist = new List<Object>();
            List<string> singleObjList = new List<string>();
            for (int i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];
                if (obj == null)
                    continue;
                string temPath = AssetDatabase.GetAssetPath(obj);
                if (ResourceCheckerHelper.isFolder(temPath))
                {
                    pathFolderList.Add(temPath);
                }
                else if (obj is T)
                {
                    objlist.Add(obj);
                }
            }
            if (pathFolderList.Count > 0)
            {
                string[] guids = AssetDatabase.FindAssets(filter, pathFolderList.ToArray());
                singleObjList.AddRange(guids.Select(x => AssetDatabase.GUIDToAssetPath(x)));
            }

            for (int i = 0; i < singleObjList.Count; i++)
            {
                string s = singleObjList[i];
                if (EditorUtility.DisplayCancelableProgressBar("正在加载" + filter + "类型资源", "已完成：" + i + "/" + singleObjList.Count, (float)i / singleObjList.Count))
                {
                    break;
                }
                objlist.Add(AssetDatabase.LoadAssetAtPath<T>(s));
            }
            EditorUtility.ClearProgressBar();
            return objlist;
        }

        public void MoveAssetToPath()
        {
            List<Object> objects = GetBatchOptionList();
            MoveAssetEditor.Init(objects);
        }

        public void AddSelectObjectToCustomList()
        {
            List<Object> objects = GetSelectObjectDetails().Select(x => x.checkObject).ToList();
            checkModule.AddObjectToSideBarList(objects);
        }

        public void OnContexMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            if (enableReloadCheckItem)
                genericMenu.AddItem(new GUIContent("刷新"), false, new GenericMenu.MenuFunction(ReloadCheckDetail));
            genericMenu.AddItem(new GUIContent("全选"), false, new GenericMenu.MenuFunction(SelectAll));
            genericMenu.AddItem(new GUIContent("取消"), false, new GenericMenu.MenuFunction(ClearSelect));
            genericMenu.AddItem(new GUIContent("批量移动到指定目录"), false, new GenericMenu.MenuFunction(MoveAssetToPath));
            genericMenu.AddItem(new GUIContent("批量修改格式"), false, new GenericMenu.MenuFunction(BatchSetResConfig));
            genericMenu.AddItem(new GUIContent("检查结果导出Excel表"), false, new GenericMenu.MenuFunction(ExportCurrentCheckerResult));
            genericMenu.AddItem(new GUIContent("保存筛选", "将当前筛选条件存储起来，可以通过载入筛选直接加载"), false, new GenericMenu.MenuFunction(SaveCheckFilter));
            genericMenu.AddItem(new GUIContent("载入筛选", "载入之前存储的筛选条件"), false, new GenericMenu.MenuFunction(LoadCheckFilter));
            genericMenu.AddItem(new GUIContent("将筛选结果作为原始结果", "将当前结果作为检查的原始结果，并清理掉所有筛选条件"), false, new GenericMenu.MenuFunction(SetCurFilterResultAsSource));
            genericMenu.ShowAsContext();
        }

        public virtual void ShowOptionButton()
        {
            if (GUILayout.Button("其他功能", GUILayout.Width(60)))
            {
                OnContexMenu();
            }
        }

        private void OnNameButtonClick(ObjectDetail detail)
        {
            //回调之前带了选中的功能，此处空实现
        }

        private void OnRefButtonClick(ObjectDetail detail)
        {
            checkModule.AddObjectToSideBarList(detail.foundInReference);
            SelectObjects(detail.foundInReference);
        }

        private void OnDetailRefButton(ObjectDetail detail)
        {
            checkModule.AddObjectToSideBarList(detail.totalRef);
            SelectObjects(detail.totalRef);
        }

        //当检查条目达到上千级别时，滚动条会很卡，目前的策略是看不见的就不显示了。貌似Unity5.6直接就有类似的控件
        //目前3000左右没有问题，不过上万之后还是会很卡
        //感谢这位老哥 http://blog.csdn.net/akof1314/article/details/70285033
        public void OptimizeCheckResult()
        {
            float y = viewListScrollPos.y;
            //可能还需要减去高度，不过多显示一两个无所谓了...
            float height = ResourceCheckerPlus.instance.position.height;
            int first = (int)Mathf.Floor(y / checkItemHeight);
            int last = first + (int)Mathf.Ceil(height / checkItemHeight);
            firstVisible = Mathf.Max(first - 10, 0);
            lastVisible = Mathf.Min(last, FilterList.Count - 1);
        }

        private bool IsItemVisible(int id)
        {
            if (id >= firstVisible && id <= lastVisible)
                return true;
            return false;
        }

        public static void CreateChecker(ResCheckModuleBase module, CheckerCfg cfg)
        {
            System.Type type = System.Type.GetType("ResourceCheckerPlus." + cfg.checkerName);
            if (type == null)
                return;
            ObjectChecker checker = System.Activator.CreateInstance(type) as ObjectChecker;
            checker.checkModule = module;
            checker.checkerEnabled = cfg.checkerEnabled;
            checker.config = cfg;
            module.checkerList.Add(checker);
        }

        public void ExportCheckResult(string path)
        {
            StreamWriter sw = new StreamWriter(path + "/" + checkerName + "CheckResult.txt", true, System.Text.Encoding.Default);
            string checkTitle = "";
            foreach (var v in checkItem)
            {
                if (v.type == CheckType.Texture)
                    continue;
                checkTitle += v.title + "\t";
            }
            sw.WriteLine(checkTitle);
            for (int i = 0; i < FilterList.Count; i++)
            {
                ObjectDetail detail = FilterList[i];
                string line = "";
                checkItem.ForEach(item => GenerateDetailLine(ref line, detail, item));
                sw.WriteLine(line);
            }
            sw.Close();
        }

        private void GenerateDetailLine(ref string line, ObjectDetail detail, CheckItem item)
        {
            if (item.type != CheckType.Texture)
            {
                if (item.type == CheckType.FormatSize)
                {
                    line += FormatSizeString((int)detail.checkMap[item]);
                }
                else if (item.type == CheckType.List)
                {
                    List<Object> list = detail.checkMap[item] as List<Object>;
                    line += list.Count.ToString();
                }
                else
                {
                    line += detail.checkMap[item].ToString().Replace("\n", "").Replace(" ", "");
                }
                line += "\t";
            }
        }

        private void ExportCurrentCheckerResult()
        {
            string path = ResourceCheckerHelper.GenericExportFolderName();
            Directory.CreateDirectory(path);
            ExportCheckResult(path);
            AssetDatabase.Refresh();
        }

        public void CreateCheckFilter(PredefineFilterGroup filterGroup)
        {
            filterItem.CreateFromFilterGroup(filterGroup);
            RefreshCheckResult();
        }

        public void SaveCheckFilter()
        {
            predefineFilterGroup = filterItem.SaveAsFilterGroup();
            predefineFilterGroup.filterGroupName = checkerName;
            string path = CheckerConfigManager.configRootPath + CheckerConfigManager.predefineFilterCfgPath;
            if (!ResourceCheckerHelper.isFolder(path))
            {
                AssetDatabase.CreateFolder(CheckerConfigManager.configRootPath, "FilterCfg");
            }
            AssetDatabase.CreateAsset(predefineFilterGroup, path + "/" + predefineFilterGroup.filterGroupName + ".asset");
        }

        public void ClearCheckFilter()
        {
            filterItem.Clear(true);
        }

        public CheckItem GetCheckItemByName(string name)
        {
            return checkItem.First(x => x.title == name);
        }

        public void LoadCheckFilter()
        {
            string path = CheckerConfigManager.configRootPath + CheckerConfigManager.predefineFilterCfgPath + "/" + checkerName + ".asset";
            predefineFilterGroup = AssetDatabase.LoadAssetAtPath<PredefineFilterGroup>(path);
            CreateCheckFilter(predefineFilterGroup);
        }

        //清除全部筛选条件，将当前筛选结果作为原始结果
        public void SetCurFilterResultAsSource()
        {
            CheckList.Clear();
            CheckList.AddRange(FilterList);
            filterItem.Clear(true);
        }

        public void ReloadCheckDetail()
        {
            isReloadCheckItem = true;
            foreach(var v in FilterList)
            {
                v.ReloadCheckObject();
            }
            CheckDetailSummary();
            isReloadCheckItem = false;
        }
    };
}