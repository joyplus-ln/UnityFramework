using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    public abstract class TweenColor : TweenFromTo<Color>
    {
        [ToggleButton("Working Mode", "Gradient", "From-To")]
        public bool useGradient;
        public Gradient gradient;
        public bool toggleRGB;
        public bool toggleAlpha;


        protected override void OnInterpolate(float factor)
        {
            if (toggleRGB || toggleAlpha)
            {
                var t = (toggleRGB && toggleAlpha) ? default(Color) : current;

                if (useGradient)
                {
                    var c = gradient.Evaluate(factor);
                    if (toggleRGB)
                    {
                        t.r = c.r;
                        t.g = c.g;
                        t.b = c.b;
                    }
                    if (toggleAlpha) t.a = c.a;
                }
                else
                {
                    if (toggleRGB)
                    {
                        t.r = (to.r - from.r) * factor + from.r;
                        t.g = (to.g - from.g) * factor + from.g;
                        t.b = (to.b - from.b) * factor + from.b;
                    }
                    if (toggleAlpha) t.a = (to.a - from.a) * factor + from.a;
                }

                current = t;
            }
        }


#if UNITY_EDITOR

        public override void Reset()
        {
            base.Reset();
            useGradient = false;
            gradient = null;
            toggleRGB = false;
            toggleAlpha = false;
        }


        protected new abstract class Editor<T> : TweenFromTo<Color>.Editor<T> where T : TweenColor
        {
            SerializedProperty _useGradientProp;
            SerializedProperty _gradientProp;

            SerializedProperty _fromAProp;
            SerializedProperty _toAProp;

            SerializedProperty _toggleRGBProp;
            SerializedProperty _toggleAlphaProp;


            protected virtual bool hdr
            {
                get { return false; }
            }


            protected override void OnEnable()
            {
                base.OnEnable();

                _useGradientProp = serializedObject.FindProperty("useGradient");
                _gradientProp = serializedObject.FindProperty("gradient");

                _fromAProp = _fromProp.FindPropertyRelative("a");
                _toAProp = _toProp.FindPropertyRelative("a");

                _toggleRGBProp = serializedObject.FindProperty("toggleRGB");
                _toggleAlphaProp = serializedObject.FindProperty("toggleAlpha");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_useGradientProp);

                if (target.useGradient)
                {
                    var rect = EditorGUILayout.GetControlRect();
                    var rect2 = rect;

                    rect.width = rect.height + EditorStyles.label.CalcSize(EditorGUIKit.TempContent("RGB")).x;
                    _toggleRGBProp.boolValue = EditorGUI.ToggleLeft(rect, "RGB", _toggleRGBProp.boolValue);

                    rect.x = rect.xMax + rect.height;
                    rect.width = rect.height + EditorStyles.label.CalcSize(EditorGUIKit.TempContent("A")).x;
                    _toggleAlphaProp.boolValue = EditorGUI.ToggleLeft(rect, "A", _toggleAlphaProp.boolValue);

                    using (new DisabledScope(!target.toggleRGB && !target.toggleAlpha))
                    {
                        rect2.xMin += EditorGUIUtility.labelWidth;
                        EditorGUI.PropertyField(rect2, _gradientProp, GUIContent.none);
                    }
                }
                else
                {
                    var rect = EditorGUILayout.GetControlRect();
                    float labelWidth = EditorGUIUtility.labelWidth;

                    var fromRect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
                    var toRect = new Rect(rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
                    rect.width = labelWidth - 8;

                    _toggleRGBProp.boolValue = EditorGUI.ToggleLeft(rect, "RGB", _toggleRGBProp.boolValue);

                    using (new DisabledScope(!target.toggleRGB))
                    {
                        using (new LabelWidthScope(14))
                        {
                            _fromProp.colorValue = EditorGUI.ColorField(fromRect, EditorGUIKit.TempContent("F"), _fromProp.colorValue, false, false, hdr);
                            _toProp.colorValue = EditorGUI.ColorField(toRect, EditorGUIKit.TempContent("T"), _toProp.colorValue, false, false, hdr);
                        }
                    }

                    FromToFieldLayout("A", _fromAProp, _toAProp, _toggleAlphaProp);
                }
            }
        }

#endif

    } // class TweenColor

} // namespace UnityExtensions