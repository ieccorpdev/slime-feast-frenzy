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

        private DateTime timeResetShop;

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

            //_countdownBetweenRefresh = 0f;
            _countdownBetweenRefresh =
                (float)(PlayerData.timeEndCountdownRefreshShardShop - DateTime.Now).TotalSeconds;
            
            UpdateUI();
            timeResetShop = new DateTime(PlayerData.lastTimeUpdateCharacterShardShop).AddHours(Config.characterShopConfig.shopRefreshTimeInHours);
        }

        public void LoadData()
        {
            for (int i = 0; i < listCharacterShardPacks.Count; i++)
            {
                listCharacterShardPacks[i].LoadData(PlayerData.listCharacterPackData[i]);
            }
            
            _countdownBetweenRefresh =
                (float)(PlayerData.timeEndCountdownRefreshShardShop - DateTime.Now).TotalSeconds;

            UpdateUI();
            timeResetShop = new DateTime(PlayerData.lastTimeUpdateCharacterShardShop).AddHours(Config.characterShopConfig.shopRefreshTimeInHours);
        }

        private void UpdateUI()
        {
            refreshGoldCostText.text = Config.characterShopConfig.GetShopRefreshPrice(PlayerData.shardShopRefreshTime).ToString();

            bool isCountdown = _countdownBetweenRefresh > 0f &&
                               _countdownBetweenRefresh <= Config.characterShopConfig.countdownBetweenRefresh;
            
            btnRefreshGroup.SetActive(!isCountdown);
            btnCountdownGroup.SetActive(isCountdown);

            refreshButton.interactable = btnRefreshGroup.activeSelf;
        }

        private void Update()
        {
            UpdateRefreshTimerText();
        }

        private void UpdateRefreshTimerText()
        {
            var timeLeft = timeResetShop - DateTime.Now;
            if (timeLeft.TotalSeconds <= 0)
            {
                PlayerData.OnRefreshShardShop();
                Setup();
                UpdateUI();
            }
            else
            {
                timeToRefreshShopText.text = $"Shop reset in {DateTimeHelper.GetFormattedTimeHMS(timeLeft)}";
            }   

            if (_countdownBetweenRefresh >= 0f)
            {
                //_countdownBetweenRefresh = (float)(PlayerData.timeEndCountdownRefreshShardShop - DateTime.Now).TotalSeconds;
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
                PlayerData.timeEndCountdownRefreshShardShop = DateTime.Now.AddSeconds(Config.characterShopConfig.countdownBetweenRefresh);
                // _countdownBetweenRefresh =
                //     (float)(PlayerData.timeEndCountdownRefreshShardShop - DateTime.Now).TotalSeconds;
                Setup();
                UpdateUI();
            }
        }
    }
}
