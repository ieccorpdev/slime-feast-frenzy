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

        public TMP_Text txtLevel;

        public MainUIGroup mainUIGroup;
        public GameOverGroup gameOverUIGroup;

        public PopupSetting popupSettings;
        public PopupRating popupRating;
        public PopupShop popupShop;

        public GameObject loadingObj;

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

            OnResetAll();
        }

        public void OnResetAll()
        {
            gameOverUIGroup.gameObject.SetActive(false);
            popupSettings.gameObject.SetActive(false);
            popupRating.gameObject.SetActive(false);
            popupShop.gameObject.SetActive(false);

            popupSettings.SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            mainUIGroup.OnEffectTime(false);

            popupShop.OnResetAll();

            OnUpdateShopData();
        }

        public void OnGameOver(bool isActive, Action callback = null)
        {
            gameOverUIGroup.gameObject.SetActive(isActive);
            gameOverUIGroup.TweenGameOver(isActive, callback);
        }

        public void OnUpdateShopData()
        {
            popupShop.OnUpdateData();
        }
    }

    public interface IUIGroup
    {
        void OnInit();
    }
}