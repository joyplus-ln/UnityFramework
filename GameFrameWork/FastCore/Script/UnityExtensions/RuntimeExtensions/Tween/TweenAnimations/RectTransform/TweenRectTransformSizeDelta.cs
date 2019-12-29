using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Rect Transform/Size Delta", "Rect Transform Size Delta")]
    class TweenRectTransformSizeDelta : TweenVector2
    {
        public RectTransform targetRectTransform;


        public override Vector2 current
        {
            get
            {
                if (targetRectTransform)
                {
                    return targetRectTransform.sizeDelta;
                }
                return default(Vector2);
            }
            set
            {
                if (targetRectTransform)
                {
                    targetRectTransform.sizeDelta = value;
                }
            }
        }


#if UNITY_EDITOR

        RectTransform _originalTarget;


        public override void Record()
        {
            _originalTarget = targetRectTransform;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetRectTransform;
            targetRectTransform = _originalTarget;
            base.Restore();
            targetRectTransform = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetRectTransform = GetComponent<RectTransform>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenRectTransformSizeDelta))]
        new class Editor : Editor<TweenRectTransformSizeDelta>
        {
            SerializedProperty _targetRectTransformProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetRectTransformProp = serializedObject.FindProperty("targetRectTransform");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetRectTransformProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions