using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Rendering/Light Intensity", "Light Intensity")]
    class TweenLightIntensity : TweenFloat
    {
        public Light targetLight;


        public override float current
        {
            get
            {
                if (targetLight)
                {
                    return targetLight.intensity;
                }
                return 1f;
            }
            set
            {
                if (targetLight)
                {
                    targetLight.intensity = value;
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


        [CustomEditor(typeof(TweenLightIntensity))]
        new class Editor : Editor<TweenLightIntensity>
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