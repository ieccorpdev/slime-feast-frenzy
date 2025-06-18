using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using _IEC.Features.Common.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterSelectionTab : _BaseBehaviour
    {
        public Image mainCharacterImage;
        public Image characterShardImage;
        public Button selectCharacterButton;
        public Button deselectCharacterButton;
        public Button levelUpCharacterButton;
        public CharacterSelectionUIScroller selectionScroller;
        public TextMeshProUGUI characterNameText;
        public GameObject groupCharacterInfo;
        
        [Header("Description Text")]
        public TextMeshProUGUI characterDescriptionText;
        public float highlightTextSizeIncrease;
        public Color highlightTextColor;

        [Header("Navigate Button")] 
        public Button nextButton;
        public Button previousButton;

        [Header("Character Stat UI")] 
        public List<CharacterTabStatDisplay> listCharacterStatUI = new();

        [Header("Star UI")] 
        public List<CharacterTabStarUI> listStarUI = new();
        
        [Header(" Upgrade button UI")] 
        public TextMeshProUGUI shardCountText;
        public TextMeshProUGUI inactiveShardCountText;
        public GameObject upgradeBtnUpgradableGroup;
        public TextMeshProUGUI upgradeBtnUpgradableText;
        public GameObject upgradeBtnInactiveGroup;
        public TextMeshProUGUI upgradeBtnInactiveText;
        public GameObject upgradeBtnMaxLevelGroup;
        
        public CharacterConfig CurrentCharacterConfig { get; private set; }
        public int CurrentCharacterLevel { get; private set; }

        public bool IsCurrentShowingSelected => CurrentCharacterConfig != null &&
                                                CurrentCharacterConfig.characterName
                                                    .Equals(PlayerData.playerCharacterName);
        
        // Start is called before the first frame update
        void Start()
        {
            selectCharacterButton.onClick.AddListener(OnSelectCharacter);
            deselectCharacterButton.onClick.AddListener(OnDeselectCharacter);
            levelUpCharacterButton.onClick.AddListener(OnLevelUpCharacter);
            previousButton.onClick.AddListener(OnPreviousButtonClick);
            nextButton.onClick.AddListener(OnNextButtonClick);
        }

        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, SetCurrentCharacter);
            this.RegisterListener(EventList.GamePlay.SET_NEW_CHARACTER, UpdateCurrentCharacterUI);
            this.RegisterListener(EventList.GamePlay.UPGRADE_CHARACTER, UpdateCurrentCharacterUI);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, SetCurrentCharacter);
            this.UnregisterListener(EventList.GamePlay.SET_NEW_CHARACTER, UpdateCurrentCharacterUI);
            this.UnregisterListener(EventList.GamePlay.UPGRADE_CHARACTER, UpdateCurrentCharacterUI);
        }

        private void OnSelectCharacter()
        {
            PlayerData.playerCharacterName = CurrentCharacterConfig.characterName;
            this.RaiseEvent(EventList.GamePlay.SET_NEW_CHARACTER, new CharacterEventData()
            {
                characterName = PlayerData.playerCharacterName
            });
            EventController.RaiseEvent(EventList.GamePlay.SET_NEW_CHARACTER);
            //UpdateCurrentCharacterUI();
        }
        
        private void OnDeselectCharacter()
        {
            PlayerData.playerCharacterName = Config.allCharactersConfig.GetDefaultCharacterConfig().characterName;
            this.RaiseEvent(EventList.GamePlay.SET_NEW_CHARACTER, new CharacterEventData()
            {
                characterName = PlayerData.playerCharacterName
            });
            //UpdateCurrentCharacterUI();
        }

        private void OnLevelUpCharacter()
        {
            if (!PlayerData.CheckEnoughShardToLevelUp(CurrentCharacterConfig.characterName))
            {
                Locator.Main.PopupInsufficientCharacterShard.Show();
                return;
            }
            
            PlayerData.CheckLevelUpCharacter(CurrentCharacterConfig.characterName);

            CurrentCharacterLevel = PlayerData.GetCharacterLevel(CurrentCharacterConfig.characterName);
            
            //selectionScroller.UpdateCharacterLevel(CurrentCharacterConfig.characterName, CurrentCharacterLevel);
            
            this.RaiseEvent(EventList.GamePlay.UPGRADE_CHARACTER, new CharacterEventData()
            {
                characterName = CurrentCharacterConfig.characterName
            });
            EventController.RaiseEvent(EventList.GamePlay.UPGRADE_CHARACTER);
            
            UpdateCurrentCharacterUI();

            if (PlayerData.CheckShowCharacterUnlockPopup(CurrentCharacterConfig.characterName))
            {
                Locator.Main.PopupUnlockCharacter.Setup(CurrentCharacterConfig);
                Locator.Main.PopupUnlockCharacter.Show();
            }
        }

        private void SetCurrentCharacter(object param)
        {
            var data = (CharacterEventData)param;

            if (data == null)
                return;

            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(data.characterName);
            //CurrentCharacterLevel = PlayerData.GetCharacterLevel(CurrentCharacterConfig.characterName);
            UpdateCurrentCharacterUI();
        }

        public void UpdateCurrentCharacterUI(object param = null)
        {
            characterNameText.text = CurrentCharacterConfig.characterName;
            CurrentCharacterLevel = PlayerData.GetCharacterLevel(CurrentCharacterConfig.characterName);
            if (mainCharacterImage != null)
            {
                mainCharacterImage.sprite = CurrentCharacterConfig.characterMainSprite;
                mainCharacterImage.SetNativeSize();
            }

            if (characterShardImage != null)
            {
                characterShardImage.sprite = CurrentCharacterConfig.characterShardSpriteSmall;
            }

            UpdateDescriptionText();
            UpdateGroupButtonsUI();
            UpdateUpgradeButtonUI();
            UpdateCurrentLevelStarUI();
            UpdateStatUI();
            UpdateDefaultCharacterUI();
            CheckNavigateButton();
        }

        private void UpdateUpgradeButtonUI()
        {
            upgradeBtnUpgradableGroup.SetActive(false);
            upgradeBtnInactiveGroup.SetActive(false);
            upgradeBtnMaxLevelGroup.SetActive(false);

            upgradeBtnUpgradableText.text = CurrentCharacterLevel >= 0 ? "Upgrade now!" : "Collect";
            upgradeBtnInactiveText.text = CurrentCharacterLevel >= 0 ? "Upgrade" : "Collect";

            //levelUpCharacterButton.interactable = false;
            
            var upgradeConfig = CurrentCharacterConfig.GetCharacterUpgrade(CurrentCharacterLevel + 1);

            var clickEffect = levelUpCharacterButton.GetComponent<ButtonClickEffect>();
            
            if(clickEffect != null)
                clickEffect.enabled = false;
            
            if (upgradeConfig == null)
            {
                //shardCountText.text = "Max";
                upgradeBtnMaxLevelGroup.SetActive(true);
                return;
            }
            
            var upgradeShardCost = upgradeConfig.shardCost;
            var currentShard = PlayerData.GetCharacterShards(CurrentCharacterConfig.characterName);
            
            shardCountText.text = $"{currentShard}/{upgradeShardCost}";
            inactiveShardCountText.text = $"{currentShard}/{upgradeShardCost}";
            
            if(upgradeShardCost == 0)
                shardCountText.text = "Free";

            if (currentShard < upgradeShardCost)
            {
                upgradeBtnInactiveGroup.SetActive(true);
                return;
            }
            
            //levelUpCharacterButton.interactable = true;
            
            if(clickEffect != null)
                clickEffect.enabled = true;

            upgradeBtnUpgradableGroup.SetActive(true);
        }

        private void UpdateCurrentLevelStarUI()
        {
            for (int i = listStarUI.Count - 1; i >= 0; i--)
            {
                listStarUI[i].SetStarActive(i <= CurrentCharacterLevel);
            }
        }

        private void UpdateStatUI()
        {
            foreach (var statUI in listCharacterStatUI)
            {
                statUI.Setup(CurrentCharacterConfig.GetStat(statUI.statType));
            }
        }

        private void UpdateGroupButtonsUI()
        {
            if (CurrentCharacterLevel >=0)
            {
                selectCharacterButton.gameObject.SetActive(!IsCurrentShowingSelected);
                deselectCharacterButton.gameObject.SetActive(IsCurrentShowingSelected);
            }
            else
            {
                selectCharacterButton.gameObject.SetActive(false);
                deselectCharacterButton.gameObject.SetActive(false);
            }
        }

        private void UpdateDefaultCharacterUI()
        {
            if (!CurrentCharacterConfig.isDefaultCharacter)
                return;
            
            deselectCharacterButton.gameObject.SetActive(false);
            selectCharacterButton.gameObject.SetActive(false);
        }

        public void Setup()
        {
            foreach(var character in Config.allCharactersConfig.listAllCharactersConfig)
            {
                PlayerData.CheckUnlockCharacter(character.characterName);
            }
            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(PlayerData.playerCharacterName);

            if (CurrentCharacterConfig.isDefaultCharacter) //Hides default character
                CurrentCharacterConfig = Config.allCharactersConfig.listAllCharactersConfig[1];
            
            SetCurrentCharacter(new CharacterEventData() {characterName = CurrentCharacterConfig.characterName });
            
            UpdateCurrentCharacterUI();
            selectionScroller.Init(this);
            CheckNavigateButton();
        }

        public string GetDescriptionText(CharacterConfig characterConfig)
        {
            var descriptionText = characterConfig.characterDescription;
            var charLevel = PlayerData.GetCharacterLevel(characterConfig.characterName);

            List<object> formatValues = new List<object>();

            foreach (var power in characterConfig.listCharacterPowers)
            {
                if (power != null)
                {
                    var value = power.GetCharacterPowerValue(charLevel);
                    float powerValue = value.buffAmount;

                    if (powerValue < 1f)
                        powerValue *= 100f;
                    
                    string stringValue = GetHighlightedText(powerValue.ToString());
                    if (!String.IsNullOrEmpty(value.overrideDescriptionString))
                    {
                        stringValue = GetHighlightedText(value.overrideDescriptionString);
                    }
                    formatValues.Add(stringValue);
                }
            }
            
            descriptionText = ReplaceIconTags(String.Format(descriptionText, formatValues.ToArray()));

            return descriptionText;
        }

        private void UpdateDescriptionText()
        {
            characterDescriptionText.text = GetDescriptionText(CurrentCharacterConfig);
            // characterDescriptionText.text = CurrentCharacterConfig.characterDescription;

            // List<object> formatValues = new List<object>();
            //
            // foreach (var power in CurrentCharacterConfig.listCharacterPowers)
            // {
            //     if (power != null)
            //     {
            //         var value = power.GetCharacterPowerValue(CurrentCharacterLevel);
            //         float powerValue = value.buffAmount;
            //
            //         if (powerValue < 1f)
            //             powerValue *= 100f;
            //         
            //         string stringValue = GetHighlightedText(powerValue.ToString());
            //         if (!String.IsNullOrEmpty(value.overrideDescriptionString))
            //         {
            //             stringValue = GetHighlightedText(value.overrideDescriptionString);
            //         }
            //         formatValues.Add(stringValue);
            //     }
            // }
            //
            // //characterDescriptionText.text = String.Format(characterDescriptionText.text, formatValues.ToArray());
            // characterDescriptionText.text = ReplaceIconTags(String.Format(characterDescriptionText.text, formatValues.ToArray()));
        }
        
        private string ReplaceIconTags(string inputText)
        {
            return Regex.Replace(inputText, @"\[(\w+)\]", match =>
            {
                string iconName = match.Groups[1].Value;
                return $"<size={characterDescriptionText.fontSize + highlightTextSizeIncrease}><sprite name={iconName}></size>";
            });
        }
        
        private string GetHighlightedText(string value)
        {
            string colorHex = ColorToHex(highlightTextColor);
            return $"<size={characterDescriptionText.fontSize + highlightTextSizeIncrease}><color={colorHex}>{value}</color></size>";
        }
        
        private string ColorToHex(Color color)
        {
            Color32 color32 = color; // Converts to 0–255 range
            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
        }

        private void OnNextButtonClick()
        {
            var listCharacter = Config.allCharactersConfig.GetListCharacterConfigNotDefault();
            var currentIndex = listCharacter.FindIndex(character =>
                character.characterName.Equals(CurrentCharacterConfig.characterName));
            var nextIndex = currentIndex + 1;
            
            this.RaiseEvent(EventList.GamePlay.SELECT_NEW_CHARACTER, new CharacterEventData()
            {
                characterName = listCharacter[nextIndex].characterName
            });
            
        }

        private void OnPreviousButtonClick()
        {
            var listCharacter = Config.allCharactersConfig.GetListCharacterConfigNotDefault();
            var currentIndex = listCharacter.FindIndex(character =>
                character.characterName.Equals(CurrentCharacterConfig.characterName));
            var previousIndex = currentIndex - 1;
            
            this.RaiseEvent(EventList.GamePlay.SELECT_NEW_CHARACTER, new CharacterEventData()
            {
                characterName = listCharacter[previousIndex].characterName
            });
        }

        private void CheckNavigateButton()
        {
            var listCharacter = Config.allCharactersConfig.GetListCharacterConfigNotDefault();
            var currentIndex = listCharacter.FindIndex(character =>
                character.characterName.Equals(CurrentCharacterConfig.characterName));
            
            if (currentIndex == 0)
            {
                previousButton.gameObject.SetActive(false);
            }
            else if (!previousButton.gameObject.activeInHierarchy)
            {
                previousButton.gameObject.SetActive(true);
            }

            if (currentIndex == listCharacter.Count - 1)
            {
                nextButton.gameObject.SetActive(false);
            }
            else if (!nextButton.gameObject.activeInHierarchy)
            {
                nextButton.gameObject.SetActive(true);
            }
        }
    }
}
