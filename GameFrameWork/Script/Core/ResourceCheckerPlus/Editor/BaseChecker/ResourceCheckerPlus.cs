using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace ResourceCheckerPlus
{
    public enum CheckInputMode
    {
        DragMode,
        SelectMode,
    }

    public enum BatchOptionSelection
    {
        AllInFilterList,
        CurrentSelect,
    }

    public class ResourceCheckerPlus : EditorWindow
    {
        private int currentActiveCheckModule = 0;
        public GUIContent[] checkModeListNames = null;
        public List<ResCheckModuleBase> resCheckModeList = new List<ResCheckModuleBase>();
        public CheckerConfigManager configManager = new CheckerConfigManager();
        public static ResourceCheckerPlus instance = null;

        [MenuItem("Window/Resource Checker Plus &R")]
        public static void Init()
        {
            //getwindow(xx, true) 可以使之有边框，并且不会被挡住，不过不能拖到unity主窗体里
            ResourceCheckerPlus window = (ResourceCheckerPlus)EditorWindow.GetWindow(typeof(ResourceCheckerPlus));
            window.minSize = new Vector2(800, 600);
        }

        void OnEnable()
        {
            instance = this;
            configManager.InitConfig();
            InitCheckerModule(); 
        }

        void OnDestroy()
        {
            configManager.SaveCheckerConfig();
            instance = null;
            Resources.UnloadUnusedAssets();
        }

        void OnGUI()
        {
            Rect rect = this.position;
            int sideBarWidth = CheckerConfigManager.checkerConfig.sideBarWidth;
            int spriteBarWidth = CheckerConfigManager.spriteBarWidth;
            Rect rectLeft = new Rect(0, 0, sideBarWidth, rect.height);
            Rect rectMid = new Rect(sideBarWidth, 0, spriteBarWidth, rect.height);
            Rect rectRight = new Rect(sideBarWidth + spriteBarWidth, 0, rect.width - sideBarWidth - spriteBarWidth, rect.height);

            SetRect(rectLeft, rectRight);

            //侧边栏
            GUILayout.BeginArea(rectLeft);
            ShowSideBar();
            GUILayout.EndArea();

            //分割线
            GUILayout.BeginArea(rectMid);
            GUI.backgroundColor = Color.black;
            GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(spriteBarWidth));
            GUI.backgroundColor = CheckerConfigManager.defaultBackgroundColor;
            GUILayout.EndArea();

            //主界面
            GUILayout.BeginArea(rectRight);
            ShowCheckDetail();
            GUILayout.EndArea();
        }

        public ResCheckModuleBase CurrentCheckModule()
        {
            if (currentActiveCheckModule < resCheckModeList.Count)
                return resCheckModeList[currentActiveCheckModule];
            return null;
        }

        public void SetRect(Rect sideBar, Rect mainWindow)
        {
            ResCheckModuleBase module = CurrentCheckModule();
            if (module != null)
            {
                module.SideBarRect = sideBar;
                module.MainRect = mainWindow;
            }
        }

        private void ShowSideBar()
        {
            if (checkModeListNames == null)
                return;
            currentActiveCheckModule = GUILayout.Toolbar(currentActiveCheckModule, checkModeListNames);
            GUILayout.BeginVertical();
            ResCheckModuleBase module = CurrentCheckModule();
            if (module != null)
            {
                module.ShowCurrentSideBar();
            }
            GUILayout.EndVertical();
        }

        private void ShowCheckDetail()
        {
            if (checkModeListNames == null)
                return;
            ResCheckModuleBase module = CurrentCheckModule();
            if (module != null)
            {
                module.ShowCurrentCheckDetail();
            }
        }

        public void InitCheckerModule()
        {
            //初始化检查模式
            resCheckModeList.Clear();
            foreach(var v in CheckerConfigManager.checkModuleConfigList)
            {
                ResCheckModuleBase.CreateCheckModule(this, v);
            }
            resCheckModeList.Sort(delegate (ResCheckModuleBase module1, ResCheckModuleBase module2) { return module1.checkModuleCfg.checkModuleOrder - module2.checkModuleCfg.checkModuleOrder; });
            checkModeListNames = resCheckModeList.Select(x => x.checkModeName).ToArray();
        }

        public void ClearCheckModule()
        {
            resCheckModeList.ForEach(x => x.Clear());
        } 

        public void SetCurrentActiveCheckModule(string checkModuleName)
        {
            var curModule = resCheckModeList.First(x => x.checkModeName.text == checkModuleName);
            currentActiveCheckModule = resCheckModeList.IndexOf(curModule);
        }
    }
}

