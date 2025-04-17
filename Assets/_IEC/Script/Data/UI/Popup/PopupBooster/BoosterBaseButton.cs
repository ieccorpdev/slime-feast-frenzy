using System;
using System.Collections;
using System.Collections.Generic;
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

        public TextMeshProUGUI unlimitedTimeText;
        
        public BoosterBaseState State { get; private set; }
        
        public BoosterData BoosterData { get; private set; }

        private void Start()
        {
            boosterButton.onClick.AddListener(OnBoosterButtonClick);
        }

        private void Update()
        {
            UpdateUnlimitedTimer();
        }

        public void OnBoosterButtonClick()
        {
            if (State == BoosterBaseState.Using)
            {
                CheckCurrentState();
            }
            else if (State == BoosterBaseState.Usable || State == BoosterBaseState.Unlimited)
            {
                State = BoosterBaseState.Using;
            }
            
            CheckShowStateObj();
            bgImage.sprite = Config.AllBoosterConfig.GetBgSprite(State);
        }

        public void Setup()
        {
            BoosterData = Config.AllBoosterConfig.GetBoosterData(boosterType);
            CheckCurrentState();
            CheckShowStateObj();
            bgImage.sprite = Config.AllBoosterConfig.GetBgSprite(State);
        }

        private void CheckCurrentState()
        {
            if (PlayerData.level < BoosterData.levelUnlock)
            {
                State = BoosterBaseState.Lock;
            }
            else if (PlayerData.GetPlayerCurrencyCount(boosterType) > 0)
            {
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
