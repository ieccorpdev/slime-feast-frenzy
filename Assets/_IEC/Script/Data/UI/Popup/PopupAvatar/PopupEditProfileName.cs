using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupEditProfileName : BasePopup
    {
        public int minCharacter = 1;
        public int characterLimit = 30;
        public TMP_InputField playerName;

        public Button btnSave;
        // Start is called before the first frame update
        void Start()
        {
            SetEvent(btnSave, SaveProfile);
            playerName.characterLimit = characterLimit;
        }

        public override void Hide()
        {
            //EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE);
            base.Hide();
        }

        private void SaveProfile()
        {
            if (playerName.text.Length <= minCharacter)
                return;
            
            PlayerData.UpdatePlayerProfileName(playerName.text);
            //EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE);
            OnHideClick();
        }
    }
}
