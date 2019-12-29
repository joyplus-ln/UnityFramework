#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Other/Move Along Path", "Move Along Path")]
    class TweenMoveAlongPath : TweenFloat
    {
        public MoveAlongPath targetMoveAlongPath;


        public override float current
        {
            get
            {
                if (targetMoveAlongPath)
                {
                    return targetMoveAlongPath.distance;
                }
                return 0;
            }
            set
            {
                if (targetMoveAlongPath)
                {
                    targetMoveAlongPath.distance = value;
                }
            }
        }


#if UNITY_EDITOR

        MoveAlongPath _originalTarget;


        public override void Record()
        {
            _originalTarget = targetMoveAlongPath;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetMoveAlongPath;
            targetMoveAlongPath = _originalTarget;
            base.Restore();
            targetMoveAlongPath = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetMoveAlongPath = GetComponent<MoveAlongPath>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenMoveAlongPath))]
        new class Editor : Editor<TweenMoveAlongPath>
        {
            SerializedProperty _targetMoveAlongPathProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetMoveAlongPathProp = serializedObject.FindProperty("targetMoveAlongPath");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetMoveAlongPathProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions