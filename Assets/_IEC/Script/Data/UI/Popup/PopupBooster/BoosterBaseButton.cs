using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BoosterBaseButton : _BaseBehaviour
    {
        public ResourceType boosterType;
        public Button boosterButton;

        public Image bgImage;
        
        public GameObject lockObj, buyObj, usableObj, usingObj, unlimitedObj;

        public TextMeshProUGUI countText;
        public TextMeshProUGUI levelLockText;
        public TextMeshProUGUI unlimitedTimeText;
        
        public BoosterBaseState State { get; private set; }
        
        public BoosterData BoosterData { get; private set; }

        private void Start()
        {
            boosterButton.onClick.AddListener(OnBoosterButtonClick);
            gameObject.OnEvent(EventList.GamePlay.UPDATE_RESOURCE_UI, Setup);
        }

        private void Update()
        {
            UpdateUnlimitedTimer();
        }

        public void OnBoosterButtonClick()
        {
            if (State == BoosterBaseState.Using)
            {
                CheckCurrentState(true);
            }
            else if (State == BoosterBaseState.Usable)
            {
                State = BoosterBaseState.Using;
            }
            else if (State == BoosterBaseState.Buy)
            {
                Locator.Main.PopupAddHelper.SetData(boosterType);
                Locator.Main.PopupAddHelper.Show();
            }
            
            CheckShowStateObj();
            bgImage.sprite = Config.AllBoosterConfig.GetBgSprite(State);
        }

        public void Setup()
        {
            BoosterData = Config.AllBoosterConfig.GetBoosterData(boosterType);
            CheckCurrentState();
            CheckShowStateObj();

            countText.text = PlayerData.GetPlayerCurrencyCount(boosterType).ToString();
            bgImage.sprite = Config.AllBoosterConfig.GetBgSprite(State);
            levelLockText.text ="Lvl." + BoosterData.levelUnlock.ToString();
        }

        private void CheckCurrentState(bool isCheckOnClick = false)
        {
            if (PlayerData.level < BoosterData.levelUnlock)
            {
                State = BoosterBaseState.Lock;
            }
            else if (PlayerData.GetPlayerCurrencyCount(boosterType) > 0)
            {
                if(State != BoosterBaseState.Using || (State == BoosterBaseState.Using && isCheckOnClick))
                    State = BoosterBaseState.Usable;
            }
            else if (PlayerData.GetPlayerCurrencyCount(boosterType) == 0)
            {
                State = BoosterBaseState.Buy;
            }
            else if(PlayerData.IsUnlimitedBooster(boosterType))
            {
                State = BoosterBaseState.Unlimited;
            }
        }

        private void CheckShowStateObj()
        {
            lockObj.SetActive(State == BoosterBaseState.Lock);
            buyObj.SetActive(State == BoosterBaseState.Buy);
            usableObj.SetActive(State == BoosterBaseState.Usable);
            usingObj.SetActive(State == BoosterBaseState.Using);
            unlimitedObj.SetActive(State == BoosterBaseState.Unlimited);
        }

        public void UpdateUnlimitedTimer()
        {
            if (State != BoosterBaseState.Unlimited)
                return;

            unlimitedTimeText.text = PlayerData.GetTimeLeftUnlimitedBooster(boosterType);
        }
    }
    
    public enum BoosterBaseState
    {
        Lock,
        Buy,
        Usable,
        Using,
        Unlimited,
    }
}
