using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterSelectionUI : EnhancedScrollerCellView
    {
        public Button btn;
        public CharacterConfig CharacterConfig { get; private set; }

        public PlayerSecret PlayerData => Pool.Get<PlayerSecret>();

        private void Start()
        {
            if(btn != null)
                btn.onClick.AddListener(OnSelectCharacter);
            
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, CheckCurrentCharacter);
        }

        public void Setup(CharacterConfig characterConfig)
        {
            CharacterConfig = characterConfig;
            SetSelectedUI(CharacterConfig.characterName.Equals(PlayerData.playerCharacterName));
        }

        public void OnSelectCharacter()
        {
            PlayerData.playerCharacterName = CharacterConfig.characterName;
            this.RaiseEvent(EventList.GamePlay.SELECT_NEW_CHARACTER, new SelectCharacterData()
            {
                characterName = PlayerData.playerCharacterName
            });
        }

        private void CheckCurrentCharacter(object param = null)
        {
            var data = (SelectCharacterData)param;

            if (data != null)
            {
                // if (CharacterConfig.characterName.Equals(data.characterName))
                // {
                //
                // }
                
                SetSelectedUI(CharacterConfig.characterName.Equals(data.characterName));
            }
        }

        private void SetSelectedUI(bool isSelected)
        {
            
        }
    }

    [Serializable]
    public class SelectCharacterData
    {
        public string characterName;
    }
}
