using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// 在字段之前设置缩进
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IndentBeforeAttribute : PropertyAttribute
    {
        public IndentBeforeAttribute(int indentLevel = 1)
        {
#if UNITY_EDITOR
            this.indentLevel = indentLevel;
#endif
        }

#if UNITY_EDITOR
        public readonly int indentLevel;
#endif
    }


    /// <summary>
    /// 在字段之后设置缩进
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IndentAfterAttribute : PropertyAttribute
    {
        public IndentAfterAttribute(int indentLevel = -1)
        {
#if UNITY_EDITOR
            this.indentLevel = indentLevel;
#endif
        }

#if UNITY_EDITOR
        public readonly int indentLevel;
#endif
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(IndentBeforeAttribute))]
    [CustomPropertyDrawer(typeof(IndentAfterAttribute))]
    class BeginIndentDrawer : BasePropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is IndentBeforeAttribute)
            {
                EditorGUI.indentLevel += (attribute as IndentBeforeAttribute).indentLevel;
            }

            base.OnGUI(position, property, label);

            if (attribute is IndentAfterAttribute)
            {
                EditorGUI.indentLevel += (attribute as IndentAfterAttribute).indentLevel;
            }
        }
    }

#endif // UNITY_EDITOR

} // namespace UnityExtensions