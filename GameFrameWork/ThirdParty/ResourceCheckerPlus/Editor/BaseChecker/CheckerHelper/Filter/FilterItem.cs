using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace ResourceCheckerPlus
{
    /// <summary>
    /// 筛选器
    /// </summary>
    public class FilterItem
    {
        public enum FilterType
        {
            AndFilter,
            OrFilter,
        }

        public ObjectChecker checker = null;
        public string currentFilterStr = "";
        public int currentFliterType = 0;
        public bool positive = true;
        public string[] filterTypeArray = null;
        public CheckItem[] filterArray = null;
        public Object specialFilter = null;
        //一种简单的职责链进行实现
        public FilterItem nextFilterNode = null;
        public FilterItem parentFilterNode = null;
        //目前只支持And筛选或者Or筛选，混用暂时不支持
        public static FilterType filterType = FilterType.AndFilter;

        public FilterItem(ObjectChecker c)
        {
            checker = c;
            filterTypeArray = c.checkItem.Where(x => x.type != CheckType.Texture && x.show).Select(x => x.title).ToArray();
            filterArray = c.checkItem.Where(x => x.type != CheckType.Texture && x.show).ToArray();
        }

        public void ShowFilter()
        {
            CustomShowFilter();
            //职责链
            if (nextFilterNode != null)
            {
                nextFilterNode.ShowFilter();
            }
        }

        public List<ObjectDetail> CheckDetailFilter(List<ObjectDetail> inList)
        {
            if (filterType == FilterType.AndFilter)
                return AndDetailFiter(inList);
            else if (filterType == FilterType.OrFilter)
                return OrDetailFilter(inList);
            else
                return inList;
        }

        private List<ObjectDetail> AndDetailFiter(List<ObjectDetail> inList)
        {
            List<ObjectDetail> tempList = null;
            tempList = CustomDoFilter(inList);
            if (nextFilterNode != null)
            {
                return nextFilterNode.AndDetailFiter(tempList);
            }
            else
            {
                return tempList;
            }
        }

        private List<ObjectDetail> OrDetailFilter(List<ObjectDetail> inList)
        {
            List<ObjectDetail> tempList = null;
            tempList = CustomDoFilter(inList);
            if (nextFilterNode != null)
            {
                List<ObjectDetail> childList = nextFilterNode.OrDetailFilter(inList);
                foreach(var v in childList)
                {
                    if (!tempList.Contains(v))
                        tempList.Add(v);
                }
            }
            return tempList;
        }

        private bool CheckDetailFilterInternal(object value, CheckItem item, string filter, bool positive)
        {
            if (string.IsNullOrEmpty(filter))
                return true;
            switch (item.type)
            {
                case CheckType.String:
                    {
                        string str = value as string;
                        return positive ? str.ToLower().Contains(filter.ToLower()) : !str.ToLower().Contains(filter.ToLower());
                    }
                case CheckType.Int:
                case CheckType.FormatSize:
                    {
                        int num = 0;
                        int.TryParse(filter, out num);
                        return positive ? (int)value >= num : (int)value <= num;
                    }
                case CheckType.Float:
                    {
                        float num = 0;
                        float.TryParse(filter, out num);
                        return positive ? (float)value >= num : (float)value <= num;
                    }
                case CheckType.List:
                    {
                        int num = 0;
                        int.TryParse(filter, out num);
                        List<Object> list = value as List<Object>;
                        return positive ? list.Count >= num : list.Count <= num;
                    }
                case CheckType.Custom:
                    {
                        return item.customFilter(value);
                    }
                default:
                    return true;
            }
        }

        public void Clear(bool clearChildren)
        {
            currentFilterStr = "";
            currentFliterType = 0;
            positive = true;
            if (clearChildren)
            {
                nextFilterNode = null;
            }
        }

        public void AddFilterNode(FilterItem item)
        {
            if (nextFilterNode == null)
            {
                nextFilterNode = item;
                item.parentFilterNode = this;
            }
            else
            {
                nextFilterNode.AddFilterNode(item);
            }
        }

        public void RemoveFilterNode()
        {
            if (parentFilterNode != null)
                parentFilterNode.nextFilterNode = nextFilterNode;
            if (nextFilterNode != null)
                nextFilterNode.parentFilterNode = parentFilterNode;
        }

        public virtual void CustomShowFilter()
        {
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            currentFilterStr = GUILayout.TextField(currentFilterStr, new GUIStyle("SearchTextField"), GUILayout.Width(300));
            if (EditorGUI.EndChangeCheck())
            {
                checker.RefreshCheckResult();
            }

            EditorGUI.BeginChangeCheck();
            currentFliterType = EditorGUILayout.Popup(currentFliterType, filterTypeArray, GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
            {
                currentFilterStr = "";
                checker.RefreshCheckResult();
            }

            EditorGUI.BeginChangeCheck();
            positive = GUILayout.Toggle(positive, positive ? "正向" : "反向", GUILayout.Width(40));
            if (EditorGUI.EndChangeCheck())
            {
                checker.RefreshCheckResult();
            }

            if (parentFilterNode == null)
            {
                if (GUILayout.Button("增加筛选", GUILayout.Width(60)))
                {
                    AddFilterNode(new FilterItem(checker));
                }
                EditorGUI.BeginChangeCheck();
                filterType = (FilterType)EditorGUILayout.EnumPopup(filterType, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    checker.RefreshCheckResult();
                }
                checker.ShowOptionButton();
            }
            else
            {
                if (GUILayout.Button("删除筛选", GUILayout.Width(60)))
                {
                    RemoveFilterNode();
                    checker.RefreshCheckResult();
                }
            }
            GUILayout.EndHorizontal();
        }

        public virtual List<ObjectDetail> CustomDoFilter(List<ObjectDetail> inList)
        {
            CheckItem currentCheckItem = filterArray[currentFliterType];
            if (currentCheckItem == null || string.IsNullOrEmpty(currentFilterStr))
            {
                return filterType == FilterType.AndFilter || parentFilterNode == null ? inList.Where(x => true).ToList() : new List<ObjectDetail>(0);
            }
            else
            {
                return inList.Where(x => CheckDetailFilterInternal(x.checkMap[currentCheckItem], currentCheckItem, currentFilterStr, positive) == true).ToList();
            }
        }

        //不用反射获取原生风格的搜索框了，在多个检查器切换时，数据切换了，显示得不对，怀疑是static实现？
        //public string SearchField(string value, params GUILayoutOption[] options)
        //{
        //    MethodInfo info = typeof(EditorGUILayout).GetMethod("ToolbarSearchField", BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(GUILayoutOption[]) }, null);
        //    if (info != null)
        //    {
        //        value = (string)info.Invoke(null, new object[] { value, options });
        //    }
        //    return value;
        //}

        public void CreateFilterFromCfg(FilterItem item, PredefineFilterGroup filterGroup, int index)
        {
            FilterItemCfg cfg = null;
            if (index < filterGroup.filterCfgGroup.Length)
                cfg = filterGroup.filterCfgGroup[index];
            else
                return;
            FilterItem newItem = new FilterItem(checker);
            if (InitFilterByCfg(newItem, cfg))
                item.AddFilterNode(newItem);
            CreateFilterFromCfg(newItem, filterGroup, ++index);
        }

        private bool InitFilterByCfg(FilterItem item, FilterItemCfg cfg)
        {
            if (cfg == null)
                return false;
            CheckItem cItem = checker.GetCheckItemByName(cfg.checkItemName);
            if (cItem != null)
            {
                item.currentFliterType = ArrayUtility.IndexOf(item.filterArray, cItem);
                item.currentFilterStr = cfg.filterString;
                item.positive = cfg.positive;
                return true;
            }
            return false;
        }

        public void SaveFilterAsCfg(List<FilterItemCfg> cfgList)
        {
            FilterItemCfg cfg = new FilterItemCfg();
            cfg.checkItemName = filterArray[currentFliterType].title;
            cfg.filterString = currentFilterStr;
            cfg.positive = positive;
            cfgList.Add(cfg);
            if (nextFilterNode != null)
            {
                nextFilterNode.SaveFilterAsCfg(cfgList);
            }
        }

        public void CreateFromFilterGroup(PredefineFilterGroup filterGroup)
        {
            Clear(true);
            //首先设置自身，然后设置子节点
            if (filterGroup != null && filterGroup.filterCfgGroup != null && filterGroup.filterCfgGroup.Length > 0)
            {
                FilterItemCfg cfg = filterGroup.filterCfgGroup[0];
                InitFilterByCfg(this, cfg);
                CreateFilterFromCfg(this, filterGroup, 1);
            }
        }

        public PredefineFilterGroup SaveAsFilterGroup()
        {
            PredefineFilterGroup filterGroup = ScriptableObject.CreateInstance<PredefineFilterGroup>();
            List<FilterItemCfg> cfgList = new List<FilterItemCfg>();
            SaveFilterAsCfg(cfgList);
            filterGroup.filterCfgGroup = cfgList.ToArray();
            return filterGroup;
        }
    }
}
