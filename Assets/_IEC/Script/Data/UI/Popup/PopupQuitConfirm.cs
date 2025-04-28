using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC
{
    public class PopupQuitConfirm : BasePopup
    {
        [SerializeField] private GroupCurrencyResult groupCurrencyResult;
        public Button resumeButton;

        public void Start()
        {
            SetEvent(resumeButton, () =>
            {
                Hide();
            });           
            SetEvent(btnClose, () =>
            {
                Services.ShowFullAdsWithTimeAndCondition(Game.ShowFullAdsCondition, Game.GetTimeDelayFullAds(),
                    () =>
                    {
                        PlayerData.DecreasePlayerLife();
                        EventController.RaiseEvent(EventList.Audio.LIFE_LOST);
                        GameController.OpenScene(GameScene.Main);
                        EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
                    },
                    () =>
                    {
                        Services.PushEvent(new TrackingEvent.SFApostition("in_game_setting_quit"));
                    });
            });
            
        }

        public override void Show()
        {
            base.Show();
            groupCurrencyResult.UpdateUI();
        }
    }
}