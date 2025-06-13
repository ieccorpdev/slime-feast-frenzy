using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterSelectionUI : EnhancedScrollerCellView
    {
        public Image characterAvatar;
        public Image selectedFrame;
        [Header("Lock Group")]
        public GameObject lockGroup;
        public GameObject shardUnlockGroup;
        public TextMeshProUGUI shardUnlockCountText;
        
        [Space(10)]
        public Vector2 selectedAvatarMaskSize = new Vector2(205, 205);
        public Image normalFrame;
        public Image lockFrame;
        public Vector2 normalAvatarMaskSize = new Vector2(175, 175);
        public RectTransform avatarGroup;
        public CharacterTabStarGroup starGroup;
        public Button btn;
        public CharacterConfig CharacterConfig { get; private set; }
        
        public int CurrentCharacterLevel { get; private set; }

        private PlayerSecret m_playerData => Pool.Get<PlayerSecret>();

        private void Start()
        {
            if(btn != null)
                btn.onClick.AddListener(OnSelectCharacter);
            
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, CheckCurrentCharacter);
            this.RegisterListener(EventList.GamePlay.UPGRADE_CHARACTER, CheckUpdateCurrentCharacter);
        }

        public void Setup(CharacterConfig characterConfig, string currentCharacterName)
        {
            CharacterConfig = characterConfig;
            // if(characterAvatar != null)
            //     characterAvatar.sprite = CharacterConfig.characterSelectionSprite;

            UpdateCharacterLevel(m_playerData.GetCharacterLevel(CharacterConfig.characterName));
            SetSelectedUI(CharacterConfig.characterName.Equals(currentCharacterName));
            CheckSetLockUI();
        }

        public void UpdateCharacterLevel(int level)
        {
            CurrentCharacterLevel = level;
            UpdateCurrentLevelStarUI();
        }

        public void OnSelectCharacter()
        {
            this.RaiseEvent(EventList.GamePlay.SELECT_NEW_CHARACTER, new CharacterEventData()
            {
                characterName = CharacterConfig.characterName
            });
        }

        private void CheckUpdateCurrentCharacter(object param = null)
        {
            var data = (CharacterEventData)param;

            if (data != null && CharacterConfig.characterName.Equals(data.characterName))
            {
                UpdateCharacterLevel(m_playerData.GetCharacterLevel(CharacterConfig.characterName));
                CheckSetLockUI();
            }
        }

        private void CheckCurrentCharacter(object param = null)
        {
            var data = (CharacterEventData)param;

            if (data != null)
            {
                // if (CharacterConfig.characterName.Equals(data.characterName))
                // {
                //
                // }
                
                SetSelectedUI(CharacterConfig.characterName.Equals(data.characterName));
            }
        }
        
        private void UpdateCurrentLevelStarUI()
        {
            starGroup.UpdateStarUI(CurrentCharacterLevel);
        }

        private void SetSelectedUI(bool isSelected)
        {
            selectedFrame.gameObject.SetActive(isSelected);
            normalFrame.gameObject.SetActive(!isSelected);
            lockFrame.gameObject.SetActive(!isSelected);

            avatarGroup.sizeDelta =
                isSelected 
                    ? selectedAvatarMaskSize 
                    : normalAvatarMaskSize;
            
            if(!isSelected)
                CheckSetLockUI();
        }

        private void CheckSetLockUI()
        {
            var level = m_playerData.GetCharacterLevel(CharacterConfig.characterName);
            bool isLock = level == -1;
            characterAvatar.sprite = isLock ? CharacterConfig.characterLockedSprite : CharacterConfig.characterSelectionSprite;
            lockGroup.SetActive(isLock);
            //shardUnlockGroup.SetActive(isLock);
            //starGroup.gameObject.SetActive(!isLock);
            
            // var upgradeConfig = CharacterConfig.GetCharacterUpgrade(level + 1);
            // if (upgradeConfig != null)
            // {
            //     shardUnlockCountText.text = upgradeConfig.shardCost.ToString();
            // }
        }
    }

    [Serializable]
    public class CharacterEventData
    {
        public string characterName;
    }
}
