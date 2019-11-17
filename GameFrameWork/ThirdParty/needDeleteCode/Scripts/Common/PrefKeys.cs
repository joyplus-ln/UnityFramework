using System.Reflection.Emit;

public class PrefKeys
{
    public const string Currency = "coins";
    public const string NewCurrency = "new_curreny";
    public const string TempCoin = "TempCoin";

    /// <summary>
    /// 内购相关
    /// </summary>

    public const string PromotionPopupTimesPerDay = "P_PopupTimesPerDay";

    public const string PurchasedItem = "PurchasedItem";

    public const string ShowNoAdsSale = "ShowNoAdsSale";

    public const string ShopPendingList = "ShopBuyPendingListItem";
    public const string ShopPendingReceiptList = "ShopPendingReceiptList";

#if UNITY_ANDROID
    public const string BuyItem = "buy_item";
#else
    public const string BuyItem = "shoped_player";//为了兼容之前老用户
#endif

    /// <summary>
    /// 关卡相关存档
    /// </summary>
    public const string Current_World = "current_world";

    public const string Current_Sub_World = "current_sub_world";
    public const string Current_Level = "current_level";
    public const string Unlocked_World = "unlocked_world";
    public const string Unlocked_Sub_World = "unlocked_sub_world";
    public const string Unlocked_Level = "unlocked_level";
    public const string Unlocked_Maplevel = "unlocked_maplevel";
    public const string Level_Progress = "level_progress";
    public const string Daily_Level_Progress = "DailyChallenge_progress";
    public const string Daily_Level_Index = "Daily_level_index";
    public const string Extra_Progress = "extra_progress";
    public const string Extra_prefix = "Extra_prefix";//前缀 实际key为前缀+ gamemode

    //public const string Extra_Target = "extra_target";//曾经被使用，不能再用
    //public const string Total_Extra_Target = "total_extra_added";//曾经被使用，不能再用

    public const string AllPlayerScore = "All_PlayerScore";
    public const string PlayerScore = "Playerscore";
    public const string PlayerScoreDaily = "Playerscore_daily";
    

    /// <summary>
    /// 用户信息存档
    /// </summary>
    public const string Player_Near_Login_Time = "Info_playernearloginPayTime";

    public const string Player_CompleteLevel_PerDay = "player_complete_level_per_day";
    public const string Player_Login_Days = "player_login_days";
    public const string Player_MaxLoginConsecutiveDays = "player_max_login_consecutive_days";
    public const string Player_LoginConsecutiveDays = "player_login_consecutive_days";
    public const string Player_Install_Day = "FirstInstallDay";
    public const string Player_Open_Shop_Times = "info_playerOpenShopTimes";
    public const string PlayerPreserve = "PlayerPreserve";//留存
    public const string PlayerRateGame = "rate_game";
    public const string Player_PlayerTag = "Player_PlayerTag";//用户标签
    public const string Unlock_ExtraWord = "Unlock_ExtraWord";
    public const string PlayerPopFacebookPanelTimes = "PlayerPopFacebookPanelTimes";
    public const string PlayerTheLastPayTime = "PlayerTheLastPayTime";//用户上一次付费时间
    public const string PlayerDistanceInTwoPay = "playerDistanceInTwoPay";//两次付费间隔
    public const string PlayerOpenGameTimes = "PlayerOpenGameTimes";//玩家打开游戏的次数
    public const string PlayerFirstTag = "PlayerFirstTag";
    public const string PassLevelDayAverage = "passlevel_day_average";
    public const string GiveUpdateReward = "give_update_reward";
    public const string GiveUpdateReward_Version = "give_update_reward_version";
    public const string LackCoinsClose = "LackCoinsClose";
    public const string signCountKey = "signCountKey";
    public const string IsUserSyncDataDone = "IsUserSyncDataDone";
    public const string UseOnLinePData = "UseOnLinePData";

    /// <summary>
    /// 广告相关
    /// </summary>
    public const string RewardVideo_TotalShowTimes = "reward_video_total_showtimes";

    public const string RewardVideo_ShowTimes = "shop_video_showtimes";
    public const string RewardVideo_ShowFailTimes = "reward_video_show_fail_count";
    public const string Remove_Ads = "remove_ads";
    public const string WrongWord_RewardVideoDlg_CloseTimes = "WrongWord_RewardVideoDlg_CloseTimes";
    public const string WrongWordVideoTVTipShow = "WrongWordVideoTVTipShow";

    /// <summary>
    /// 每日奖励相关
    /// </summary>

    public const string ResetRewardVideoTime = "ResetRewardVideoTime";
    public const string ChannelGroup = "channel_organic";
    public const string ChannelConversion = "channel_conversion";
    public const string AppsflyerAllData = "AppsflyerAllData";
    public const string FaceBookImageURL = "FaceBookImageURL";
    public const string FaceBookImageCache = "FaceBookImageCache";

    /// <summary>
    /// 新手引导相关
    /// </summary>
    public const string NoviceGuidance_FirstLevel = "noviceguidance_firstlevel";

    public const string NoviceGuidance_ShuffleAndHint = "noviceguidance_shuffleandhint";
    public const string NoviceGuidance_Level2 = "NoviceGuidance_Level2";
    public const string NoviceGuidance_LevelThree = "NoviceGuidance_LevelThree";
    public const string NoviceGuidance_LevelFour = "NoviceGuidance_LevelFour";
    public const string NoviceGuidance_Level13 = "NoviceGuidance_Level13";
    public const string NoviceGuidance_Level13Win = "NoviceGuidance_Level13Win";
    public const string NoviceGuidance_LevelMissionOne = "NoviceGuidance_LevelMissionOne";
    public const string NoviceGuidance_LevelMissionOTwo = "NoviceGuidance_LevelMissionOTwo";
    public const string NoviceGuidance_LevelMissionThree = "NoviceGuidance_LevelMissionThree";
    public const string NoviceGuidance_LevelRate = "NoviceGuidance_LevelRate";
    public const string NoviceGuidance_RemoveAds = "noviceguidance_removeads";
    public const string NoviceGuidance_ChallengeWord = "noviceguidance_challengeword";
    public const string NoviceGuidance_Bonus = "noviceguidance_bonus";
    public const string NoviceGuidance_NewSubWorld = "noviceguidance_newsubworld";
    public const string NoviceGuidance_BonusDisableLength = "NoviceGuidance_BonusDisableLength";
    public const string NoviceGuidance_SpecificHint = "noviceguidance_specifichint";

    public const string NoviceGuidance_MultiHint = "NoviceGuidance_MultiHint";

    public const string NoviceGuidance_ReduceMoves = "NoviceGuidance_ReduceMoves";

    //firebase fb 相关
    public const string FaceBookLogined = "PlayerFaceBookLogined";
    public const string FbLoginGiftClaimed = "GetFacebookLoginGift";
    public const string FaceBookID = "FaceBookID";
    public const string FaceBookUserEmail = "FaceBookUserEmail";
    public const string FaceBookName = "FaceBookName";

    public const string MultiHint_Unlock = "multihintunlock";
    public const string MultiHint_Cost = "multihint_cost";
    public const string SpecificHint_Cost = "specifichint_cost";
    public const string NormalHint_Cost = "normalhint_cost";
    public const string SubscriptionMultiHintCount = "multihintsubcount";
    public const string SubscriptionMultiHintBeginTime = "sub_multihint_begin_time";


    //通知优化
    public const string disableNotiKey = "disableNotiKey";

    public const string fiveRatedKey = "fiveRatedKey";

    /// <summary>
    /// 评分是否给过金币
    /// </summary>
    public const string rateGiveCoin = "rateGiveCoin";

    //2/7 留存
    public const string uFirstOpenApp = "uFirstOpenApp";//时间值

    public const string twoDayRetention = "twoDayRetention";
    public const string sevenDayRetention = "sevenDayRetention";


    public const string PrivacyPolicyShown = "PrivacyPolicyShown";


    public const string New_Unlocked_World = "new_unlocked_world";
    public const string New_Unlocked_Sub_World = "new_unlocked_sub_world";
    public const string New_Unlocked_Level = "new_unlocked_level";




    //新的每日挑战
    public const string DailyGuidStep = "DailyGuidStep";

    public const string DailyGuidLevelIndex = "DailyGuidLevelIndex";
    public const string DailyGuidThreeInfo = "DailyGuidThreeInfo";

    // 大郭要求存的时间
    public const string SaveDate = "SD_SaveDate";

    public const string DeviceId = "DeviceId";

    public const string BIMainCompleteLevel = "BIMainCompleteLevel";
    public const string BIDailyCompleteLevel = "BIDailyCompleteLevel";
    public const string RequestPushAuthentication = "RequestPushAuthentication";
    public const string ReportNotification = "ReportNotification";
    public const string DDL_OnlineConfig = "DDL_OnlineConfig";
    public const string DDL_LevelMap = "DDL_LevelMap";
    public const string DDL_TurnIndex = "DDL_TurnIndex";
    public const string DDL_TurnTime = "DDL_TurnTime";

    public const string AC_CommonConfig = "AC_CommonConfig";
    public const string AC_RankInfo = "AC_RankInfo";
    public const string AC_RankData = "AC_RankData";
    public const string AC_NumberData = "AC_NumberData";
    public const string AC_GiftData = "AC_GiftData";

    public const string HS_first = "HS_first";
    public const string SFXEnable = "sound_enabled";
    public const string MusicEnable = "music_enabled";

    public const string ABTestingRandId = "ABTestingRandId";

	//任务
	public const string MS_t_ID = "MS_t_ID";//任务id
	public const string MS_F_N = "MS_F_N";//完成数
	public const string MS_G_N = "MS_G_N";//目标数
	public const string MS_S_N = "MS_S_N";//阶段数
	public const string MS_F_S = "MS_F_S";//完成状态
	public const string MS_U = "MS_U";
	public const string MS_Guide = "MS_Guide";
	public const string MS_Stamp = "MS_Stamp";
	public const string MS_ThisTime_C_N = "MS_ThisTime_C_N";//主线
	public const string MS_C_Index = "MS_C_Index";
	public const string MS_H_Index = "MS_H_Index";
	public const string MS_claim_S_N = "MS_claim_S_N";

	public const string Collect_Pet_Local = "Collect_Pet_Local";//pet 本地数据
	public const string Collect_Pet_Theme_Local = "Collect_Pet_Theme_Local";//pet 本地数据
	public const string Collect_Pet_Current_Used_Local = "Collect_Pet_Current_Used_Local";//pet 本地数据
	public const string Collect_Pet_Current_Used_Theme_Local = "Collect_Pet_Current_Used_Local";//pet 本地数据
	
	public const string Event_Before_Version = "Event_Before_Version";
	public const string Event_Version = "Event_Version";
	public const string Event_CurrentActivityIds = "Event_CurrentActivityIds";
	public const string Event_Active_Listt = "Event_Active_Listt";
	public const string PlayerNeedReGift = "PlayerNeedReGift";
	// 奖励界面 rewars 下的两种类型
	public const string PlayerChampionChipsRewads = "PlayerChampionChipsRewads";
	public const string PlayerAchievementRewards = "PlayerAchievementRewards";

	public const string Skin_RPL_D = "Skin_RPL_D_{0}_{1}_{2}";

	public const string LocalItemKey = "LocalItemKey";
	public const string OnLineItemKey = "BagOnLineItemKey";
	public const string OnLineUserInfoKey = "OnLineUserInfoKey";
}