using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Resources;
using FastBundle;
using UnityEngine.SceneManagement;

namespace FastFrameWork
{
    public class ApplicationManager : MonoBehaviour
    {
        private static ApplicationManager instance;

        public static ApplicationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ApplicationManager>();
                }

                return ApplicationManager.instance;
            }
            set { ApplicationManager.instance = value; }
        }

        public AppMode m_AppMode = AppMode.Developing;

        private ApplicationStart startApp;

        public static AppMode AppMode
        {
            get
            {
#if APPMODE_DEV
                return AppMode.Developing;
#elif APPMODE_REL
            return AppMode.Release;
#else
            return instance.m_AppMode;
#endif
            }
        }

        /// <summary>
        /// 显示括号标识多语言转换的字段
        /// </summary>
        public bool showLanguageValue = false;

        public void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            AppLaunch();
        }

        /// <summary>
        /// 程序启动
        /// </summary>
        public void AppLaunch()
        {

                    if (AppMode != AppMode.Release)
                    {
                        GUIConsole.Init(); //运行时Console         
                        Log.Init(true); //开启 Debug
                    }
                    else
                    {
                        Log.Init(false); //关闭 Debug
                    }
        }

        #region 程序生命周期事件派发

        public static ApplicationVoidCallback s_OnApplicationQuit = null;
        public static ApplicationBoolCallback s_OnApplicationPause = null;
        public static ApplicationBoolCallback s_OnApplicationFocus = null;
        public static ApplicationVoidCallback s_OnApplicationUpdate = null;
        public static ApplicationVoidCallback s_OnApplicationFixedUpdate = null;
        public static ApplicationVoidCallback s_OnApplicationOnGUI = null;
        public static ApplicationVoidCallback s_OnApplicationOnDrawGizmos = null;
        public static ApplicationVoidCallback s_OnApplicationLateUpdate = null;

        void OnApplicationQuit()
        {
            if (s_OnApplicationQuit != null)
            {
                try
                {
                    s_OnApplicationQuit();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        /*
         * 强制暂停时，先 OnApplicationPause，后 OnApplicationFocus
         * 重新“启动”游戏时，先OnApplicationFocus，后 OnApplicationPause
         */
        void OnApplicationPause(bool pauseStatus)
        {
            if (s_OnApplicationPause != null)
            {
                try
                {
                    s_OnApplicationPause(pauseStatus);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (s_OnApplicationFocus != null)
            {
                try
                {
                    s_OnApplicationFocus(focusStatus);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        void Update()
        {
            if (s_OnApplicationUpdate != null)
                s_OnApplicationUpdate();
            if(startApp != null)
                startApp.OnUpdate();
        }

        private void LateUpdate()
        {
            if (s_OnApplicationLateUpdate != null)
            {
                s_OnApplicationLateUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (s_OnApplicationFixedUpdate != null)
                s_OnApplicationFixedUpdate();
        }

        void OnGUI()
        {
            if (s_OnApplicationOnGUI != null)
                s_OnApplicationOnGUI();
        }

        private void OnDrawGizmos()
        {
            if (s_OnApplicationOnDrawGizmos != null)
                s_OnApplicationOnDrawGizmos();
        }

        #endregion

        #region 程序启动细节
        

        #endregion
    }
}

public enum AppMode
{
    Developing,
    Release
}

public delegate void ApplicationBoolCallback(bool status);

public delegate void ApplicationVoidCallback();