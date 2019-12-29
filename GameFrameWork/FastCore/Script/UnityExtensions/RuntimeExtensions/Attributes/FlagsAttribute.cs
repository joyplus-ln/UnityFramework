using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个枚举字段上, 将其作为选择掩码使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class FlagsAttribute : PropertyAttribute
    {
        bool _includeObsolete;

        public FlagsAttribute(bool includeObsolete = false)
        {
            _includeObsolete = includeObsolete;
        }
 
#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(FlagsAttribute))]
        class FlagsDrawer : BasePropertyDrawer<FlagsAttribute>
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var value = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

                using (var scope = new ChangeCheckScope(null))
                {
                    value = EditorGUI.EnumFlagsField(position, label, value, attribute._includeObsolete);
                    if (scope.changed)
                    {
                        property.intValue = value.GetHashCode();
                    }
                }
            }
        }

#endif // UNITY_EDITOR

    } // class FlagsAttribute

} // namespace UnityExtensions