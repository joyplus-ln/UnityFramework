#if UNITY_POST_PROCESSING_STACK_V2

using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Rendering/Post-process Weight", "Post Process Weight")]
    class TweenPostProcessWeight : TweenFloat
    {
        public PostProcessVolume targetVolume;


        public override float current
        {
            get
            {
                if (targetVolume)
                {
                    return targetVolume.weight;
                }
                return 1f;
            }
            set
            {
                if (targetVolume)
                {
                    targetVolume.weight = value;
                }
            }
        }


#if UNITY_EDITOR

        PostProcessVolume _originalTarget;


        public override void Record()
        {
            _originalTarget = targetVolume;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetVolume;
            targetVolume = _originalTarget;
            base.Restore();
            targetVolume = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetVolume = GetComponent<PostProcessVolume>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenPostProcessWeight))]
        new class Editor : Editor<TweenPostProcessWeight>
        {
            SerializedProperty _targetVolumeProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetVolumeProp = serializedObject.FindProperty("targetVolume");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetVolumeProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions

#endif // UNITY_POST_PROCESSING_STACK_V2