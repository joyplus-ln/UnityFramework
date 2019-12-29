using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class CheckerConfig : ScriptableObject
    {
        public CheckItemConfig[] checkItemCfg = new CheckItemConfig[] { };
    }

    [System.Serializable]
    public class CheckItemConfig
    {
        public string CheckerName;
        public string ItemClassName;
    }

}
