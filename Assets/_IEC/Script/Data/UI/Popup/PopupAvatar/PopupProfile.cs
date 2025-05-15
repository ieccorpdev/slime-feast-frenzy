using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupProfile : BasePopup
    {
        public Button btnEditProfile;
        public TextMeshProUGUI playerNameText;

        public TextMeshProUGUI playerLevelText;
        // Start is called before the first frame update
        void Start()
        {
            SetEvent(btnEditProfile,
                () =>
                {
                    Locator.Main.PopupEditProfile.Show();
                });
            
            gameObject.OnEvent(EventList.GamePlay.UPDATE_PLAYER_NAME, UpdatePlayerName);
            UpdatePlayerName();
            UpdatePlayerLevelText();
        }

        private void UpdatePlayerLevelText()
        {
            playerLevelText.text = PlayerData.level.ToString();
        }
        
        private void UpdatePlayerName()
        {
            playerNameText.text = PlayerData.playerName;
        }
    }
}
