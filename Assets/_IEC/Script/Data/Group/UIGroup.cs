using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System;

namespace IEC
{
    public class UIGroup : _BaseBehaviour, IGroup
    {
        public Canvas canvas;
        public CanvasScaler canvasScaler;

        public RectTransform bottomAds;

        public TMP_Text txtLevel, txtGold;

        public MainUIGroup mainUIGroup;
        public GameOverGroup gameOverUIGroup;
        public GameOverRewardGroup gameOverReward;

        public PopupSetting popupSettings;
        public PopupRating popupRating;
        public PopupShop popupShop;

        public GameObject loadingObj;
        [SerializeField]
        private GameObject m_PanelGoldObj;

        [SerializeField]
        private EffectCoinClaim m_EffectCoinClaim;

        [SerializeField]
        private Transform m_TransCoinIcon;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void OnInit()
        {
            mainUIGroup.OnInit();
            gameOverUIGroup.OnInit();

            popupSettings.OnInit();
            popupRating.OnInit();
            popupShop.OnInit(ShopConfig);
            gameOverReward.OnInit();

            OnResetAll();
        }

        public void OnResetAll()
        {
            gameOverUIGroup.gameObject.SetActive(false);
            popupSettings.gameObject.SetActive(false);
            popupRating.gameObject.SetActive(false);
            popupShop.gameObject.SetActive(false);

            popupSettings.SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            mainUIGroup.SetActiveBtnNoAds();
            mainUIGroup.OnEffectTime(false);
            mainUIGroup.SetActiveTopButtons(true);
            gameOverReward.gameObject.SetActive(false);
            popupShop.OnResetAll();

            OnUpdateShopData();

            m_PanelGoldObj.SetActive(false);
        }

        public void EffectCoinClaim(int moneyAdd, int endMoney, Action callback = null)
        {
            m_EffectCoinClaim.SpawnEffect(txtGold, m_TransCoinIcon.position, moneyAdd, endMoney, callback);
        }

        public void OpenShop()
        {
            OnUpdateShopData();
            m_PanelGoldObj.SetActive(true);
            popupShop.gameObject.SetActive(true);
        }

        public void OnGameOver(bool isActive, bool isNewGameOver, Action callback = null)
        {
            if (isNewGameOver)
            {
                gameOverReward.gameObject.SetActive(true);
                mainUIGroup.SetActiveTopButtons(false);
                m_PanelGoldObj.SetActive(true);
            }
            else
            {
                gameOverUIGroup.gameObject.SetActive(isActive);
                gameOverUIGroup.TweenGameOver(isActive, callback);
            }
        }

        public void OnUpdateShopData()
        {
            popupShop.OnUpdateData();
            UpdateGold();
        }

        public void UpdateGold()
        {
            txtGold.text = StringHelper.GetNumber(PlayerData.gold);
        }
    }

    public interface IUIGroup
    {
        void OnInit();
    }
}