using System;
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
        
        public TextMeshProUGUI playerDaysPlayedText;

        public List<PlayerStatUI> listPlayerStatUI = new();
        
        // Start is called before the first frame update
        void Start()
        {
            SetEvent(btnEditProfile,
                () =>
                {
                    Locator.Main.PopupEditProfile.Show();
                });
            
            gameObject.OnEvent(EventList.GamePlay.UPDATE_PLAYER_NAME, UpdatePlayerName);
        }

        public override void Show()
        {
            base.Show();
            UpdatePlayerName();
            UpdatePlayerLevelText();
            UpdatePlayerStatUI();
        }

        private void UpdatePlayerLevelText()
        {
            playerLevelText.text = PlayerData.level.ToString();
        }
        
        private void UpdatePlayerName()
        {
            playerNameText.text = PlayerData.playerName;
        }

        private void UpdatePlayerStatUI()
        {
            playerDaysPlayedText.text 
                = Mathf.FloorToInt(
                    (float)
                    (DateTime.Now - DateTime.Parse(Pool.Get<ServiceData>().installTime, 
                        null, 
                        System.Globalization.DateTimeStyles.RoundtripKind)).TotalDays)
                    .ToString() + " days";

            foreach (var stat in listPlayerStatUI)
            {
                stat.UpdateText(PlayerData.GetPlayerStat(stat.statType).ToString());
            }
        }
    }

    public enum PlayerGeneralStat
    {
        FirstTryWin,
        CollectedStars,
        HighestComboCount
    }
}
