#if UNITY_EDITOR

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions
{
    public abstract partial class TweenAnimation
    {
        public static readonly Dictionary<Type, TweenAnimationAttribute> allTypes = new Dictionary<Type, TweenAnimationAttribute>();

        [SerializeField] bool _foldout = true;


        [InitializeOnLoadMethod]
        static void Init()
        {
            var types = ReflectionKit.allAssemblyTypes.Where(
                t => t.IsSubclassOf(typeof(TweenAnimation))
                && t.IsDefined(typeof(TweenAnimationAttribute), false)
                && !t.IsAbstract);

            foreach (var t in types)
            {
                allTypes.Add(t, t.GetCustomAttributes(typeof(TweenAnimationAttribute), false)[0] as TweenAnimationAttribute);
            }
        }


        public abstract void Record();
        public abstract void Restore();


        public virtual void Reset()
        {
            enabled = true;
            _minNormalizedTime = 0f;
            _maxNormalizedTime = 1f;
            _interpolator = new CustomizableInterpolator();
            _foldout = true;
        }


        protected static void FromToFieldLayout(string label, ref float from, ref float to, out bool fromChanged, out bool toChanged)
        {
            var rect = EditorGUILayout.GetControlRect();
            float labelWidth = EditorGUIUtility.labelWidth;

            var fromRect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
            var toRect = new Rect(rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
            rect.width = labelWidth - 8;

            var content = EditorGUIKit.TempContent(label);
            EditorGUI.LabelField(rect, content);

            rect.width = EditorStyles.label.CalcSize(content).x;
            float delta = EditorGUIKit.DragValue(rect, 0, 0.01f);

            using (new LabelWidthScope(14))
            {
                float newFrom = EditorGUI.FloatField(fromRect, "F", from + delta);
                float newTo = EditorGUI.FloatField(toRect, "T", to + delta);

                fromChanged = from != newFrom;
                from = newFrom;

                toChanged = to != newTo;
                to = newTo;
            }
        }


        protected static void FromToFieldLayout(string label, ref float from, ref float to)
        {
            FromToFieldLayout(label, ref from, ref to, out _, out _);
        }


        protected static void FromToFieldLayout(string label, SerializedProperty from, SerializedProperty to)
        {
            float fromValue = from.floatValue;
            float toValue = to.floatValue;

            FromToFieldLayout(label, ref fromValue, ref toValue, out bool fromChanged, out bool toChanged);

            if (fromChanged) from.floatValue = fromValue;
            if (toChanged) to.floatValue = toValue;
        }


        protected static void FromToFieldLayout(string label, ref float from, ref float to, ref bool toggle, out bool fromChanged, out bool toChanged, out bool toggleChanged)
        {
            var rect = EditorGUILayout.GetControlRect();
            float labelWidth = EditorGUIUtility.labelWidth;

            var fromRect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
            var toRect = new Rect(rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
            var toggleRect = new Rect(rect.x, rect.y, rect.height, rect.height);
            rect.width = labelWidth - 8 - toggleRect.width;
            rect.x = toggleRect.xMax;

            bool newToggle = EditorGUI.Toggle(toggleRect, toggle);
            using (new DisabledScope(!newToggle))
            {
                var content = EditorGUIKit.TempContent(label);
                EditorGUI.LabelField(rect, content);

                rect.width = EditorStyles.label.CalcSize(content).x;
                float delta = EditorGUIKit.DragValue(rect, 0, 0.01f);

                using (new LabelWidthScope(14))
                {
                    float newFrom = EditorGUI.FloatField(fromRect, "F", from + delta);
                    float newTo = EditorGUI.FloatField(toRect, "T", to + delta);

                    fromChanged = from != newFrom;
                    from = newFrom;

                    toChanged = to != newTo;
                    to = newTo;

                    toggleChanged = toggle != newToggle;
                    toggle = newToggle;
                }
            }
        }


        protected static void FromToFieldLayout(string label, ref float from, ref float to, ref bool toggle)
        {
            FromToFieldLayout(label, ref from, ref to, ref toggle, out _, out _, out _);
        }


        protected static void FromToFieldLayout(string label, SerializedProperty from, SerializedProperty to, SerializedProperty toggle)
        {
            float fromValue = from.floatValue;
            float toValue = to.floatValue;
            bool toggleValue = toggle.boolValue;

            FromToFieldLayout(label, ref fromValue, ref toValue, ref toggleValue, out bool fromChanged, out bool toChanged, out bool toggleChanged);

            if (fromChanged) from.floatValue = fromValue;
            if (toChanged) to.floatValue = toValue;
            if (toggleChanged) toggle.boolValue = toggleValue;
        }


        public abstract class Editor : UnityEditor.Editor
        {
            public abstract void OnInspectorGUI(Tween tween);
        }


        protected abstract class Editor<T> : Editor where T : TweenAnimation
        {
            protected new T target => base.target as T;

            SerializedProperty _interpolatorProp;

            GenericMenu _optionsMenu;


            protected virtual void OnEnable()
            {
                _interpolatorProp = serializedObject.FindProperty("_interpolator");
            }


            protected virtual void InitOptionsMenu(GenericMenu menu, Tween tween)
            {
                menu.AddItem(new GUIContent("Reset"), false, () => { Undo.RecordObject(target, "Reset"); target.Reset(); });
                menu.AddItem(new GUIContent("Remove"), false, () => tween.UndoRemoveAnimation(target));
            }


            public sealed override void OnInspectorGUI(Tween tween)
            {
                var rect = EditorGUILayout.GetControlRect();
                var rect2 = rect;

                // foldout
                using (var scope = new ChangeCheckScope(target))
                {
                    rect2.width = rect.height;
                    var result = GUI.Toggle(rect2, target._foldout, GUIContent.none, EditorStyles.foldout);
                    if (scope.changed) target._foldout = result;
                }

                // enabled
                using (var scope = new ChangeCheckScope(target))
                {
                    rect2.x = rect2.xMax;
                    var result = EditorGUI.ToggleLeft(rect2, GUIContent.none, target.enabled);
                    if (scope.changed) target.enabled = result;
                }

                var optionsIcon = EditorGUIKit.paneOptionsIcon;

                // name
                rect2.x = rect2.xMax;
                rect2.xMax = rect.xMax - optionsIcon.width;
                EditorGUI.LabelField(rect2, allTypes[typeof(T)].name, EditorStyles.boldLabel);

                // options
                rect.Set(rect2.xMax, rect.y + 4, optionsIcon.width, optionsIcon.height);
                if (GUI.Button(rect, EditorGUIKit.TempContent(image: optionsIcon), GUIStyle.none))
                {
                    if (_optionsMenu == null)
                    {
                        _optionsMenu = new GenericMenu();
                        InitOptionsMenu(_optionsMenu, tween);
                    }
                    _optionsMenu.DropDown(rect);
                }

                rect = EditorGUILayout.GetControlRect(false, 3);
                rect.xMin += EditorGUIUtility.singleLineHeight * 2;
                rect.xMax -= EditorGUIUtility.singleLineHeight * 2;

                // progress
                EditorGUI.DrawRect(rect, Tween.Editor.progressBackgroundInvalid);

                rect2.Set(rect.x + target.minNormalizedTime * rect.width, rect.y,
                    Mathf.Max(1, rect.width * (target.maxNormalizedTime - target.minNormalizedTime)), rect.height);

                if (target.enabled)
                {
                    rect.width = Mathf.Round(rect.width * tween.normalizedTime);
                    EditorGUI.DrawRect(rect, Tween.Editor.progressForegroundInvalid);
                }

                EditorGUI.DrawRect(rect2, Tween.Editor.progressBackgroundValid);

                if (target.enabled)
                {
                    rect2 = rect.GetIntersection(rect2);
                    if (rect2.width > 0) EditorGUI.DrawRect(rect2, Tween.Editor.progressForegroundValid);
                }

                GUILayout.Space(4);

                if (target._foldout)
                {
                    using (var scope = new ChangeCheckScope(target))
                    {
                        float min = target.minNormalizedTime * tween.duration;
                        float max = target.maxNormalizedTime * tween.duration;

                        FromToFieldLayout("Time Range", ref min, ref max, out bool fromChanged, out bool toChanged);

                        if (scope.changed)
                        {
                            if (fromChanged) target.minNormalizedTime = Mathf.Min(min / tween.duration, target.maxNormalizedTime);
                            if (toChanged) target.maxNormalizedTime = Mathf.Max(max / tween.duration, target.minNormalizedTime);
                        }
                    }

                    serializedObject.Update();
                    EditorGUILayout.PropertyField(_interpolatorProp);
                    OnPropertiesGUI(tween);
                    GUILayout.Space(4);
                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), EditorStyles.centeredGreyMiniLabel.normal.textColor);
            }


            protected abstract void OnPropertiesGUI(Tween tween);
        }

    } // TweenAnimation

} // namespace UnityExtensions

#endif