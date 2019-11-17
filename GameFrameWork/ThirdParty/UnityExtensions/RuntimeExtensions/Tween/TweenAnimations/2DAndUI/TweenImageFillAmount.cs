using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("2D and UI/Image Fill Amount", "Image Fill Amount")]
    class TweenImageFillAmount : TweenFloat
    {
        public Image targetImage;


        public override float current
        {
            get
            {
                if (targetImage)
                {
                    return targetImage.fillAmount;
                }
                return 1f;
            }
            set
            {
                if (targetImage)
                {
                    targetImage.fillAmount = value;
                }
            }
        }


#if UNITY_EDITOR

        Image _originalTarget;


        public override void Record()
        {
            _originalTarget = targetImage;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetImage;
            targetImage = _originalTarget;
            base.Restore();
            targetImage = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetImage = GetComponent<Image>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenImageFillAmount))]
        new class Editor : Editor<TweenImageFillAmount>
        {
            SerializedProperty _targetImageProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetImageProp = serializedObject.FindProperty("targetImage");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetImageProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions