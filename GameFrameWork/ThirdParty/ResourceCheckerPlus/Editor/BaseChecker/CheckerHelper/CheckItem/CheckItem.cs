namespace ResourceCheckerPlus
{
    public enum CheckType
    {
        String,
        Int,
        Float,
        Custom,
        FormatSize,
        Texture,
        List,
        None,
    }

    /// <summary>
    /// 检查控件类，用于存储，显示，有待重构
    /// </summary>
    public delegate bool CustomFilter(object o);
    public delegate void CustomClickOption(ObjectDetail detail);
    public class CheckItem
    {
        public string title;                    //显示名称
        public int width;                       //显示宽度
        public CheckType type;                  //检查类型
        public CustomFilter customFilter;       //自定义筛选函数
        public CustomClickOption clickOption;   //点击操作
        public bool show = true;                //是否显示
        public int order = 0;                   //显示顺序
        public bool sortSymbol = true;          //排序顺序

        public CheckItem(ObjectChecker checker, string t, int w = 80, CheckType ty = CheckType.String, CustomClickOption option = null, CustomFilter f = null)
        {
            title = t;
            width = w;
            type = ty;
            customFilter = f;
            clickOption = option;
            order = checker.checkItem.Count;
            checker.checkItem.Add(this);
        }

        public static CheckItem CreateCheckItemFromConfig(ObjectChecker checker, CheckItemConfig cfg)
        {
            //先带着命名空间
            System.Type type = System.Type.GetType("ResourceCheckerPlus." + cfg.ItemClassName);
            //如果为null，再不带命名空间试一下，防止有哥们忘了加命名空间
            if (type == null)
                type = System.Type.GetType(cfg.ItemClassName);
            if (type == null)
                return null;
            CheckItem item = System.Activator.CreateInstance(type, checker, cfg.CheckerName) as CheckItem;
            return item;
        }

        public virtual object GetCheckValue(UnityEngine.Object obj)
        {
            return "null";
        }
    };
}