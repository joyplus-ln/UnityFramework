#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    public abstract class TweenFloat : TweenFromTo<float>
    {
        protected override void OnInterpolate(float factor)
        {
            current = (to - from) * factor + from;
        }

#if UNITY_EDITOR

        protected new abstract class Editor<T> : TweenFromTo<float>.Editor<T> where T : TweenFloat
        {
            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                FromToFieldLayout("Value", _fromProp, _toProp);
            }
        }

#endif

    } // class TweenFloat

} // namespace UnityExtensions