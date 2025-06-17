using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterRecommendBar : _BaseBehaviour
    {
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI shardCostText;
        public Image bgImage;
        public Button selectButton;
        public GameObject freeTrialGroup;
        public Button deselectButton;
        public Button unlockButton;
        
        public CharacterRecommendData CharacterRecommendData { get; private set; }
        public CharacterRecommendState CharacterRecommendState { get; private set; }
        public bool IsForcedTrial { get; private set; }

        private void Start()
        {
            selectButton.onClick.AddListener(OnSelect);
            deselectButton.onClick.AddListener(OnDeselect);
            unlockButton.onClick.AddListener(OnUnlock);
        }

        public bool CheckSetup()
        {
            CharacterRecommendData = Config.characterRecommendConfig.GetForcedTrialCharacter(PlayerData.level);
            if (CharacterRecommendData != null)
            {
                IsForcedTrial = true;
                PlayerData.SetTempCharacter(CharacterRecommendData.characterName);
            }
            else
            {
                IsForcedTrial = false;
                CharacterRecommendData = Config.characterRecommendConfig.GetRecommendedCharacter(PlayerData.level);
            }

            if (CharacterRecommendData == null)
                return false;

            characterNameText.text = CharacterRecommendData.characterName;
            bgImage.sprite = Config.allCharactersConfig.GetCharacterConfig(CharacterRecommendData.characterName)
                .characterRecommendBarSprite;
            
            UpdateUI();

            return true;
        }

        private void UpdateUI()
        {
            freeTrialGroup.SetActive(false);
            deselectButton.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(false);

            var currentCharName = PlayerData.GetCurrentCharacterName();

            if (IsForcedTrial)
            {
                freeTrialGroup.SetActive(true);
                selectButton.interactable = false;
                CharacterRecommendState = CharacterRecommendState.FreeTrial;
            }
            else
            {
                selectButton.interactable = true;
                if (currentCharName.Equals(CharacterRecommendData.characterName))
                {
                    deselectButton.gameObject.SetActive(true);
                    CharacterRecommendState = CharacterRecommendState.InUse;
                }
                else
                {
                    if (PlayerData.GetCharacterLevel(CharacterRecommendData.characterName) < 0)
                    {
                        unlockButton.gameObject.SetActive(true);
                        CharacterRecommendState = CharacterRecommendState.Unlock;

                        var config =
                            Config.allCharactersConfig.GetCharacterConfig(CharacterRecommendData.characterName);

                        var upgrade = config.GetCharacterUpgrade(0);
                        shardCostText.text = upgrade.shardCost.ToString();
                    }
                    else
                    {
                        selectButton.gameObject.SetActive(true);
                        CharacterRecommendState = CharacterRecommendState.Select;
                    }
                }
            }
        }

        // private void OnInteract()
        // {
        //     switch (CharacterRecommendState)
        //     {
        //         case CharacterRecommendState.FreeTrial:
        //             break;
        //         case CharacterRecommendState.InUse:
        //             PlayerData.ResetTempCharacter();
        //             break;
        //         case CharacterRecommendState.Unlock:
        //             if (!PlayerData.CheckEnoughShardToLevelUp(CharacterRecommendData.characterName))
        //             {
        //                 //Locator.Main.PopupInsufficientCharacterShard.Show();
        //                 //return;
        //             }
        //             else
        //             {
        //                 PlayerData.CheckLevelUpCharacter(CharacterRecommendData.characterName);
        //             }
        //
        //             break;
        //         case CharacterRecommendState.Select:
        //             PlayerData.SetTempCharacter(CharacterRecommendData.characterName);
        //             break;
        //     }
        //     
        //     UpdateUI();
        // }

        public void OnSelect()
        {
            PlayerData.SetTempCharacter(CharacterRecommendData.characterName);
        }

        public void OnDeselect()
        {
            PlayerData.ResetTempCharacter();
        }

        public void OnUnlock()
        {
            if (!PlayerData.CheckEnoughShardToLevelUp(CharacterRecommendData.characterName))
            {
                Locator.Main.PopupInsufficientCharacterShard.Show();
            }
            else
            {
                PlayerData.CheckLevelUpCharacter(CharacterRecommendData.characterName);
            }
        }
    }

    public enum CharacterRecommendState
    {
        FreeTrial,
        InUse,
        Unlock,
        Select
    }
}
