using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupUnlockCharacter : BasePopup
    {
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI tapToContinueText;
        public Image mainCharacterImage;
        public CharacterTabStarGroup starGroup;
        public TextMeshProUGUI characterDescriptionText;

        public CharacterConfig CharacterConfig { get; private set; }

        private string originalTitleText;
        private string originalTapText;

        private void Start()
        {
            originalTitleText = titleText.text;
        }

        public void Setup(CharacterConfig characterConfig, string titleHeaderText = null, string tapText = null)
        {
            CharacterConfig = characterConfig;
            characterNameText.text = CharacterConfig.characterName;
            mainCharacterImage.sprite = CharacterConfig.characterMainSprite;
            starGroup.UpdateStarUI(PlayerData.GetCharacterLevel(CharacterConfig.characterName));
            //characterDescriptionText.text = CharacterConfig.characterDescription;
            characterDescriptionText.text = Locator.Main.CharacterHomeTab.characterSelectionTab.GetDescriptionText(CharacterConfig);

            if (!String.IsNullOrEmpty(titleHeaderText))
            {
                titleText.text = titleHeaderText;
            }
            
            if (!String.IsNullOrEmpty(tapText))
            {
                tapToContinueText.text = tapText;
            }
        }

        public override void Hide()
        {
            base.Hide();

            titleText.text = originalTitleText;
            tapToContinueText.text = originalTapText;
        }
    }
}
