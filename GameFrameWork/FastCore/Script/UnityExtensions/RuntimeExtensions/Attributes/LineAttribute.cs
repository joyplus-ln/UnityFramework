using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// 在字段之上绘制一条横线
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class LineAttribute : PropertyAttribute
    {
        public LineAttribute(float spaceBefore = 6, float spaceAfter = 8, float height = 1)
        {
#if UNITY_EDITOR
            _spaceBefore = spaceBefore;
            _spaceAfter = spaceAfter;
            _height = height;
#endif
        }


#if UNITY_EDITOR

        float _spaceBefore;
        float _spaceAfter;
        float _height;

        [CustomPropertyDrawer(typeof(LineAttribute))]
        class LineDrawer : BaseDecoratorDrawer<LineAttribute>
        {
            public override bool CanCacheInspectorGUI()
            {
                return false;
            }

            public override float GetHeight()
            {
                return attribute._spaceBefore + attribute._spaceAfter + attribute._height;
            }

            public override void OnGUI(Rect position)
            {
                position.y += attribute._spaceBefore;
                position.height = attribute._height;
                var color = EditorGUIKit.defaultTextColor;
                color.a = 0.4f;
                EditorGUI.DrawRect(position, color);
            }
        }

#endif // UNITY_EDITOR

    } // class LineAttribute

} // namespace UnityExtensions