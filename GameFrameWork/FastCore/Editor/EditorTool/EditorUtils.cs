using System;
using UnityEditor;
using UnityEngine;

    public sealed partial class EditorUtils
    {
        private const float Tolerance = 0.00001f;

        public static bool EventIsRepaint
        {
            get { return Event.current.type == EventType.Repaint; }
        }


        //================================================================================================
        //                                              Swaps
        //================================================================================================
        public static void SwapHandlesColor(Color color, Action action)
        {
            var oldColor = Handles.color;
            Handles.color = color;
            action();
            Handles.color = oldColor;
        }

        public static void SwapColor(Color color, Action action)
        {
            var oldColor = GUI.color;
            GUI.color = color;
            action();
            GUI.color = oldColor;
        }

        //This does not work!
        private static void SwapBackColor(Color color, Action action)
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            action();
            GUI.backgroundColor = oldColor;
        }


        public static void SwapDisabled(Action action, bool condition)
        {
            var oldEnabled = GUI.enabled;
            if (condition) GUI.enabled = false;
            action();
            GUI.enabled = oldEnabled;
        }


        //================================================================================================
        //                                              Layout
        //================================================================================================
        public static void Vertical(Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void Vertical(GUIStyle style, Action callback)
        {
            EditorGUILayout.BeginVertical(style);
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void Vertical(GUIStyle style, Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void Horizontal(Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void Horizontal(GUIStyle style, Action callback)
        {
            EditorGUILayout.BeginHorizontal(style);
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void Horizontal(GUIStyle style, Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void WithLabelWidth(int width, Action callback)
        {
            var oldValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
            callback();
            EditorGUIUtility.labelWidth = oldValue;
        }

        public static void WithFieldWidth(int width, Action callback)
        {
            var oldValue = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.fieldWidth = width;
            callback();
            EditorGUIUtility.fieldWidth = oldValue;
        }

        public static void WithWidth(int labelWidth, int fieldWidth, Action callback)
        {
            WithLabelWidth(labelWidth, () => WithFieldWidth(fieldWidth, callback));
        }

        public static void Box(Action action)
        {
            Vertical("Box", action);
        }

    }
