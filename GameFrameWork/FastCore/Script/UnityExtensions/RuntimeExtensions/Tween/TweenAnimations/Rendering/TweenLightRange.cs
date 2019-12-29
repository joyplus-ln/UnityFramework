using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Rendering/Light Range", "Light Range")]
    class TweenLightRange : TweenFloat
    {
        public Light targetLight;


        public override float current
        {
            get
            {
                if (targetLight)
                {
                    return targetLight.range;
                }
                return 10f;
            }
            set
            {
                if (targetLight)
                {
                    targetLight.range = value;
                }
            }
        }


#if UNITY_EDITOR

        Light _originalTarget;


        public override void Record()
        {
            _originalTarget = targetLight;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetLight;
            targetLight = _originalTarget;
            base.Restore();
            targetLight = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetLight = GetComponent<Light>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenLightRange))]
        new class Editor : Editor<TweenLightRange>
        {
            SerializedProperty _targetLightProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetLightProp = serializedObject.FindProperty("targetLight");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetLightProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions