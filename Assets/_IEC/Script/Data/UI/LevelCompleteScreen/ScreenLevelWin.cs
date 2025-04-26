using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
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

        public Button btnNext;
        private void Start()
        {
            listButtonHideOnClick.Add(btnNext);

            SetEvent(btnNext, () =>
            {
                ActionCompletePopup();
            });

        }

        public override void Hide()
        {
            Pool.Get<ConfettiGroup>().StopConfetti();
            gameObject.SetActive(false);
        }
        public override void Show()
        {
            //groupStar.DoShowStarEffect(gameplayTimer.CurrentRemainingTimeState);
            base.Show();

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