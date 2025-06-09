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
        public Image mainCharacterImage;
        public Button selectCharacterButton;
        public Button characterInUseButton;
        public CharacterSelectionUIScroller selectionScroller;
        public TextMeshProUGUI characterNameText;
        
        public CharacterConfig CurrentCharacterConfig { get; private set; }

        public bool IsCurrentShowingSelected => CurrentCharacterConfig != null &&
                                                CurrentCharacterConfig.characterName
                                                    .Equals(PlayerData.playerCharacterName);
        
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
            UpdateCurrentCharacterUI();
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
            if (mainCharacterImage != null)
            {
                mainCharacterImage.sprite = CurrentCharacterConfig.characterMainSprite;
                mainCharacterImage.SetNativeSize();
            }

            UpdateGroupButtonsUI();
        }

        private void UpdateGroupButtonsUI()
        {
            selectCharacterButton.gameObject.SetActive(!IsCurrentShowingSelected);
            characterInUseButton.gameObject.SetActive(IsCurrentShowingSelected);
        }

        public void Setup()
        {
            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(PlayerData.playerCharacterName);
            UpdateCurrentCharacterUI();
            selectionScroller.Init(this);
        }
    }
}
