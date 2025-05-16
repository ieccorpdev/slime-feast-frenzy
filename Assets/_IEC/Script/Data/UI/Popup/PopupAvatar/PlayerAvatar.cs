using System;
using System.Collections;
using System.Collections.Generic;
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
            
            UpdateAvatarUI();
        }

        public void UpdateAvatarUI()
        {
            playerAvatar.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
            playerAvatarFrame.sprite = Config.playerAvatarConfig.GetAvatarFrameSprite(PlayerData.playerAvatarFrameName);
        }
    }
}
