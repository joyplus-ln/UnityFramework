using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    public abstract class TweenVector3 : TweenFromTo<Vector3>
    {
        public bool3 toggle;

        protected override void OnInterpolate(float factor)
        {
            if (toggle.anyTrue)
            {
                var t = toggle.allTrue ? default(Vector3) : current;

                if (toggle.x) t.x = (to.x - from.x) * factor + from.x;
                if (toggle.y) t.y = (to.y - from.y) * factor + from.y;
                if (toggle.z) t.z = (to.z - from.z) * factor + from.z;

                current = t;
            }
        }


#if UNITY_EDITOR

        public override void Reset()
        {
            base.Reset();
            toggle = default(bool3);
        }


        protected new abstract class Editor<T> : TweenFromTo<Vector3>.Editor<T> where T : TweenVector3
        {
            SerializedProperty _fromXProp;
            SerializedProperty _fromYProp;
            SerializedProperty _fromZProp;

            SerializedProperty _toXProp;
            SerializedProperty _toYProp;
            SerializedProperty _toZProp;

            SerializedProperty _toggleXProp;
            SerializedProperty _toggleYProp;
            SerializedProperty _toggleZProp;


            protected override void OnEnable()
            {
                base.OnEnable();

                _fromXProp = _fromProp.FindPropertyRelative("x");
                _fromYProp = _fromProp.FindPropertyRelative("y");
                _fromZProp = _fromProp.FindPropertyRelative("z");

                _toXProp = _toProp.FindPropertyRelative("x");
                _toYProp = _toProp.FindPropertyRelative("y");
                _toZProp = _toProp.FindPropertyRelative("z");

                var _toggleProp = serializedObject.FindProperty("toggle");
                _toggleXProp = _toggleProp.FindPropertyRelative("x");
                _toggleYProp = _toggleProp.FindPropertyRelative("y");
                _toggleZProp = _toggleProp.FindPropertyRelative("z");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                FromToFieldLayout("X", _fromXProp, _toXProp, _toggleXProp);
                FromToFieldLayout("Y", _fromYProp, _toYProp, _toggleYProp);
                FromToFieldLayout("Z", _fromZProp, _toZProp, _toggleZProp);
            }
        }

#endif

    } // class TweenVector3

} // namespace UnityExtensions