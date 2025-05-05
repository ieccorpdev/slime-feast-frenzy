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
   


        public void SetMedalReward(int medal)
        {
        }
        
        private void ActionCompletePopup()
        {
            Luna.Unity.Playable.InstallFullGame();
            Luna.Unity.LifeCycle.GameEnded();
        }
      
  
    }
    

}