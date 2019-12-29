using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("2D and UI/Graphic Color", "Graphic Color")]
    class TweenGraphicColor : TweenColor
    {
        public Graphic targetGraphic;


        public override Color current
        {
            get
            {
                if (targetGraphic)
                {
                    return targetGraphic.color;
                }
                return new Color(1, 1, 1);
            }
            set
            {
                if (targetGraphic)
                {
                    targetGraphic.color = value;
                }
            }
        }


#if UNITY_EDITOR

        Graphic _originalTarget;


        public override void Record()
        {
            _originalTarget = targetGraphic;
            base.Record();
        }


        public override void Restore()
        {
            var t = targetGraphic;
            targetGraphic = _originalTarget;
            base.Restore();
            targetGraphic = t;
        }


        public override void Reset()
        {
            base.Reset();
            targetGraphic = GetComponent<Graphic>();
            from = current;
            to = current;
        }


        [CustomEditor(typeof(TweenGraphicColor))]
        new class Editor : Editor<TweenGraphicColor>
        {
            SerializedProperty _targetGraphicProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _targetGraphicProp = serializedObject.FindProperty("targetGraphic");
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetGraphicProp);

                base.OnPropertiesGUI(tween);
            }
        }

#endif
    }

} // namespace UnityExtensions