using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Helper;
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
        public GameObject btnRefreshGroup;
        public GameObject btnCountdownGroup;
        public TextMeshProUGUI refreshGoldCostText;
        public TextMeshProUGUI countdownBetweenRefreshText;
        public TextMeshProUGUI timeToRefreshShopText;

        private float _countdownBetweenRefresh = 0f;

        private DateTime timeEndDay;

        private void Start()
        {
            refreshButton.onClick.AddListener(OnRefresh);
        }

        public void CheckSetupDataShop()
        {
            if (PlayerData.CheckRefreshCharacterShardShop())
            {
                Setup();
            }
            else
            {
                LoadData();
            }
        }

        public void Setup()
        {
            for (int i = 0; i < listCharacterShardPacks.Count; i++)
            {
                listCharacterShardPacks[i].Setup(Config.characterShopConfig.listShardSlotData[i]);
            }

            _countdownBetweenRefresh = 0f;
            
            UpdateUI();
            timeEndDay = DateTime.Now.Date.AddDays(1);
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
            
            btnRefreshGroup.SetActive(_countdownBetweenRefresh <= 0f);
            btnCountdownGroup.SetActive(_countdownBetweenRefresh > 0f);

            refreshButton.interactable = btnRefreshGroup.activeSelf;
        }

        private void Update()
        {
            UpdateRefreshTimerText();
        }

        private void UpdateRefreshTimerText()
        {
            timeToRefreshShopText.text = $"Shop reset in {DateTimeHelper.GetFormattedTimeHMS(timeEndDay - DateTime.Now)}";

            if (_countdownBetweenRefresh > 0f)
            {
                int countdown = Mathf.FloorToInt(_countdownBetweenRefresh);
                string cnt = countdown >= 10 ? $"{countdown}" : "0" + countdown;
                countdownBetweenRefreshText.text = $"00:{cnt}";

                _countdownBetweenRefresh -= Time.deltaTime;
                if (_countdownBetweenRefresh <= 0f)
                {
                    UpdateUI();
                }
            }
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
                _countdownBetweenRefresh = Config.characterShopConfig.countdownBetweenRefresh;
                Setup();
                UpdateUI();
            }
        }
    }
}
