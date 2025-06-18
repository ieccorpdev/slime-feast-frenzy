using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
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
        public Button getCharacterButton;

        public CharacterRecommendData CharacterRecommendData { get; private set; }
        public CharacterRecommendState CharacterRecommendState { get; private set; }
        public bool IsTrial { get; private set; }

        private void Start()
        {
            selectButton.onClick.AddListener(OnSelect);
            deselectButton.onClick.AddListener(OnDeselect);
            unlockButton.onClick.AddListener(OnUnlock);
            getCharacterButton.onClick.AddListener(OnGetCharacter);
        }

        public bool CheckSetupWinScreen()
        {
            if (String.IsNullOrEmpty(PlayerData.tempCharacterName))
                return false;
            
            if (PlayerData.GetCharacterLevel(PlayerData.tempCharacterName) >= 0)
                return false;

            characterNameText.text = PlayerData.tempCharacterName;
            bgImage.sprite = Config.allCharactersConfig.GetCharacterConfig(PlayerData.tempCharacterName)
                .characterRecommendBarSprite;

            CharacterRecommendData = Config.characterRecommendConfig.GetCharacterData(PlayerData.tempCharacterName);
            
            UpdateUI(true);
            
            return true;
        }

        public bool CheckSetup()
        {
            CharacterRecommendData = Config.characterRecommendConfig.GetForcedTrialCharacter(PlayerData.level);
            if (CharacterRecommendData != null)
            {
                //IsForcedTrial = true;
                PlayerData.SetTempCharacter(CharacterRecommendData.characterName);
                return false;
            }

            CharacterRecommendData = Config.characterRecommendConfig.GetRecommendedCharacter(PlayerData.level);
            if (CharacterRecommendData == null)
                return false;
            
            IsTrial = CharacterRecommendData.isTrial;

            characterNameText.text = CharacterRecommendData.characterName;
            bgImage.sprite = Config.allCharactersConfig.GetCharacterConfig(CharacterRecommendData.characterName)
                .characterRecommendBarSprite;
            
            UpdateUI();

            return true;
        }

        private void UpdateUI(bool isLevelWin = false)
        {
            freeTrialGroup.SetActive(false);
            deselectButton.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(false);
            getCharacterButton.gameObject.SetActive(false);

            if (isLevelWin)
            {
                getCharacterButton.gameObject.SetActive(true);
                return;
            }

            var currentCharName = PlayerData.GetCurrentCharacterName();

            if (IsTrial)
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
            UpdateUI();
        }

        public void OnDeselect()
        {
            PlayerData.ResetTempCharacter();
            UpdateUI();
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
                UpdateUI();
            }
        }

        public void OnGetCharacter()
        {
            if (Locator.Game.ScreenLevelWin.gameObject.activeInHierarchy)
            {
                Locator.Game.ScreenLevelWin.AddActionAfterShowAllPopup(
                    null,
                    () =>
                    {
                        var tab = Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Character);
                        tab.onClick?.Invoke();
                        this.RaiseEvent(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, CharacterTabType.Character);
                        TweenControl.GetInstance().DelayCall(tab.gameObject.transform, 0.75f, () =>
                        {
                            this.RaiseEvent(EventList.GamePlay.SELECT_NEW_CHARACTER, new CharacterEventData()
                            {
                                characterName = CharacterRecommendData.characterName
                            });
                        });
                    }, 
                    null);
                Locator.Game.ScreenLevelWin.btnNext.onClick.Invoke();
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
