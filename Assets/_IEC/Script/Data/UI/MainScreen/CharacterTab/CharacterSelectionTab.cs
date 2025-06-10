using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public Button levelUpCharacterButton;
        public CharacterSelectionUIScroller selectionScroller;
        public TextMeshProUGUI characterNameText;

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
            levelUpCharacterButton.onClick.AddListener(OnLevelUpCharacter);
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

        private void OnLevelUpCharacter()
        {
            PlayerData.CheckLevelUpCharacter(CurrentCharacterConfig.characterName);
            CurrentCharacterLevel = PlayerData.GetCharacterLevel(CurrentCharacterConfig.characterName);
            
            selectionScroller.UpdateCharacterLevel(CurrentCharacterConfig.characterName, CurrentCharacterLevel);
            
            UpdateCurrentCharacterUI();
        }

        private void SetCurrentCharacter(object param)
        {
            var data = (SelectCharacterData)param;

            if (data == null)
                return;

            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(data.characterName);
            CurrentCharacterLevel = PlayerData.GetCharacterLevel(CurrentCharacterConfig.characterName);
            UpdateCurrentCharacterUI();
        }

        public void UpdateCurrentCharacterUI()
        {
            characterNameText.text = CurrentCharacterConfig.characterName;
            if (mainCharacterImage != null)
            {
                mainCharacterImage.sprite = CurrentCharacterConfig.characterMainSprite;
                mainCharacterImage.SetNativeSize();
            }

            UpdateGroupButtonsUI();
            UpdateUpgradeButtonUI();
            UpdateCurrentLevelStarUI();
            UpdateStatUI();
        }

        private void UpdateUpgradeButtonUI()
        {
            upgradeBtnUpgradableGroup.SetActive(false);
            upgradeBtnInactiveGroup.SetActive(false);
            upgradeBtnMaxLevelGroup.SetActive(false);

            upgradeBtnUpgradableText.text = CurrentCharacterLevel >= 0 ? "Upgrade now!" : "Collect";
            upgradeBtnInactiveText.text = CurrentCharacterLevel >= 0 ? "Upgrade" : "Collect";

            levelUpCharacterButton.interactable = false;
            
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

            if (currentShard < upgradeShardCost)
            {
                upgradeBtnInactiveGroup.SetActive(true);
                return;
            }
            
            levelUpCharacterButton.interactable = true;
            
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
                characterInUseButton.gameObject.SetActive(IsCurrentShowingSelected);
            }
            else
            {
                selectCharacterButton.gameObject.SetActive(false);
                characterInUseButton.gameObject.SetActive(false);
            }
        }

        public void Setup()
        {
            foreach(var character in Config.allCharactersConfig.listAllCharactersConfig)
            {
                PlayerData.CheckUnlockCharacter(character.characterName);
            }
            CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(PlayerData.playerCharacterName);
            UpdateCurrentCharacterUI();
            selectionScroller.Init(this);
        }
    }
}
