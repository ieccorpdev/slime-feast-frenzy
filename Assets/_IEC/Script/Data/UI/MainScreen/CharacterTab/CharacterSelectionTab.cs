using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterSelectionTab : _BaseBehaviour
    {
        public Button selectCharacterButton;
        public CharacterSelectionUIScroller selectionScroller;
        public TextMeshProUGUI characterNameText;
        
        public CharacterConfig CurrentCharacterConfig { get; private set; }
        
        // Start is called before the first frame update
        void Start()
        {
            selectCharacterButton.onClick.AddListener(OnSelectCharacter);
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, SetCurrentCharacter);
        }

        private void OnSelectCharacter()
        {
            PlayerData.playerCharacterName = CurrentCharacterConfig.characterName;
            this.RaiseEvent(EventList.GamePlay.SET_NEW_CHARACTER, new SelectCharacterData()
            {
                characterName = PlayerData.playerCharacterName
            });
        }

        private void SetCurrentCharacter(object param)
        {
            var data = (SelectCharacterData)param;

            if (data == null)
                return;

            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(data.characterName);
            UpdateCurrentCharacterUI();
        }

        private void UpdateCurrentCharacterUI()
        {
            characterNameText.text = CurrentCharacterConfig.characterName;
        }

        public void Setup()
        {
            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(PlayerData.playerCharacterName);
            UpdateCurrentCharacterUI();
            selectionScroller.Init();
        }
    }
}
