using System.Collections;
using System.Collections.Generic;
using _IEC;
using _IEC.Features.VipProHackerUITutorial;
using IEC;
using UnityEngine;
using IEC.CloudSave;

public static partial class Locator
{
    public static class Main
    {
        public static ScreenMain ScreenMain => Pool.Get<ScreenMain>();
        
        //"Home Main"
        public static HomeScreenTab HomeScreenTab => Pool.Get<HomeScreenTab>();
        public static PopupEditProfile PopupEditProfile=> Pool.Get<PopupEditProfile>();
        public static PopupProfile PopupProfile => Pool.Get<PopupProfile>();
        public static PopupEditProfileName PopupEditProfileName => Pool.Get<PopupEditProfileName>();
        public static PopupLuckyWheel PopupLuckyWheel=> Pool.Get<PopupLuckyWheel>();
        public static PopupNoAds PopupNoAds=> Pool.Get<PopupNoAds>();
        public static PopupLifeOffer PopupLifeOffer=> Pool.Get<PopupLifeOffer>();
        public static PopupStarterPack PopupStarterPack=> Pool.Get<PopupStarterPack>();
        public static PopupPiggyBank PopupPiggyBank=> Pool.Get<PopupPiggyBank>();
        public static PopupDailyReward PopupDailyReward=> Pool.Get<PopupDailyReward>();
        public static PopupProgressReward PopupProgressReward => Pool.Get<PopupProgressReward>();
        public static PopupOutOfLive PopupOutOfLive => Pool.Get<PopupOutOfLive>();

        public static ClaimBonusRewardPanel ClaimBonusRewardPanel => Pool.Get<ClaimBonusRewardPanel>();
        public static PopupLevelChestReward PopupLevelChestReward => Pool.Get<PopupLevelChestReward>();
        public static PopupStarChestReward PopupStarChestReward => Pool.Get<PopupStarChestReward>();
        public static PopupGoldOffer PopupGoldOffer => Pool.Get<PopupGoldOffer>();
        public static PopupHardLevel PopupHardLevel => Pool.Get<PopupHardLevel>();

        public static PopupAddHelper PopupAddHelper => Pool.Get<PopupAddHelper>();
        public static PopupBooster PopupBooster => Pool.Get<PopupBooster>();
        public static PopupGiftCode PopupGiftCode => Pool.Get<PopupGiftCode>();
        public static PopupCloudSave PopupCloudSave => Pool.Get<PopupCloudSave>();
        public static PopupSettingInHome PopupSettingInHome => Pool.Get<PopupSettingInHome>();
        public static PopupStarChase PopupStarChase => Pool.Get<PopupStarChase>();
        public static PopupStartStarChaseEvent PopupStartStarChaseEvent => Pool.Get<PopupStartStarChaseEvent>();

        public static PopupSharkRace PopupSharkRace => Pool.Get<PopupSharkRace>();
        public static PopupStartSharkRaceEvent PopupStartSharkRaceEvent => Pool.Get<PopupStartSharkRaceEvent>();

        public static PopupJourney PopupJourney => Pool.Get<PopupJourney>();

        public static PopupVoyagePassIAPInfor PopupVoyagePassIAPInfor => Pool.Get<PopupVoyagePassIAPInfor>();

        public static PopupUnderseaVoyageIntro PopupUnderseaVoyageIntro => Pool.Get<PopupUnderseaVoyageIntro>();
        
        public static PopupChompOff PopupChompOff => Pool.Get<PopupChompOff>();

        public static PopupRelicHunt PopupRelicHunt => Pool.Get<PopupRelicHunt>();

        public static HighLightCanvas HighLightCanvas => Pool.Get<HighLightCanvas>();

    } 
}
