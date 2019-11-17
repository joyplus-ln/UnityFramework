using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Transform/Scale", "Transform Scale")]
    class TweenTransformScale : TweenVector3
    {
        public Transform targetTransform;


        public override Vector3 current
        {
            get
            {
                if (targetTransform)
                {
                    return targetTransform.localScale;
                }
                return new Vector3(1, 1, 1);
            }
            set
            {
                if (targetTransform)
                {
                    targetTransform.localScale = value;
                }
            }
        }


#if UNITY_EDITOR

        Transform _originalTarget;


        public override void Record()
        {
            _originalTarget = targetTransform;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetTransform;
            targetTransform = _originalTarget;
            base.Restore();
            targetTransform = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetTransform = transform;
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenTransformScale))]
        new class Editor : Editor<TweenTransformScale>
        {
            SerializedProperty _targetTransformProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetTransformProp = serializedObject.FindProperty("targetTransform");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetTransformProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions