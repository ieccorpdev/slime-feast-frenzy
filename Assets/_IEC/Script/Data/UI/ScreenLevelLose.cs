using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace IEC
{
    public class ScreenLevelLose : ScreenUI
    {
        public Button btnRetry;
        public Button btnRetryAds;
        public Button btnKeepPlaying;
        public Button btnGiveUp;
        public Button addSpaceBtn;
        public Button backHomeBtn;
        public TextMeshProUGUI goldNeedUseHelper;
        private HelperData dataAddSpace;

        public GameObject hardLevelGroup;
        public GameObject levelFailGroups;
        public GameObject outOfLifeGroups;
        public TextMeshProUGUI nextTimeLifeTMP;
        public List<GameObject> listPackIAPLose;
        private void Start()
        {
            listButtonHideOnClick.Add(btnRetry);

            SetEvent(btnRetry, () =>
            {
                OnRetryLife();
            });
            
            SetEvent(btnGiveUp,
                () =>
                {
                    Services.PushEvent(new TrackingEvent.ClickBtnTracking("lose_give_up"));
                    EventController.RaiseEvent(EventList.GamePlay.GO_MAIN);
                    EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
                    EventController.RaiseEvent(EventList.Audio.VITALITY_PACK_POPUP_OPEN);
                });      
          
            SetEvent(backHomeBtn,
                () =>
                {
                    if(Services.ShowFullAdsWithTime(Game.timeDelayFullAds, () =>
                    {
                        Services.PushEvent(new TrackingEvent.ClickBtnTracking("lose_back_home"));
                        GameController.OpenScene(GameScene.Main);
                    }, () =>
                    {
                        Services.PushEvent(new IEC.TrackingEvent.SFApostition($"lose_back_home"));
                    }));


                });            
            SetEvent(btnRetryAds,
                () =>
                {
                    Services.ShowRewardAds(() =>
                    {
                        Services.PushEvent(new TrackingEvent.SRApostition("RESTART_LOSE"));
                        if (PlayerData.IsHardLevel())
                        {
                            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_HARD_LEVEL);
                        }
                        else
                        {
                            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                        }
                        Hide();
                    });
                });
        }

        public void Update()
        {
            nextTimeLifeTMP.text = PlayerData.GetTimeToNextLife();
        }
        public override void Show()
        {
            UpdateUI();
            base.Show();
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
        }

        public void UpdateUI()
        {
            if(hardLevelGroup != null)
                hardLevelGroup.gameObject.SetActive(GamePlay.IsInLevelHard);
            
            ActiveGroups(PlayerData.playerLife <=0 && !PlayerData.IsInUnlimitedLife());
            ActivePopupLosePackIap();
        }

        private void ActivePopupLosePackIap()
        {
            foreach (var iappack in listPackIAPLose)
            {
                iappack.gameObject.SetActive(false);
            }
            listPackIAPLose[Random.Range(0,listPackIAPLose.Count)].gameObject.SetActive(true);
        }
        private void ActiveGroups(bool outOfLive)
        {
            levelFailGroups.gameObject.SetActive(!outOfLive);
            btnRetry.gameObject.SetActive(!outOfLive);
            outOfLifeGroups.gameObject.SetActive(outOfLive);
            btnRetryAds.gameObject.SetActive(outOfLive);
        }

        public void OnRetryLife()
        {
            if (PlayerData.IsInUnlimitedLife() || PlayerData.playerLife > 0)
            {
                EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
                EventController.RaiseEvent(EventList.Audio.VITALITY_PACK_POPUP_OPEN);
                Services.PushEvent(new TrackingEvent.ClickBtnRetry());
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS, new EventParam(this, "lose_level_retry_life"));
                Hide();
            }
            else
            {
                EnableGropusOutOfLife();
            }

        }

        public void EnableGropusOutOfLife()
        {
            //Add Animation here
            ActiveGroups(true);
        }
    }
}