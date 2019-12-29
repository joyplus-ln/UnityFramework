
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 检查模块基类
    /// </summary>
    public class ResCheckModuleBase
    {
        public GUIContent checkModeName = null;
        //用于检查某个文件夹下所有的功能
        public Object objInputSlot = null;
        public List<ObjectChecker> checkerList = new List<ObjectChecker>();
        public List<ObjectChecker> activeCheckerList = null;
        public string currentModuleTip = "";
        public SelfObjectChecker sideBarObjList = new SelfObjectChecker();
        public int[] activeCheckerConfig = null;
        public Rect SideBarRect;
        public Rect MainRect;
        public CheckModuleConfig checkModuleCfg = null;
        public List<string> checkRecord = new List<string>();

        public int currentActiveChecker = 0;
        public string[] checkerListNames = null;

        public void CreateChecker()
        {
            foreach (var v in checkModuleCfg.checkerCfgs)
            {
                ObjectChecker.CreateChecker(this, v);
            }
        }

        public void ShowRefCheckItem(bool refObj, bool detailRef, bool activeInRef)
        {
            foreach( var v in checkerList)
            {
                v.refItem.show = refObj;
                v.totalRefItem.show = detailRef;
                v.activeItem.show = activeInRef;
            }
        }

        public ObjectChecker CurrentActiveChecker()
        {
            if (activeCheckerList != null && currentActiveChecker < activeCheckerList.Count)
            {
                return activeCheckerList[currentActiveChecker];
            }
            return null;
        }

        public void ShowCurrentCheckDetail()
        {
            var checker = CurrentActiveChecker();
            if (checker != null)
            {
                checker.ShowCheckerTitle();
            }
            else
            {
                GUILayout.Label("当前无可用检查类别，请从右侧下拉列表中选择需要检查的资源类型");
            }
            ShowCheckerSelecter();
            if (checker != null)
            {
                checker.ShowCheckResult();
                if (Event.current.button == 1 && MainRect.Contains(Event.current.mousePosition))
                {
                    checker.OnContexMenu();
                }
            }
        }

        public void ShowCheckerSelecter()
        {
            if (checkerListNames == null || activeCheckerList == null)
                return;
            GUILayout.BeginHorizontal();
            currentActiveChecker = GUILayout.Toolbar(currentActiveChecker, checkerListNames, GUILayout.Width(MainRect.width - 80));
            if (GUILayout.Button("检查类型", GUILayout.Width(80)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var c in checkerList.Where(x => !x.isSpecialChecker))
                {
                    //这个地方不能用delegate，Unity5.5版本没有问题，但在Unity5.3版本测试时下拉菜单传入的checker一直是for循环的最后一个
                    menu.AddItem(new GUIContent(c.checkerName), c.checkerEnabled, new GenericMenu.MenuFunction2(OnCheckerItemSelected), c);
                }
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("全部开启"), false, new GenericMenu.MenuFunction2(SetAllCheckerEnable), true);
                menu.AddItem(new GUIContent("全部关闭"), false, new GenericMenu.MenuFunction2(SetAllCheckerEnable), false);
                menu.AddSeparator(string.Empty);
                //special的不受全部开关控制
                foreach (var c in checkerList.Where(x => x.isSpecialChecker))
                {
                    menu.AddItem(new GUIContent(c.checkerName), c.checkerEnabled, new GenericMenu.MenuFunction2(OnCheckerItemSelected), c);
                }
                menu.ShowAsContext();
            }

            GUILayout.EndHorizontal();
        }

        private void OnCheckerItemSelected(object obj)
        {
            ObjectChecker checker = obj as ObjectChecker;
            if (checker == null)
                return;
            checker.checkerEnabled = !checker.checkerEnabled;
            RefreshCheckerConfig(checker);
        }

        private void SetAllCheckerEnable(object enable)
        {
            bool enabled = (bool)enable;
            foreach(var v in checkerList.Where(x => !enabled || !x.isSpecialChecker))
            {
                v.checkerEnabled = enabled;
                RefreshCheckerConfig(v);
            }
        }

        public virtual void ShowCurrentSideBar()
        {
            ShowCommonSideBarContent();
            if (GUILayout.Button("导出全部激活列表内容"))
            {
                ExportAllActiveCheckerResult();
            }
            sideBarObjList.ShowCheckResult();
        }

        public virtual void ShowCommonSideBarContent()
        {
            CheckerCommonConfig cfg = CheckerConfigManager.checkerConfig;
            if (cfg.inputType == CheckInputMode.DragMode)
            {
                ShowObjectDragSlot();
            }
            if (GUILayout.Button("检查资源"))
            {
                CheckResource(null);
            }
        }

        public void InitCheckModule(CheckModuleConfig cfg)
        {
            checkerList.Clear();
            sideBarObjList.Clear();
            checkModuleCfg = cfg;
            if (cfg == null)
                return;
            CreateChecker();
            SetCheckerConfig();
            LoadCheckRecord();
            RefreshCheckerConfig();
        }

        public virtual void SetCheckerConfig() { }

        public void RefreshCheckerConfig(ObjectChecker checker = null)
        {
            if (checkerList != null)
            {
                activeCheckerList = checkerList.Where(x => x.checkerEnabled).ToList();
                checkerListNames = activeCheckerList.Select(x => x.checkerName).ToArray();
                if (checker != null && checker.checkerEnabled)
                {
                    currentActiveChecker = activeCheckerList.IndexOf(checker);
                    checker.Clear();
                }
                else
                {
                    currentActiveChecker = 0;
                }
            }
        }

        public void Clear()
        {
            //全清
            checkerList.ForEach(x => x.Clear());
            ClearSideBarList();
        }

        public void Refresh()
        {
            checkerList.ForEach(x => x.RefreshCheckResult());
        }

        public virtual void CheckResource(Object[] resources) { }

        public Object[] GetAllObjectInSelection()
        {
            if (CheckerConfigManager.checkerConfig.inputType == CheckInputMode.SelectMode)
                return Selection.objects;
            else
                return new Object[] { objInputSlot };
        }

        protected void ShowObjectDragSlot()
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            objInputSlot = EditorGUILayout.ObjectField(objInputSlot, typeof(UnityEngine.Object), true);
            if (EditorGUI.EndChangeCheck())
            {
                RecordCheckPath(objInputSlot);
            }
            ShowCheckRecord();
            GUILayout.EndHorizontal();
        }

        protected void ShowCheckRecord()
        {
            if (checkRecord.Count == 0)
                return;
            if (GUILayout.Button("常用路径"))
            {
                GenericMenu genericMenu = new GenericMenu();
                for(int i = checkRecord.Count - 1; i >= 0; i--)
                {
                    var v = checkRecord[i];
                    if (v == null)
                        continue;
                    string path = v.Replace('/', '.');
                    genericMenu.AddItem(new GUIContent(path), false, new GenericMenu.MenuFunction2(this.OnCheckRecordSelect), v);
                }
                genericMenu.ShowAsContext();
            }
        }

        private void OnCheckRecordSelect(object o)
        {
            string path = o as string;
            objInputSlot = AssetDatabase.LoadAssetAtPath<Object>(path);
            RecordCheckPath(objInputSlot);
        }

        private void RecordCheckPath(Object obj)
        {
            if (obj == null)
                return;
            string path = AssetDatabase.GetAssetPath(obj);
            if (checkRecord.Contains(path))
            {
                checkRecord.Remove(path);
            }
            checkRecord.Add(path);
            while (checkRecord.Count > CheckerConfigManager.checkerConfig.maxCheckRecordCount)
            {
                checkRecord.RemoveAt(0);
            }
            checkModuleCfg.checkRecord = checkRecord.ToArray();
        }

        private void LoadCheckRecord()
        {
            checkRecord.Clear();
            checkRecord.AddRange(checkModuleCfg.checkRecord);
        }

        public void AddObjectToSideBarList(List<Object> objects)
        {
            sideBarObjList.AddObjectToSelfObjectChecker(objects);
        }

        public void ClearSideBarList()
        {
            sideBarObjList.ClearSelfObjectList();
        }

        public void OnSideBarMouseMenu()
        {
            sideBarObjList.OnContexMenu();
        }

        public void ExportAllActiveCheckerResult()
        {
            string path = ResourceCheckerHelper.GenericExportFolderName();
            Directory.CreateDirectory(path);
            activeCheckerList.ForEach(x => x.ExportCheckResult(path));
            AssetDatabase.Refresh();
        }

        public void LoadPredefineCheckFilterCfg()
        {
            activeCheckerList.ForEach(x => x.LoadCheckFilter());
        }

        public static void CreateCheckModule(ResourceCheckerPlus root, CheckModuleConfig cfg)
        {
            System.Type type = System.Type.GetType("ResourceCheckerPlus." + cfg.CheckModuleClassName);
            if (type == null)
                return;
            ResCheckModuleBase checkModule = System.Activator.CreateInstance(type) as ResCheckModuleBase;
            checkModule.InitCheckModule(cfg);
            checkModule.checkModeName = new GUIContent(cfg.CheckModuleTitleName, cfg.CheckModuleDescription);
            root.resCheckModeList.Add(checkModule);
        }
    }
}
