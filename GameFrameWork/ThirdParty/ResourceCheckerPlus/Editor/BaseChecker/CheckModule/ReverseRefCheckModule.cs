using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 资源在全工程反向引用查找
    /// </summary>
    public class ReverseRefCheckModule : ResCheckModuleBase
    {
        public override void SetCheckerConfig()
        {
            ShowRefCheckItem(true, false, false);
        }

        public override void CheckResource(Object[] resources)
        {
            Clear();
            Object[] selection = resources == null ? GetAllObjectInSelection() : resources;
            foreach (var v in activeCheckerList)
            {
                v.ReverseRefResCheck(selection);
            }
            Refresh();
        }
    }
}
