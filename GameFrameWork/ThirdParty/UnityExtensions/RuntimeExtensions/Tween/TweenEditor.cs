#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions
{
    public partial class Tween
    {
        static List<Tween> _tweenList = new List<Tween>();
        static List<TweenAnimation> _animationList = new List<TweenAnimation>();
        static List<GameObject> _objectList = new List<GameObject>();


        [SerializeField] bool _foldoutController = true;
        [SerializeField] bool _foldoutEvents = false;


        bool _preview;
        bool _dragging;

        bool _enabledRecord;
        float _normalizedTimeRecord;
        PlayDirection _directionRecord;


        void RecordAll()
        {
            _enabledRecord = enabled;
            _normalizedTimeRecord = _normalizedTime;
            _directionRecord = direction;

            if (_animations != null)
                foreach (var anim in _animations)
                    anim?.Record();
        }


        void RestoreAll()
        {
            enabled = _enabledRecord;
            _normalizedTime = _normalizedTimeRecord;
            direction = _directionRecord;

            if (_animations != null)
                foreach (var anim in _animations)
                    anim?.Restore();
        }


        bool playing
        {
            get
            {
                if (Application.isPlaying) return enabled;
                else return _preview;
            }
            set
            {
                if (Application.isPlaying) enabled = value;
                else
                {
                    if (_preview != value)
                    {
                        _preview = value;

                        if (value)
                        {
                            EditorApplication.update += OnUpdate;
                            EditorApplication.quitting += StopPreview;
                            EditorApplication.playModeStateChanged += StopPreview2;

                            RecordAll();
                        }
                        else
                        {
                            EditorApplication.update -= OnUpdate;
                            EditorApplication.quitting -= StopPreview;
                            EditorApplication.playModeStateChanged -= StopPreview2;

                            RestoreAll();
                        }
                    }
                }

                void StopPreview() { playing = false; }
                void StopPreview2(PlayModeStateChange msg)
                {
                    if (msg == PlayModeStateChange.ExitingEditMode) playing = false;
                }
            }
        }


        bool dragging
        {
            get { return _dragging; }
            set
            {
                if (_dragging != value)
                {
                    _dragging = value;

                    if (value)
                    {
                        if (!playing)
                        {
                            RecordAll();
                        }
                    }
                    else
                    {
                        if (!playing)
                        {
                            RestoreAll();
                        }
                    }
                }
            }
        }


        static bool AnyContainsInTweenList(TweenAnimation target)
        {
            foreach (var tween in _tweenList)
            {
                if (tween._animations != null && tween._animations.Contains(target))
                {
                    return true;
                }
            }
            return false;
        }


        // 删除 GameObject 上没有被引用的 Animation 组件
        static void RemoveAllUnusedAnimations(GameObject target)
        {
            target.GetComponents(_tweenList);
            target.GetComponents(_animationList);

            foreach (var anim in _animationList)
            {
                if (!AnyContainsInTweenList(anim))
                {
                    Undo.DestroyObjectImmediate(anim);
                }
            }

            _animationList.Clear();
            _tweenList.Clear();
        }


        [InitializeOnLoadMethod]
        static void Init()
        {
            // 编辑器下每帧检查没有被引用的 Animation 组件
            // 因为有些编辑器操作无法监测（Copy/Paste)，OnValidate 又不能删除组件，只能通过这种办法来了 :-/

            EditorApplication.update += () =>
            {
                foreach (var obj in _objectList)
                {
                    if (obj) RemoveAllUnusedAnimations(obj);
                }
                _objectList.Clear();
            };
        }


        private void Reset()
        {
            RemoveAllUnusedAnimations(gameObject);
        }


        protected override void OnValidate()
        {
            base.OnValidate();

            if (!GeneralKit.IsNullOrEmpty(_animations))
            {
                // 删除自身引用的错误的 Animation 组件
                GetComponents(_tweenList);
                _tweenList.Remove(this);
                for (int i = 0; i < _animations.Count; i++)
                {
                    if (_animations[i] && _animations[i].gameObject == gameObject && !AnyContainsInTweenList(_animations[i]))
                    {
                        continue;
                    }
                    _animations.RemoveAt(i--);
                }
                _tweenList.Clear();
            }

            _objectList.Add(gameObject);
        }


        public void UndoRemoveAnimation(TweenAnimation animation)
        {
            Undo.RecordObject(this, "RemoveAnimation");
            _animations.Remove(animation);
            Undo.DestroyObjectImmediate(animation);
        }


        [CustomEditor(typeof(Tween))]
        [CanEditMultipleObjects]
        internal class Editor : BaseEditor<Tween>
        {
            static GUIStyle _imageButtonStyle;
            static GUIStyle imageButtonStyle
            {
                get
                {
                    if (_imageButtonStyle == null)
                    {
                        _imageButtonStyle = new GUIStyle(EditorStyles.miniButton);
                        _imageButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                    }
                    return _imageButtonStyle;
                }
            }


            internal static Color progressBackgroundInvalid
            {
                get { return EditorGUIUtility.isProSkin ? new Color() : new Color(0,0,0,0.6f); }
            }

            internal static Color progressBackgroundValid
            {
                get { return EditorGUIUtility.isProSkin ? new Color() : new Color(0,1,0.2f,0.5f); }
            }

            internal static Color progressForegroundInvalid
            {
                get { return EditorGUIUtility.isProSkin ? new Color() : new Color(0.8f,0.8f,1,0.6f); }
            }

            internal static Color progressForegroundValid
            {
                get { return EditorGUIUtility.isProSkin ? new Color() : new Color(0.2f,1f,0,1); }
            }


            SerializedProperty _durationProp;
            SerializedProperty _updateModeProp;
            SerializedProperty _timeModeProp;
            SerializedProperty _wrapModeProp;
            SerializedProperty _arrivedActionProp;
            SerializedProperty _onForwardArrivedProp;
            SerializedProperty _onBackArrivedProp;

            List<UnityEditor.Editor> _editors = new List<UnityEditor.Editor>();
            GenericMenu _addMenu;


            void ShowAddMenu(Rect rect)
            {
                if (_addMenu == null)
                {
                    _addMenu = EditorGUIKit.CreateMenu(
                        TweenAnimation.allTypes.Keys,
                        t => new GUIContent(TweenAnimation.allTypes[t].menu),
                        t => MenuItemState.Normal,
                        t =>
                        {
                            var cmpt = Undo.AddComponent(target.gameObject, t) as TweenAnimation;
                            Undo.RecordObject(cmpt, "AddComponent");
                            cmpt.Reset();

                            Undo.RecordObject(target, "AddComponent");
                            target.AddAnimationInternal(cmpt);
                        });
                }

                _addMenu.DropDown(rect);
            }


            void OnEnable()
            {
                _durationProp = serializedObject.FindProperty("_duration");
                _updateModeProp = serializedObject.FindProperty("_updateMode");
                _timeModeProp = serializedObject.FindProperty("timeMode");
                _wrapModeProp = serializedObject.FindProperty("wrapMode");
                _arrivedActionProp = serializedObject.FindProperty("arrivedAction");

                _onForwardArrivedProp = serializedObject.FindProperty("_onForwardArrived");
                _onBackArrivedProp = serializedObject.FindProperty("_onBackArrived");
            }


            void OnDisable()
            {
                if (!Application.isPlaying)
                    if (target) target.playing = false;
            }


            void OnDestroy()
            {
                foreach (var editor in _editors)
                {
                    GeneralKit.DestroySafely(editor);
                }
                _editors.Clear();
            }


            public override bool RequiresConstantRepaint()
            {
                if (Application.isPlaying) return target.isActiveAndEnabled;
                else return target._preview;
            }


            public override void OnInspectorGUI()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                var rect = EditorGUILayout.GetControlRect();
                var rect2 = rect;

                // controller foldout
                using (var scope = new ChangeCheckScope(target))
                {
                    rect2.width = rect2.height;
                    var result = GUI.Toggle(rect2, target._foldoutController, GUIContent.none, EditorStyles.foldout);
                    if (scope.changed) target._foldoutController = result;
                }

                // controller label
                rect.xMin = rect2.xMax;
                EditorGUI.LabelField(rect, "Controller", EditorStyles.boldLabel);

                serializedObject.Update();

                // controller settings
                if (target._foldoutController)
                {
                    EditorGUILayout.PropertyField(_durationProp);
                    EditorGUILayout.PropertyField(_updateModeProp);
                    EditorGUILayout.PropertyField(_timeModeProp);
                    EditorGUILayout.PropertyField(_wrapModeProp);
                    EditorGUILayout.PropertyField(_arrivedActionProp);
                    GUILayout.Space(4);
                }

                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), EditorStyles.centeredGreyMiniLabel.normal.textColor);
                GUILayout.Space(4);

                rect = EditorGUILayout.GetControlRect();
                rect2 = rect;

                // play button
                rect2.width = EditorGUIUtility.singleLineHeight * 2 - 4;
                using (new GUIContentColorScope(target.playing ? progressForegroundValid : EditorGUIKit.defaultTextColor))
                {
                    target.playing = GUI.Toggle(rect2, target.playing,
                        EditorGUIKit.TempContent(image: EditorAsset.instance.play), imageButtonStyle);
                }

                // direction button
                rect2.x = rect.xMax - rect2.width;
                using (new DisabledScope(!target.playing))
                {
                    using (new GUIContentColorScope(EditorGUIKit.defaultTextColor))
                    {
                        if (GUI.Button(rect2, EditorGUIKit.TempContent(image: target.direction == PlayDirection.Forward ?
                            EditorAsset.instance.rightArrow : EditorAsset.instance.leftArrow), imageButtonStyle))
                        {
                            target.ReverseDirection();
                        }
                    }
                }

                rect.xMin += EditorGUIUtility.singleLineHeight * 2;
                rect.xMax -= EditorGUIUtility.singleLineHeight * 2;

                // 鼠标开始拖动
                if (Event.current.type == EventType.MouseDown)
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        target.dragging = true;
                    }
                }

                // 鼠标结束拖动
                if (Event.current.rawType == EventType.MouseUp)
                {
                    if (target.dragging)
                    {
                        target.dragging = false;
                        Repaint();
                    }
                }

                // progress bar
                using (var scope = new ChangeCheckScope(null))
                {
                    float progress = EditorGUIKit.ProgressBar(rect, target._normalizedTime, progressBackgroundInvalid, progressForegroundValid);
                    if (scope.changed && target.dragging)
                    {
                        target.normalizedTime = progress;
                    }
                }

                GUILayout.Space(4);

                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                rect = EditorGUILayout.GetControlRect();
                rect2 = rect;

                // events foldout
                using (var scope = new ChangeCheckScope(target))
                {
                    rect2.width = rect2.height;
                    var result = GUI.Toggle(rect2, target._foldoutEvents, GUIContent.none, EditorStyles.foldout);
                    if (scope.changed) target._foldoutEvents = result;
                }

                // events label
                rect.xMin = rect2.xMax;
                EditorGUI.LabelField(rect, "Events", EditorStyles.boldLabel);

                // events
                if (target._foldoutEvents)
                {
                    EditorGUILayout.PropertyField(_onForwardArrivedProp);
                    GUILayout.Space(2);
                    EditorGUILayout.PropertyField(_onBackArrivedProp);
                    GUILayout.Space(2);
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // 绘制 Animation 列表。为了正确处理各种意外情况（比如其他代码增删了列表），因此处理比较复杂 :-/
                if (!GeneralKit.IsNullOrEmpty(target._animations))
                {
                    int editorIndex = 0;
                    for (int i = 0; i < target._animations.Count; i++)
                    {
                        var anim = target._animations[i];
                        if (anim)
                        {
                            if (_editors.Count <= editorIndex) _editors.Add(null);

                            if (!_editors[editorIndex] || _editors[editorIndex].target != anim)
                            {
                                GeneralKit.DestroySafely(_editors[editorIndex]);
                                _editors[editorIndex] = CreateEditor(anim);
                            }

                            (_editors[editorIndex] as TweenAnimation.Editor).OnInspectorGUI(target);
                            editorIndex++;
                        }
                        else
                        {
                            target._animations.RemoveAt(i--);
                        }
                    }
                    for (; editorIndex < _editors.Count; editorIndex++)
                    {
                        GeneralKit.DestroySafely(_editors[editorIndex]);
                        _editors.RemoveAt(editorIndex--);
                    }
                }

                // add button
                GUILayout.Space(4);
                var buttonRect = EditorGUILayout.GetControlRect();
                if (GUI.Button(buttonRect, "Add Animation...", EditorStyles.miniButton))
                {
                    ShowAddMenu(buttonRect);
                }
                GUILayout.Space(4);

                EditorGUILayout.EndVertical();
            }

        } // class TweenEditor

    } // class Tween

} // UnityExtensions

#endif // UNITY_EDITOR
