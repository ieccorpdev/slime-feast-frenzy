using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterUICard : _BaseBehaviour
    {
        public Image mainImage;

        [Header("Button Group")] 
        public GameObject inUseButton;
        public Button selectButton;
        public Button upgradeButton;

        [Header("Selected Group")] 
        public GameObject selectedFrame;
        public GameObject selectedTickImg;

        [Header("Upgradable Group")] 
        public GameObject upgradableFrame;
        public GameObject upgradableImg;
        public TextMeshProUGUI upgradeShardCountText;
        public Color upgradableTextColor;
        public Color notEnoughShardTextColor;

        [Header("Lock Group")] 
        public GameObject lockImg;

        [Header("Star Group")] 
        public CharacterTabStarGroup starGroup;
        public Sprite selectedStarBgSprite;
        public Sprite normalStarBgSprite;

        public CharacterConfig CharacterConfig { get; private set; }
        
        public int CharacterLevel { get; private set; }

        private void Start()
        {
            selectButton.onClick.AddListener(OnSelect);
            upgradeButton.onClick.AddListener(OnUpgrade);
        }

        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.SET_NEW_CHARACTER,CheckUpdateUIOnSelectNewCharacter);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.SET_NEW_CHARACTER,CheckUpdateUIOnSelectNewCharacter);
        }

        private void OnSelect()
        {
            PlayerData.playerCharacterName = CharacterConfig.characterName;
            this.RaiseEvent(EventList.GamePlay.SET_NEW_CHARACTER, new CharacterEventData()
            {
                characterName = PlayerData.playerCharacterName
            });
            UpdateUI();
        }

        private void OnUpgrade()
        {
            if (!PlayerData.CheckEnoughShardToLevelUp(CharacterConfig.characterName))
            {
                Locator.Main.PopupInsufficientCharacterShard.Show();
                return;
            }
            PlayerData.CheckLevelUpCharacter(CharacterConfig.characterName);
            CharacterLevel = PlayerData.GetCharacterLevel(CharacterConfig.characterName);
            
            this.RaiseEvent(EventList.GamePlay.UPGRADE_CHARACTER, new CharacterEventData()
            {
                characterName = CharacterConfig.characterName
            });
            
            UpdateUI();
        }

        private void CheckUpdateUIOnSelectNewCharacter(object param)
        {
            UpdateUI();
        }

        public void Setup(CharacterConfig characterConfig)
        {
            CharacterConfig = characterConfig;
            CharacterLevel = PlayerData.GetCharacterLevel(CharacterConfig.characterName);
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            mainImage.sprite = CharacterConfig.characterSelectionSprite;
            UpdateSelectedUI();
            UpdateUpgradableUI();
            UpdateLockUI();
            UpdateStarUI();
        }

        private void UpdateSelectedUI()
        {
            bool selected = CharacterConfig != null &&
                            CharacterConfig.characterName
                                .Equals(PlayerData.playerCharacterName);
            
            selectedFrame.SetActive(selected);
            selectedTickImg.SetActive(selected);
            inUseButton.SetActive(selected);
            
            selectButton.gameObject.SetActive(!selected);
        }

        private void UpdateUpgradableUI()
        {
            var upgradeConfig = CharacterConfig.GetCharacterUpgrade(CharacterLevel + 1);

            if (upgradeConfig == null)
            {
                upgradableFrame.SetActive(false);
                upgradableImg.SetActive(false);
                upgradeButton.gameObject.SetActive(false);
                return;
            }

            var upgradeShardCost = upgradeConfig.shardCost;
            var currentShard = PlayerData.GetCharacterShards(CharacterConfig.characterName);
            bool upgradable = currentShard >= upgradeShardCost;
            
            upgradableFrame.SetActive(upgradable);
            upgradableImg.SetActive(upgradable);
            upgradeButton.gameObject.SetActive(upgradable);

            upgradeShardCountText.text = upgradeShardCost.ToString();
            upgradeShardCountText.color = upgradable ? upgradableTextColor : notEnoughShardTextColor;

            //upgradeButton.interactable = upgradable;

            if (upgradable)
            {
                selectedTickImg.SetActive(false);
                selectButton.gameObject.SetActive(false);
            }
        }

        private void UpdateLockUI()
        {
            bool locked = CharacterLevel == -1;
            
            lockImg.SetActive(locked);

            if (locked)
            {
                mainImage.sprite = CharacterConfig.characterLockedSprite;
                
                selectedTickImg.SetActive(false);
                upgradableImg.SetActive(false);
                upgradableFrame.SetActive(false);
                
                selectButton.gameObject.SetActive(false);
                upgradeButton.gameObject.SetActive(true);
            }
        }

        private void UpdateStarUI()
        {
            bool selected = CharacterConfig != null &&
                            CharacterConfig.characterName
                                .Equals(PlayerData.playerCharacterName);
            
            starGroup.UpdateStarUI(CharacterLevel);

            foreach (var star in starGroup.listStarUI.Select(ui => ui.GetComponent<Image>()))
            {
                star.sprite = selected ? selectedStarBgSprite : normalStarBgSprite;
            }
        }
        
    }
}
