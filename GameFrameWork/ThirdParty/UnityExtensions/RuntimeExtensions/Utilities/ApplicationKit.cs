using System;
using System.Collections;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// Unity Application 工具箱
    /// </summary>
    public struct ApplicationKit
    {
        static GameObject _globalGameObject;
        static GlobalComponent _globalComponent;


        [RuntimeInitializeOnLoadMethod]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        static void Initialize()
        {
            if (!_globalGameObject)
            {
                _globalGameObject = new GameObject("GlobalGameObject");
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    GameObject.DontDestroyOnLoad(_globalGameObject);

                _globalGameObject.hideFlags =
                    HideFlags.HideInHierarchy
                    | HideFlags.HideInInspector
                    | HideFlags.DontSaveInEditor
                    | HideFlags.DontSaveInBuild
                    | HideFlags.DontUnloadUnusedAsset;
            }

            if (!_globalComponent)
            {
                _globalComponent = _globalGameObject.AddComponent<GlobalComponent>();
            }
        }


        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action fixedUpdate;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action waitForFixedUpdate;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action update;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action lateUpdate;


        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static void AddUpdate(UpdateMode mode, Action action)
        {
            switch (mode)
            {
                case UpdateMode.FixedUpdate: fixedUpdate += action; return;
                case UpdateMode.WaitForFixedUpdate: waitForFixedUpdate += action; return;
                case UpdateMode.Update: update += action; return;
                case UpdateMode.LateUpdate: lateUpdate += action; return;
            }
        }


        public static void RemoveUpdate(UpdateMode mode, Action action)
        {
            switch (mode)
            {
                case UpdateMode.FixedUpdate: fixedUpdate -= action; return;
                case UpdateMode.WaitForFixedUpdate: waitForFixedUpdate -= action; return;
                case UpdateMode.Update: update -= action; return;
                case UpdateMode.LateUpdate: lateUpdate -= action; return;
            }
        }


        /// <summary>
        /// 添加全局组件
        /// </summary>
        public static T AddGlobalComponent<T>() where T : Component
        {
            return _globalGameObject.AddComponent<T>();
        }


        [ExecuteInEditMode]
        public class GlobalComponent : ScriptableComponent
        {
            WaitForFixedUpdate _waitForFixedUpdate;


            void Start()
            {
                _waitForFixedUpdate = new WaitForFixedUpdate();
                StartCoroutine(WaitForFixedUpdate());
            }


            void FixedUpdate()
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    fixedUpdate?.Invoke();
            }


            IEnumerator WaitForFixedUpdate()
            {
                while (true)
                {
                    yield return _waitForFixedUpdate;
#if UNITY_EDITOR
                    if (Application.isPlaying)
#endif
                        waitForFixedUpdate?.Invoke();
                }
            }


            void Update()
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    update?.Invoke();
            }


            void LateUpdate()
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    lateUpdate?.Invoke();
            }

        } // class GlobalComponent

    } // struct ApplicationKit

} // namespace UnityExtensions