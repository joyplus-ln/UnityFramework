using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class EditorStyleUtils
    {
        const string HelpBoxKey = "HelpBox";
        const string TextFiledKey = "TextField";

        public static GUIStyle GetHelpBoxStyle()
        {
            return GUI.skin.GetStyle(HelpBoxKey);
        }

        public static GUIStyle GetTextFieldStyle()
        {
            return GUI.skin.GetStyle(TextFiledKey);
        }
    }