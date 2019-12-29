using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 配置信息类，ScriptableObject与Mono脚本一样，要求类名与文件名一致，否则重启Unity后会丢失脚本
    /// </summary>
    public class CheckerCommonConfig : ScriptableObject
    {
        public Color highlightTextColor = Color.yellow;
        public Color warningItemColor = Color.red;
        //public Color highlightBackgroundColor = Color.yellow;
        public int sideBarWidth = 250;
        public bool autoFilterOnSideBarButtonClick = false;                                  //在侧边栏点击时自动进行引用筛选
        public BatchOptionSelection batchOptionType = BatchOptionSelection.AllInFilterList;  //进行批量移动或修改格式的操作是针对全列表还是选中列表
        public CheckInputMode inputType = CheckInputMode.DragMode;                           //检查模式：拖入检查或选中检查
        public string checkResultExportPath = "";
        public int maxCheckRecordCount = 8;
    }
}
