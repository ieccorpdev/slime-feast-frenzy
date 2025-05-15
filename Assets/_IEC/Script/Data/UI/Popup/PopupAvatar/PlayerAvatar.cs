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
        
        // Start is called before the first frame update
        void Start()
        {
            gameObject.OnEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE, UpdateAvatarUI);

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

        private void OnEnable()
        {
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE);
        }

        public void UpdateAvatarUI()
        {
            playerAvatar.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
            playerAvatarFrame.sprite = Config.playerAvatarConfig.GetAvatarFrameSprite(PlayerData.playerAvatarFrameName);
        }
    }
}
