using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// 设置缩进
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IndentAttribute : PropertyAttribute
    {
        public IndentAttribute(int indentLevel = 1)
        {
#if UNITY_EDITOR
            _indentLevel = indentLevel;
#endif
        }


#if UNITY_EDITOR

        int _indentLevel;

        [CustomPropertyDrawer(typeof(IndentAttribute))]
        class IndentDrawer : BasePropertyDrawer<IndentAttribute>
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.indentLevel += attribute._indentLevel;
                base.OnGUI(position, property, label);
                EditorGUI.indentLevel -= attribute._indentLevel;
            }
        }

#endif // UNITY_EDITOR

    } // class IndentAttribute

} // namespace UnityExtensions