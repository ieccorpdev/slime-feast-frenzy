using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using IEC.TrackingEvent;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelWin : ScreenUI
    {
        [SerializeField] private GameplayTimer gameplayTimer;
        [SerializeField] private ScreenLevelWin_GroupStar groupStar;
        
        public GameEventInfoBar starChaseInfoBar;
        public GameEventInfoBar sharkRaceInfoBar;

        public GameObject groupGoldDisplay;
        public TextMeshProUGUI goldDisplayText;

        public Button btnNext;

        public int StarWon => gameplayTimer.CurrentRemainingTimeState switch
        {
            GameplayTimer.RemainingTimeState.None => 1,
            GameplayTimer.RemainingTimeState.Safe => 3,
            GameplayTimer.RemainingTimeState.Warning => 2,
            GameplayTimer.RemainingTimeState.Dangerous => 1,
            _ => 1
        };
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
            
            CheckShowGameEventInfoBar();
            
            PlayerData.UpdatePlayerStat(
                PlayerGeneralStat.CollectedStars
                , PlayerData.GetPlayerStat(PlayerGeneralStat.CollectedStars) + groupStar.StarWon);
        }
        
        private void CheckShowGameEventInfoBar()
        {
            Controllers.Get<EventRankController>().CheckAddPlayerProgress();
            sharkRaceInfoBar.gameObject.SetActive(false);
            starChaseInfoBar.gameObject.SetActive(false);
            if (PlayerData.level > Config.AllGameEventConfig.GetGameEventConfig(EventType.SharkRace).levelUnlock)//Dont show first time -> show start popup at home
            {
                if ((!Locator.Main.PopupSharkRace.IsEventCompleted() || PlayerData.GetEventSaveData(EventType.SharkRace).listPlayerData.Count == 0))
                {
                    sharkRaceInfoBar.gameObject.SetActive(true);
                    starChaseInfoBar.gameObject.SetActive(false);
                    sharkRaceInfoBar.SetupData();
                }
            }
            // else if (PlayerData.level > Config.AllGameEventConfig.GetGameEventConfig(EventType.StarChase).levelUnlock)// Dont show first time -> show start popup at home
            // {
            //     if (!Locator.Main.PopupStarChase.IsEventCompleted() || PlayerData.GetEventSaveData(EventType.StarChase).listPlayerData.Count == 0)
            //     {
            //         starChaseInfoBar.gameObject.SetActive(true);
            //         sharkRaceInfoBar.gameObject.SetActive(false);
            //         starChaseInfoBar.SetupData();
            //     }
            // }
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
            List<KeyValuePair<ResourceType, int>> listCollectedItems = new();

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
            
            //Check progress reward
            var reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            while(PlayerData.CheckClaimProgressReward(reward))
            {
                List<Reward> listReward = new(){reward.reward};
                // popupQueue.Enqueue(new PopupData(
                //     Locator.Main.ClaimBonusRewardPanel,
                //     () =>
                //     {
                //         Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(listReward, null);
                //         Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
                //     }, 
                //     ShowNext));
                Locator.Main.ClaimBonusRewardPanel.AddClaimRewardData(
                    new ShowClaimRewardData()
                    {
                        listDataReward = new List<Reward>(listReward),
                    });
                reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            }
            
            //Check level chest
            var levelChestRewardPopup = CheckAddChestRewardPopup(ChestType.Level, 1, ShowNext);
            if (levelChestRewardPopup != null)
            {
                //popupQueue.Enqueue(levelChestRewardPopup);
                
                var rewardData = Config.allChestConfig.GetChestConfig(ChestType.Level)
                    .GetRewardData(PlayerData.GetChestMilestone(ChestType.Level)-1);
                Locator.Main.ClaimBonusRewardPanel.AddClaimRewardData(
                    new ShowClaimRewardData()
                    {
                        listDataReward = new List<Reward>(rewardData.listReward),
                        showAimChest = true,
                        chestType = ClaimBonusRewardSkin.Chest_2
                    });
            }

            //Check star chest
            var starChestRewardPopup = CheckAddChestRewardPopup(ChestType.Star, groupStar.StarWon, ShowNext);
            if (starChestRewardPopup != null)
            {
                //popupQueue.Enqueue(starChestRewardPopup);
                
                var rewardData = Config.allChestConfig.GetChestConfig(ChestType.Star)
                    .GetRewardData(PlayerData.GetChestMilestone(ChestType.Star)-1);
                Locator.Main.ClaimBonusRewardPanel.AddClaimRewardData(
                    new ShowClaimRewardData()
                    {
                        listDataReward = new List<Reward>(rewardData.listReward),
                        showAimChest = true,
                        chestType = ClaimBonusRewardSkin.Chest_1
                    });
            }

            if (Locator.Main.ClaimBonusRewardPanel.IsHaveShowClaimRewardAnim())
            {
                popupQueue.Enqueue(new PopupData(
                    Locator.Main.ClaimBonusRewardPanel,
                    Locator.Main.ClaimBonusRewardPanel.ShowNext,
                    ShowNext
                ));
            }
            
            if (PlayerData.level == Config.AllGameEventConfig.GetGameEventConfig(EventType.StarChase).levelUnlock && !PlayerData.isShownPopupStartStarChase)
            {
                PopupData StartStarChase = new PopupData(
                    Locator.Main.PopupStartStarChaseEvent,
                    () =>
                    {
                        Locator.Main.PopupStartStarChaseEvent.Show();
                        PlayerData.isShownPopupStartStarChase = true;
                    },
                    ShowNext
                );
                popupQueue.Enqueue(StartStarChase);
            }
            
            if (PlayerData.level == Config.AllGameEventConfig.GetGameEventConfig(EventType.SharkRace).levelUnlock && !PlayerData.isShownPopupStartSharkRace)
            {
                PopupData StartSharkRace = new PopupData(
                    Locator.Main.PopupStartSharkRaceEvent,
                    () =>
                    {
                        Locator.Main.PopupStartSharkRaceEvent.Show();
                        PlayerData.isShownPopupStartSharkRace = true;
                    },
                    ShowNext
                );
                popupQueue.Enqueue(StartSharkRace);
            }

            if (PlayerData.level > Config.AllGameEventConfig.GetGameEventConfig(EventType.SharkRace).levelUnlock 
                && Locator.Main.PopupSharkRace.IsEndEventStage() 
                && PlayerData.GetEventSaveData(EventType.SharkRace).listPlayerData.Count > 0
                && !PlayerData.GetEventSaveData(EventType.SharkRace).claimedReward)
            {
                PopupData ClaimSharkRace = new PopupData(
                    Locator.Main.PopupSharkRace,
                    () =>
                    {
                        Locator.Main.PopupSharkRace.Show();
                    },
                    ShowNext
                );
                popupQueue.Enqueue(ClaimSharkRace);
            }
            
            if (PlayerData.level > Config.AllGameEventConfig.GetGameEventConfig(EventType.StarChase).levelUnlock 
                && Locator.Main.PopupStarChase.IsEndEventStage() 
                && PlayerData.GetEventSaveData(EventType.StarChase).listPlayerData.Count > 0
                && !PlayerData.GetEventSaveData(EventType.StarChase).claimedReward)
            {
                PopupData ClaimStarChase = new PopupData(
                    Locator.Main.PopupStarChase,
                    () =>
                    {
                        Locator.Main.PopupStarChase.Show();
                    },
                    ShowNext
                );
                popupQueue.Enqueue(ClaimStarChase);
            }
            
            #endregion
            
            #region Claim Reward Effect
            
            listCollectedItems.Add(new KeyValuePair<ResourceType, int>(ResourceType.LevelStar, groupStar.StarWon));
            if (GamePlay.levelController.GoldCollected > 0)
            {
                listCollectedItems.Add(new KeyValuePair<ResourceType, int>(ResourceType.Gold, GamePlay.levelController.GoldCollected));
            }

            if (GamePlay.levelController.SpecialProgressRewardObjCollected > 0)
            {
                listCollectedItems.Add(new KeyValuePair<ResourceType, int>(ResourceType.OceanGiftObject, GamePlay.levelController.SpecialProgressRewardObjCollected));
            }
            #endregion

            if (PlayerData.level >= Config.levelWinBackHome || popupQueue.Count > 0)
            {
                //GameController.OpenScene(GameScene.Main, callback ?? (() => { }));
                GameController.OpenScene(GameScene.Main, ()=>
                {
                    //ShowNext();
                    Locator.Main.HomeScreenTab.AddOpenSceneAction(
                        () =>
                        {
                            Locator.Main.HomeScreenTab.SpawnCollectItemOnEndGameEffect(listCollectedItems, ShowNext);
                        });
                });
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
                            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(chestType == ChestType.Level ? ClaimBonusRewardSkin.Chest_2: ClaimBonusRewardSkin.Chest_1);
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