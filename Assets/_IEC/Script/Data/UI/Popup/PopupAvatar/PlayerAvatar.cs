using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC.Common.Event;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PlayerAvatar : _BaseBehaviour
    {
        public Image playerAvatar;
        public Image playerAvatarFrame;

        public Button btnShowProfile;
        [SerializeField] private GameObject notiLockInfo;

        void Awake()
        {
            gameObject.OnEvent(EventList.GamePlay.UPDATE_PLAYER_AVATAR_PROFILE, UpdateAvatarUI);

            if (btnShowProfile != null)
            {
                btnShowProfile.onClick.RemoveAllListeners();
                btnShowProfile.onClick.AddListener(
                    () =>
                    {
                        Locator.Main.PopupProfile.Show();
                    });
            }
            HideNoti();
        }

        public void ShowNoti()
        {
            if (notiLockInfo == null)
                return;
            TweenControl.GetInstance().ScaleFromZero(notiLockInfo, 0.5f);
        }

        public void HideNoti()
        {
            if (notiLockInfo == null)
                return;
            notiLockInfo.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (notiLockInfo == null)
                return;
            if (PlayerData.journeyData.isEnableMaxHeart)
            {
                if (Input.GetMouseButtonDown(0) && PlayerData.journeyData.isShowNoti)
                {
                    HideNoti();
                }
                if (!PlayerData.journeyData.isShowNoti && Locator.Main.PopupJourney.GetHideStatus())
                {
                    ShowNoti();
                    PlayerData.journeyData.isShowNoti = true;
                }
            }
        }

        private void OnEnable()
        {
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_AVATAR_PROFILE);
        }

        public void UpdateAvatarUI()
        {
            playerAvatar.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
            playerAvatarFrame.sprite = Config.playerAvatarConfig.GetAvatarFrameSprite(PlayerData.playerAvatarFrameName);
        }
    }
}
