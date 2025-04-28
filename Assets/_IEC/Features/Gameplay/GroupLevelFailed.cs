using System;
using System.Collections.Generic;
using _IEC.Features.Booster;
using IEC;
using IEC.TrackingEvent;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupLevelFailed : BasePopup
    {
        [SerializeField] private Button btnRetry;
        [SerializeField] private GroupBoosterInLevelFail groupBooster;
        [SerializeField] private GameObject groupDefault;
        [SerializeField] private Button homeButton;
        
        public event Action OnClose;
        public event Action OnRetry;
        public GroupBoosterInLevelFail GetGroupBooster()
        {
            return groupBooster;
        }
        protected override void Awake()
        {
            base.Awake();
            SetEvent(btnClose, () =>
            {
                //SoundController.PlayExitBtnAudio();
                OnClose?.Invoke();
                EventController.RaiseEvent(EventList.Audio.ACTIVE_BUTTON_CLICK);
            });
            SetEvent(btnRetry, () =>
            {
                //SoundController.PlayClickBtnAudio();
                Services.PushEvent(new ClickBtnTracking("losescreen_retry"));
                OnRetry?.Invoke();
                EventController.RaiseEvent(EventList.Audio.ACTIVE_BUTTON_CLICK);
            });
            SetEvent(homeButton, () =>
            {
                //SoundController.PlayExitBtnAudio();
                GameController.OpenScene(GameScene.Main);
            });
        }
        
        public void ShowBoosterGroup(List<BoosterId> boosterIds)
        {
            groupBooster.Show(boosterIds);
            groupDefault.SetActive(false);
        }

        public void HideBoosterGroup()
        {
            groupBooster.gameObject.SetActive(false);
            groupDefault.SetActive(true);
        }

        public override void Show()
        {
            isPlayAudioPopup = false;
            //MagicPouchController.OnLevelLose();
            base.Show();
            EventController.RaiseEvent(EventList.Audio.LOSE);
        }
    }
}