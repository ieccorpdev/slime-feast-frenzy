using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterShardPack : _BaseBehaviour
    {
        public Image characterShardImage;
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
                PackData.isBought = true;

                var shardToMaxCount = Config.allCharactersConfig.GetCharacterConfig(PackData.characterName)
                    .GetShardCountToMaximum(PlayerData.GetCharacterLevel(PackData.characterName), PlayerData.GetCharacterShards(PackData.characterName));

                var shardCount = PackData.slotData.shardCount;
                int goldChange = 0;

                if (shardCount > shardToMaxCount)
                {
                    goldChange = (shardCount - shardToMaxCount) * (PackData.slotData.goldCost / shardCount);
                    Locator.Main.ScreenMain.AddRewardClaimEffect(ResourceType.Gold, goldChange);
                    Locator.Main.ScreenMain.DoAnimAllAvailableReward();
                    shardCount = shardToMaxCount;
                }
                PlayerData.SubtractCurrency(ResourceType.Gold, PackData.slotData.goldCost - goldChange);
                PlayerData.AddCharacterShards(PackData.characterName, shardCount);
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
            characterShardImage.sprite = playerConfig.characterShardSpriteNormal;
            shardAmountText.text = $"x{PackData.slotData.shardCount}";
            goldText.text = $"{PackData.slotData.goldCost}";
            lockLevelText.text = $"Lv.{PackData.slotData.levelUnlockSlot}";
            
            characterShardImage.gameObject.SetActive(false);
            shardAmountText.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            boughtGroup.SetActive(false);
            lockGroup.SetActive(false);

            if (PlayerData.level < PackData.slotData.levelUnlockSlot)
            {
                lockGroup.SetActive(true);
                GetComponent<Image>().enabled = true;
            }
            else
            {
                GetComponent<Image>().enabled = false;
                characterShardImage.gameObject.SetActive(true);
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
