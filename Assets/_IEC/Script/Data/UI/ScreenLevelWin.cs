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
            //+1 because when win level add 1
            if (Game.isLevelSFAAdjustment && PlayerData.level < Game.levelSFAAdjust+1)//now 8 
            {
                bonusRewardWheel.StopWheel();
            
                ActionCompletePopup();
            }
            else
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
        }
        
        private void ActionCompletePopup()
        {
            Hide();
            
            Action callback = null;
            
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

}