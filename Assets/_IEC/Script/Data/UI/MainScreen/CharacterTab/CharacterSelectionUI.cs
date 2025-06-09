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
        public Image characterAvatar;
        public Image selectedFrame;
        public Vector2 selectedAvatarMaskSize = new Vector2(205, 205);
        public Image normalFrame;
        public Vector2 normalAvatarMaskSize = new Vector2(175, 175);
        public Image avatarMask;
        public Button btn;
        public CharacterConfig CharacterConfig { get; private set; }

        private void Start()
        {
            if(btn != null)
                btn.onClick.AddListener(OnSelectCharacter);
            
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, CheckCurrentCharacter);
        }

        public void Setup(CharacterConfig characterConfig, string currentCharacterName)
        {
            CharacterConfig = characterConfig;
            if(characterAvatar != null)
                characterAvatar.sprite = CharacterConfig.characterSelectionSprite;
            
            SetSelectedUI(CharacterConfig.characterName.Equals(currentCharacterName));
        }

        public void OnSelectCharacter()
        {
            this.RaiseEvent(EventList.GamePlay.SELECT_NEW_CHARACTER, new SelectCharacterData()
            {
                characterName = CharacterConfig.characterName
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
            selectedFrame.gameObject.SetActive(isSelected);
            normalFrame.gameObject.SetActive(!isSelected);

            avatarMask.GetComponent<RectTransform>().sizeDelta =
                isSelected 
                    ? selectedAvatarMaskSize 
                    : normalAvatarMaskSize;
        }
    }

    [Serializable]
    public class SelectCharacterData
    {
        public string characterName;
    }
}
