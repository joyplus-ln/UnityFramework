using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 直接资源检查功能
    /// </summary>
    public class DirectResCheckModule : ResCheckModuleBase
    {
        public override void SetCheckerConfig()
        {
            ShowRefCheckItem(false, false, false);
        }

        public override void CheckResource(Object[] resources)
        {
            Clear();
            Object[] selection = resources == null ? GetAllObjectInSelection() : resources;
            activeCheckerList.ForEach(x => x.DirectResCheck(selection));
            Refresh();
        }
    }
}