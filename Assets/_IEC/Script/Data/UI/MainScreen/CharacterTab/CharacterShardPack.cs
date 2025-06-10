using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterShardPack : _BaseBehaviour
    {
        public Image characterMainImage;
        public TextMeshProUGUI shardAmountText;

        [Header("Buy Group")] 
        public Button buyButton;
        public TextMeshProUGUI goldText;

        [Header("Bought Group")] 
        public GameObject boughtGroup;

        [Header("Lock Group")] 
        public GameObject lockGroup;
        public TextMeshProUGUI lockLevelText;

        public CharacterShardPackData PackData { get; private set; }

        private void Start()
        {
            buyButton.onClick.AddListener(OnBuyPack);
        }

        private void OnBuyPack()
        {
            if (PlayerData.GetPlayerCurrencyCount(ResourceType.Gold) >= PackData.slotData.goldCost)
            {
                PlayerData.SubtractCurrency(ResourceType.Gold, PackData.slotData.goldCost);
                PackData.isBought = true;
                PlayerData.AddCharacterShards(PackData.characterName, PackData.slotData.shardCount);
                UpdateUI();
            }
            else
            {
                if (!Locator.Main.PopupGoldOffer.IsShow)
                    Locator.Main.PopupGoldOffer.Show();
            }
        }
        
        public void Setup(CharacterShardSlotData slotData)
        {
            var character = Config.characterShopConfig.GetRandomCharacterShard(slotData);
            PlayerData.SaveCharacterShardPackData(new CharacterShardPackData()
            {
                slotData = slotData,
                characterName = character.characterName,
                isBought = false,
            });
            PackData = PlayerData.GetPackData(slotData);
            UpdateUI();
        }

        public void LoadData(CharacterShardPackData packData)
        {
            PackData = packData;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var playerConfig = Config.allCharactersConfig.GetCharacterConfig(PackData.characterName);
            characterMainImage.sprite = playerConfig.characterSelectionSprite;
            shardAmountText.text = $"x{PackData.slotData.shardCount}";
            goldText.text = $"{PackData.slotData.goldCost}";
            lockLevelText.text = $"Lv.{PackData.slotData.levelUnlockSlot}";
            
            characterMainImage.gameObject.SetActive(false);
            shardAmountText.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            boughtGroup.SetActive(false);
            lockGroup.SetActive(false);

            if (PlayerData.level < PackData.slotData.levelUnlockSlot)
            {
                lockGroup.SetActive(true);
            }
            else
            {
                characterMainImage.gameObject.SetActive(true);
                shardAmountText.gameObject.SetActive(true);
                if (PackData.isBought)
                {
                    boughtGroup.SetActive(true);
                }
                else
                {
                    buyButton.gameObject.SetActive(true);
                }
            }
        }
        
    }

    [System.Serializable]
    public class CharacterShardPackData
    {
        public CharacterShardSlotData slotData;
        public string characterName;
        public bool isBought;
        
        
    }
}
