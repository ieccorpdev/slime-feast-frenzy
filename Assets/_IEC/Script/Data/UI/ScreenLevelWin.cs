using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelWin : ScreenUI
    {
        public TextMeshProUGUI goldRewardText;
        public TextMeshProUGUI baseGoldRewardText;
        public BonusRewardWheel bonusRewardWheel;
        public Button btnBonusReward;
        public RectTransform rewardGroup;

        [SerializeField] private Image progress;
        [SerializeField] private Image iconFeature;
        [SerializeField] private Image mainBg;
        [SerializeField] private Image mainBg2;
        [SerializeField] private Image WhiteBanner;
        [SerializeField] private float animTopPosY = -250;
        //[SerializeField] private SpineAnimOT topAnimWin;
        public Button btnNext;
        

        private int _goldRewardAmount = 10;
        public TextMeshProUGUI txtBrickBuildCity;
        [Header("Medal Leaderboard")]
        public GameObject medalObj;
        public Image medalImage;
        public TextMeshProUGUI txtMedalAmount;
        
        private void Awake()
        {
            
        }
        private void Start()
        {
            listButtonHideOnClick.Add(btnNext);

            SetEvent(btnNext, () =>
            {
                Services.PushEvent(new TrackingEvent.ClickBtnNext());
                //EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS, new EventParam(this, name));
                OnNoThanksButtonClick();
            });
            
            
        SetEvent(btnBonusReward,
            () =>
        {
            Services.PushEvent(new TrackingEvent.ClickBtnTracking("level_win_bonus_reward"));
            OnAdsGetx3Gold();
        });
        }

        public override void Hide()
        {
            base.Hide();
            Pool.Get<ConfettiGroup>().StopConfetti();
        }
        public override void Show()
        {
            Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
            base.Show();
            baseGoldRewardText.text = _goldRewardAmount.ToString();
            bonusRewardWheel.InitWheel();
        }


        private void Update()
        {
            goldRewardText.text = bonusRewardWheel.GetFinalRewardCount(_goldRewardAmount).ToString();
        }
        
        void OnAdsGetx3Gold()
        {
            bonusRewardWheel.StopWheel();
            int finalCoinBonus = bonusRewardWheel.GetFinalRewardCount(_goldRewardAmount);
            goldRewardText.text = finalCoinBonus.ToString();
            Services.ShowRewardAds(() =>
            {
                //EventController.RaiseEvent(EventList.GamePlay.ADD_GOLD, new EventParam(this, finalCoinBonus, new Vector2(-280, -1390)));
                PlayerData.AddCurrency(ResourceType.Gold,finalCoinBonus - _goldRewardAmount);//Player was get the value instant when win game
                Services.PushEvent(new IEC.TrackingEvent.SRApostition($"ad_win_bonus_reward"));
                TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
                {
                    //Hide();
                    ActionCompletePopup();
                }, () =>
                {
                });
            }, null, true);
           
        }
        
        void OnNoThanksButtonClick()
        {

                Services.ShowFullAdsWithTime(Game.timeDelayFullAds, () =>
                {
                    bonusRewardWheel.StopWheel();
                    ActionCompletePopup();
                }, () =>
                {
                    Services.PushEvent(new IEC.TrackingEvent.SFApostition($"skip_win_bonus_reward"));
                });
        }
        
        private void ActionCompletePopup()
        {
            Hide();
            
            Action callback = null;
            
            //if (PlayerData.level >= Config.levelWinBackHome)
            {
                // Define the callback to show popups once Home scene is loaded
                callback = ShowPostWinPopups;
            }
            
            //Add callbacks here

            if (PlayerData.level >= Config.levelWinBackHome || callback != null)
            {
                GameController.OpenScene(GameScene.Main, callback ?? (() => { }));
            }
            else
            {
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            }
        }
        
        private void ShowPostWinPopups()
        {
            Queue<PopupData> popupQueue = new Queue<PopupData>();

            #region Helper functions
            PopupData CheckAddChestRewardPopup(ChestType chestType, int addProgress)
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
                                    .GetRewardData(PlayerData.GetChestMilestone(chestType));
                                Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewardData.listReward, null);
                                Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
                            }, 
                            ShowNext);
                }

                return popupData;
            }

            void ShowNext()
            {
                if (popupQueue.Count > 0)
                {
                    var popupData = popupQueue.Dequeue();
                    ShowPopup(popupData.popup, popupData.onOpen, popupData.onClose);
                }
            }
            #endregion

            //Check level chest
            var levelChestRewardPopup = CheckAddChestRewardPopup(ChestType.Level, 1);
            if(levelChestRewardPopup != null)
                popupQueue.Enqueue(levelChestRewardPopup);
            
            //Check star chest
            var starChestRewardPopup = CheckAddChestRewardPopup(ChestType.Star, 3);
            if(starChestRewardPopup != null)
                popupQueue.Enqueue(starChestRewardPopup);

            ShowNext(); // Start the chain
        }
        
        private void ShowPopup(BasePopup popup,Action onOpen, Action onClosed)
        {
            popup.AddActionOnceOnClose(() =>
            {
                onClosed?.Invoke(); // Called when popup is closed
            });
            onOpen?.Invoke();
            //popup.Show();
        }
        
        public void SetMedalReward(int medal)
        {
            bool showMedal = Game.isHaveLeaderboard && PlayerData.level >= LeaderboardConfig.levelUnlock;
            medalObj.gameObject.SetActive(showMedal);
            if (!showMedal) return;
            medalImage.sprite = LeaderboardConfig.GetSpirteCurrentLeagueLevelWin();
            txtMedalAmount.text = "" + medal;
        }

        public void DeactiveMedalObj()
        {
            medalObj.gameObject.SetActive(false);
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