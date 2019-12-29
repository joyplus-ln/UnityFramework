using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    public abstract class TweenQuaternion : TweenFromTo<Quaternion>
    {
        protected override void OnInterpolate(float factor)
        {
            current = Quaternion.SlerpUnclamped(from, to, factor);
        }

#if UNITY_EDITOR

        Quaternion _fromQuaternion = Quaternion.identity;
        Vector3 _fromEulerAngles = Vector3.zero;
        Quaternion _toQuaternion = Quaternion.identity;
        Vector3 _toEulerAngles = Vector3.zero;


        protected new abstract class Editor<T> : TweenFromTo<Quaternion>.Editor<T> where T : TweenQuaternion
        {
            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                if (target._fromQuaternion != _fromProp.quaternionValue)
                {
                    _fromProp.quaternionValue = target._fromQuaternion = _fromProp.quaternionValue.normalized;
                    target._fromEulerAngles = target._fromQuaternion.eulerAngles;
                }

                if (target._toQuaternion != _toProp.quaternionValue)
                {
                    _toProp.quaternionValue = target._toQuaternion = _toProp.quaternionValue.normalized;
                    target._toEulerAngles = target._toQuaternion.eulerAngles;
                }

                bool3 fromChanged, toChanged;

                FromToFieldLayout("X", ref target._fromEulerAngles.x, ref target._toEulerAngles.x, out fromChanged.x, out toChanged.x);
                FromToFieldLayout("Y", ref target._fromEulerAngles.y, ref target._toEulerAngles.y, out fromChanged.y, out toChanged.y);
                FromToFieldLayout("Z", ref target._fromEulerAngles.z, ref target._toEulerAngles.z, out fromChanged.z, out toChanged.z);

                if (fromChanged.anyTrue)
                {
                    _fromProp.quaternionValue = target._fromQuaternion = Quaternion.Euler(target._fromEulerAngles).normalized;
                }

                if (toChanged.anyTrue)
                {
                    _toProp.quaternionValue = target._toQuaternion = Quaternion.Euler(target._toEulerAngles).normalized;
                }
            }
        }

#endif

    } // class TweenQuaternion

} // namespace UnityExtensions