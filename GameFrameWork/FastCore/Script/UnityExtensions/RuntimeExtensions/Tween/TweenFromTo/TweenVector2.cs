using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    public abstract class TweenVector2 : TweenFromTo<Vector2>
    {
        public bool2 toggle;

        protected override void OnInterpolate(float factor)
        {
            if (toggle.anyTrue)
            {
                var t = toggle.allTrue ? default(Vector2) : current;

                if (toggle.x) t.x = (to.x - from.x) * factor + from.x;
                if (toggle.y) t.y = (to.y - from.y) * factor + from.y;

                current = t;
            }
        }

#if UNITY_EDITOR

        public override void Reset()
        {
            base.Reset();
            toggle = default(bool2);
        }


        protected new abstract class Editor<T> : TweenFromTo<Vector2>.Editor<T> where T : TweenVector2
        {
            SerializedProperty _fromXProp;
            SerializedProperty _fromYProp;

            SerializedProperty _toXProp;
            SerializedProperty _toYProp;

            SerializedProperty _toggleXProp;
            SerializedProperty _toggleYProp;


            protected override void OnEnable()
            {
                base.OnEnable();

                _fromXProp = _fromProp.FindPropertyRelative("x");
                _fromYProp = _fromProp.FindPropertyRelative("y");

                _toXProp = _toProp.FindPropertyRelative("x");
                _toYProp = _toProp.FindPropertyRelative("y");

                var _toggleProp = serializedObject.FindProperty("toggle");
                _toggleXProp = _toggleProp.FindPropertyRelative("x");
                _toggleYProp = _toggleProp.FindPropertyRelative("y");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                FromToFieldLayout("X", _fromXProp, _toXProp, _toggleXProp);
                FromToFieldLayout("Y", _fromYProp, _toYProp, _toggleYProp);
            }
        }


#endif

    } // class TweenVector2

} // namespace UnityExtensions