using System.Collections;
using System.Collections.Generic;
using _IEC;
using IEC;
using IEC.Wind.Leaderboard;
using UnityEngine;

public static partial class Locator
{
    public static class Main
    {
        public static ScreenMain ScreenMain => Pool.Get<ScreenMain>();
        //"Home Main"
        public static HomeScreenTab HomeScreenTab => Pool.Get<HomeScreenTab>();
        public static PopupAvatar PopupAvatar=> Pool.Get<PopupAvatar>();
        public static PopupLuckyWheel PopupLuckyWheel=> Pool.Get<PopupLuckyWheel>();
        public static PopupNoAds PopupNoAds=> Pool.Get<PopupNoAds>();
        public static PopupLifeOffer PopupLifeOffer=> Pool.Get<PopupLifeOffer>();
        public static PopupStarterPack PopupStarterPack=> Pool.Get<PopupStarterPack>();
        public static PopupPiggyBank PopupPiggyBank=> Pool.Get<PopupPiggyBank>();
        public static PopupDailyReward PopupDailyReward=> Pool.Get<PopupDailyReward>();
        public static PopupOutOfLive PopupOutOfLive => Pool.Get<PopupOutOfLive>();
        //LeaderBoardMain 
        public static PopupLeaderboard PopupLeaderboard => Pool.Get<PopupLeaderboard>();
        public static PopupRankInfo PopupRankInfo => Pool.Get<PopupRankInfo>();
        public static ClaimLeaderBoardFinish ClaimLeaderBoardFinish => Pool.Get<ClaimLeaderBoardFinish>();
        public static PopupDoubleMedal PopupDoubleMedal => Pool.Get<PopupDoubleMedal>();
        public static PopupEditInfo PopupEditInfo => Pool.Get<PopupEditInfo>();
        public static PopupGoldOffer PopupGoldOffer => Pool.Get<PopupGoldOffer>();
        public static PopupHardLevel PopupHardLevel => Pool.Get<PopupHardLevel>();

        public static PopupAddHelper PopupAddHelper => Pool.Get<PopupAddHelper>();
    } 
}
