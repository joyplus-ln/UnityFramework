using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Transform/Rotation", "Transform Rotation")]
    class TweenTransformRotation : TweenQuaternion
    {
        public Transform targetTransform;
        public Space space = Space.Self;


        public override Quaternion current
        {
            get
            {
                if (targetTransform)
                {
                    return space == Space.Self ? targetTransform.localRotation : targetTransform.rotation;
                }
                return Quaternion.identity;
            }
            set
            {
                if (targetTransform)
                {
                    if (space == Space.Self) targetTransform.localRotation = value;
                    else targetTransform.rotation = value;
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
            space = Space.Self;
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenTransformRotation))]
        new class Editor : Editor<TweenTransformRotation>
        {
            SerializedProperty _targetTransformProp;
            SerializedProperty _spaceProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetTransformProp = serializedObject.FindProperty("targetTransform");
                _spaceProp = serializedObject.FindProperty("space");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetTransformProp);
                EditorGUILayout.PropertyField(_spaceProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions