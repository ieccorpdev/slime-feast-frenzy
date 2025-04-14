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
                Services.PushEvent(new TrackingEvent.ClickBtnNext());
                ActionCompletePopup();
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

        public void DeactiveMedalObj()
        {
            //medalObj.gameObject.SetActive(false);
        }
    }

}