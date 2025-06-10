using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterShardTab : _BaseBehaviour
    {
        public List<CharacterShardPack> listCharacterShardPacks = new();
        
        [Header("Refresh")] 
        public Button refreshButton;
        public TextMeshProUGUI refreshGoldCostText;
        public TextMeshProUGUI countdownBetweenRefreshText;
        public TextMeshProUGUI timeToRefreshShopText;

        private void Start()
        {
            refreshButton.onClick.AddListener(OnRefresh);
        }

        public void Setup()
        {
            for (int i = 0; i < listCharacterShardPacks.Count; i++)
            {
                listCharacterShardPacks[i].Setup(Config.characterShopConfig.listShardSlotData[i]);
            }
            
            UpdateUI();
        }

        public void LoadData()
        {
            for (int i = 0; i < listCharacterShardPacks.Count; i++)
            {
                listCharacterShardPacks[i].LoadData(PlayerData.listCharacterPackData[i]);
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            refreshGoldCostText.text = Config.characterShopConfig.GetShopRefreshPrice(PlayerData.shardShopRefreshTime).ToString();
        }

        private void UpdateRefreshTimerText()
        {
            
        }

        private void OnRefresh()
        {
            var goldCost = Config.characterShopConfig.GetShopRefreshPrice(PlayerData.shardShopRefreshTime);
            
            if (goldCost > PlayerData.GetPlayerCurrencyCount(ResourceType.Gold))
            {
                if (!Locator.Main.PopupGoldOffer.IsShow)
                    Locator.Main.PopupGoldOffer.Show();
            }
            else
            {
                PlayerData.OnRefreshShardShop();
                PlayerData.SubtractCurrency(ResourceType.Gold, goldCost);
                Setup();
                UpdateUI();
            }
        }
    }
}
