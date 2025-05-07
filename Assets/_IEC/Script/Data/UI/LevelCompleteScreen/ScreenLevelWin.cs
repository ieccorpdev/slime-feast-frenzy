using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelWin : ScreenUI
    {
        [SerializeField] private GameplayTimer gameplayTimer;
        [SerializeField] private ScreenLevelWin_GroupStar groupStar;

        public GameObject groupGoldDisplay;
        public TextMeshProUGUI goldDisplayText;

        public Button btnNext;
        private void Start()
        {
            listButtonHideOnClick.Add(btnNext);

            SetEvent(btnNext, () =>
            {
                Services.PushEvent(new TrackingEvent.ClickBtnNext());
                //ActionCompletePopup();
                Services.ShowFullAdsWithTimeAndCondition(Game.ShowFullAdsCondition, Game.GetTimeDelayFullAds(),
                    ActionCompletePopup,
                    () =>
                    {
                        Services.PushEvent(new TrackingEvent.SFApostition("win_screen_next"));
                    });
            });
        }

        public override void Hide()
        {
            base.Hide();
            Pool.Get<ConfettiGroup>().StopConfetti();
        }
        public override void Show()
        {
            Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(false);
            groupStar.DoShowStarEffect(gameplayTimer.CurrentRemainingTimeState);
            SetupGoldDisplay();
            base.Show();
        }

        void SetupGoldDisplay()
        {
            groupGoldDisplay.SetActive(GamePlay.levelController.GoldCollected > 0);
            goldDisplayText.text = GamePlay.levelController.GoldCollected.ToString();
        }


        public void SetMedalReward(int medal)
        {
            // bool showMedal = Game.isHaveLeaderboard && PlayerData.level >= LeaderboardConfig.levelUnlock;
            // medalObj.gameObject.SetActive(showMedal);
            // if (!showMedal) return;
            // medalImage.sprite = LeaderboardConfig.GetSpirteCurrentLeagueLevelWin();
            // txtMedalAmount.text = "" + medal;
        }
        
        private void ActionCompletePopup()
        {
            Hide();
            
            // Action callback = null;
            //
            // //if (PlayerData.level >= Config.levelWinBackHome)
            // {
            //     // Define the callback to show popups once Home scene is loaded
            //     callback = ShowPostWinPopups;
            // }
            
            //Add callbacks here
            
            #region check add popup
            Queue<PopupData> popupQueue = new Queue<PopupData>();

            #region Helper functions

            void ShowNext()
            {
                if (popupQueue.Count > 0)
                {
                    var popupData = popupQueue.Dequeue();
                    ShowPopup(popupData.popup, popupData.onOpen, popupData.onClose);
                }
            }
            #endregion
            if (PlayerData.level > Config.levelShowRating && !PlayerData.wasRateUs)
            {
                PopupData CallNativeReview = new PopupData(
                    null,
                    Services.RateUs,
                    ShowNext
                );
                popupQueue.Enqueue(CallNativeReview);
            }
            //Check level chest
            var levelChestRewardPopup = CheckAddChestRewardPopup(ChestType.Level, 1, ShowNext);
            if(levelChestRewardPopup != null)
                popupQueue.Enqueue(levelChestRewardPopup);
            
            //Check star chest
            var starChestRewardPopup = CheckAddChestRewardPopup(ChestType.Star, groupStar.StarWon, ShowNext);
            if(starChestRewardPopup != null)
                popupQueue.Enqueue(starChestRewardPopup);
            
            //Check progress reward
            var reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            while(PlayerData.CheckClaimProgressReward(reward))
            {
                List<Reward> listReward = new(){reward.reward};
                popupQueue.Enqueue(new PopupData(
                    Locator.Main.ClaimBonusRewardPanel,
                    () =>
                    {
                        Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(listReward, null);
                        Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
                    }, 
                    ShowNext));
                reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            }
            
            #endregion

            if (PlayerData.level >= Config.levelWinBackHome || popupQueue.Count > 0)
            {
                //GameController.OpenScene(GameScene.Main, callback ?? (() => { }));
                GameController.OpenScene(GameScene.Main, ShowNext);
            }
            else
            {
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            }
        }
        
        PopupData CheckAddChestRewardPopup(ChestType chestType, int addProgress, Action onClose = null)
        {
            PopupData popupData = null;
                
            PlayerData.AddChestProgress(
                Config.allChestConfig.GetChestConfig(chestType), 
                addProgress, 
                out var isClaimChestReward);

            if (isClaimChestReward)
            {
                popupData = 
                    new PopupData(
                        Locator.Main.ClaimBonusRewardPanel,
                        () =>
                        {
                            var rewardData = Config.allChestConfig.GetChestConfig(chestType)
                                .GetRewardData(PlayerData.GetChestMilestone(chestType)-1);
                            //-1 by MeoMew fix Task #67686, reduce 1 level chest to get true reward
                            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewardData.listReward, null);
                            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
                        }, 
                        onClose);
            }

            return popupData;
        }
        
        private void ShowPopup(BasePopup popup,Action onOpen, Action onClosed)
        {
            if (popup != null)
            {
                popup.AddActionOnceOnClose(() =>
                {
                    onClosed?.Invoke(); // Called when popup is closed
                });
                onOpen?.Invoke();
            }
            else
            {
                //Custom rating thread
                onOpen?.Invoke();
                onClosed?.Invoke();
            }


        }

        public void DeactiveMedalObj()
        {
            //medalObj.gameObject.SetActive(false);
        }
    }
    
    public class PopupData
    {
        public BasePopup popup;
        public Action onOpen;
        public Action onClose;

        public PopupData(BasePopup popup, Action onOpen = null, Action onClose = null)
        {
            this.popup = popup;
            this.onOpen = onOpen;
            this.onClose = onClose;
        }
    }

}