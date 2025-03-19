using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupHardLevel : BasePopup
    {
        public Button allBgBtn;
        public Button normalCloseBtn;
        public Button btnPlay, btnSkip, btnSkipAds;

        public TextMeshProUGUI rewardText;

        protected override void Awake()
        {
            base.Awake();
            SetEvent(allBgBtn, OnHideClick);
            
            SetEvent(btnPlay,
                () =>
                {
                    // EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_HARD_LEVEL);
                    Services.PushEvent(new TrackingEvent.ClickBtnTracking("click_play_hard_level"));
                    Hide();
                    //PlayerData.isPlayLevelHardFirstTime = false;
                    Locator.Main.HomeScreenTab.DoMoveOutAllObj(0.3f,
                        () =>
                        {
                            Services.PushEvent(new IEC.TrackingEvent.ClickPlayHomeScreen(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold).ToString()));
                            GameController.OpenScene(GameScene.Game, 
                                ()=>{EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_HARD_LEVEL);}, 
                                false);
                        },0.1f);
                });
            SetEvent(normalCloseBtn,Hide);
            SetEvent(btnSkip,
                () =>
                {
                    //EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                    Services.PushEvent(new TrackingEvent.ClickBtnTracking("click_skip_hard_level"));
                    OnSkip();
                });
            
            SetEvent(btnSkipAds,
                () =>
                {
                    Services.ShowRewardAds(()=>
                    {
                        Services.PushEvent(new IEC.TrackingEvent.SRApostition("skip_hard_level_ads"));   
                        Services.PushEvent(new TrackingEvent.ClickBtnTracking("click_skip_hard_level_ads"));
                        OnSkip();
                    });
                });
            
            btnSkipAds.gameObject.SetActive(false);
        }

        private void OnSkip()
        {
            Hide();
            PlayerData.lastHardLevel = PlayerData.level;
            //PlayerData.isPlayLevelHardFirstTime = true;
            PlayerData.currentCountLoseHardLevel = 0;
            PlayerData.currentHardLevel++;
            Locator.Main.HomeScreenTab.UpdateMainHome();
        }

        public override void Hide()
        {
            base.Hide();
            
        }

        public override void Show()
        {
            base.Show();
            SetupData();
        }

        public void SetupData()
        {
            var loseBeforeAllowSkip = Game.isOverrideHardLevelConfig
                ? Game.loseBeforeAllowSkipHardLevelOverride
                : Config.loseBeforeAllowSkipHardLevel;
            btnSkip.gameObject.SetActive(PlayerData.currentCountLoseHardLevel >= loseBeforeAllowSkip);
            rewardText.text = "x" + Config.amountGoldRewardMultiply;

            if (Game.isAllowSkipHardLevelAds)
            {
                btnSkipAds.gameObject.SetActive(true);
                btnSkip.gameObject.SetActive(false);
            }
        }
    }
}
