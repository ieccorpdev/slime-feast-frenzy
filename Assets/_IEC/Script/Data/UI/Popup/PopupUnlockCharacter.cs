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
        public Image mainCharacterImage;
        public CharacterTabStarGroup starGroup;
        public TextMeshProUGUI characterDescriptionText;

        public CharacterConfig CharacterConfig { get; private set; }
        
        public void Setup(CharacterConfig characterConfig)
        {
            CharacterConfig = characterConfig;
            characterNameText.text = CharacterConfig.characterName;
            mainCharacterImage.sprite = CharacterConfig.characterMainSprite;
            starGroup.UpdateStarUI(PlayerData.GetCharacterLevel(CharacterConfig.characterName));
            //characterDescriptionText.text = CharacterConfig.characterDescription;
            characterDescriptionText.text = Locator.Main.CharacterHomeTab.characterSelectionTab.characterDescriptionText.text;
        }
    }
}
