using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Rendering/Light Color", "Light Color")]
    class TweenLightColor : TweenColor
    {
        public Light targetLight;


        public override Color current
        {
            get
            {
                if (targetLight)
                {
                    return targetLight.color;
                }
                return new Color(1, 1, 1);
            }
            set
            {
                if (targetLight)
                {
                    targetLight.color = value;
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


        [CustomEditor(typeof(TweenLightColor))]
        new class Editor : Editor<TweenLightColor>
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