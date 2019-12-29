using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("2D and UI/Sprite Color", "Sprite Color")]
    class TweenSpriteColor : TweenColor
    {
        public SpriteRenderer targetRenderer;


        public override Color current
        {
            get
            {
                if (targetRenderer)
                {
                    return targetRenderer.color;
                }
                return new Color(1, 1, 1);
            }
            set
            {
                if (targetRenderer)
                {
                    targetRenderer.color = value;
                }
            }
        }


#if UNITY_EDITOR

        SpriteRenderer _originalTarget;


        public override void Record()
        {
            _originalTarget = targetRenderer;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetRenderer;
            targetRenderer = _originalTarget;
            base.Restore();
            targetRenderer = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetRenderer = GetComponent<SpriteRenderer>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenSpriteColor))]
        new class Editor : Editor<TweenSpriteColor>
        {
            SerializedProperty _targetRendererProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetRendererProp = serializedObject.FindProperty("targetRenderer");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetRendererProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions