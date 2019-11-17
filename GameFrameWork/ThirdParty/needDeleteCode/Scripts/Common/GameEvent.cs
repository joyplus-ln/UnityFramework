public enum GameEvent
{
    //重启游戏
    AppRestart,

    //请求用户数据
    RequestSync,

    FbLoginSync,
    NormalSync,
    UploadUserData,

    //内购打点事件
    AnalysisIapEvent,

    //购买金币和道具事件
    CurrencyIapEvent,

    CustomIapEvent,

    FacebookIapEvent,
    AppsflyerIapEvent,
    AdjustIapEvent,
    FabricIapEvent,
    FirebaseIapEvent,
    FlurryIapEvent,
    FTDIapEvent,

    //礼包
    ReqestGift,

    //内购校验
    ValidateReceipt,

    //请求广告配置
    ReqestAds,

    //礼包是否弹出事件
    SalePopup,

    //加载关卡
    LoadLevel,

    //加载bonus词
    LoadBonus,

    //飞金币
    RubyFly,

    //请求背景图
    ReqBg,

    //隔天重置的
    ResetPerDay,

    //
    LocalNotification,

    //预加载资源
    PreloadAsset,

    //开始AB test
    ABTesting,

    //动态关卡
    ReqestDdl,

    // 每日签到
    DailySignGift,
    // 娃娃机
    PrizeClaw,

    //设置声效和音乐
    SettingSound,

//    // 活动配置
//    ActivityConfig,

    // 锦标赛上传数据
    WeekRankUpload,

    // 锦标赛拉取排行榜
    WeekRankGetList,
    ReqChests,
    //注册接口
    Login,
    //获取其他玩家统计数据接口
    PlayerProfile,
    //普通活动
    Activity_Common,
    //普通活动的排行榜
    Activity_Rank,
    //普通活动的排行榜
    Event_List,
    //获取卡包奖励内容
    CardBagGift,
    //获取成就内容
    ReqAchievement,
    //获得pve数据
    ReqPveMatch,
    //获得pverank数据
    ReqPveRankMatch
}