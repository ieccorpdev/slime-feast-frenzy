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
        public EventSystem eventSystem;

        public RectTransform bottomAds;

        public TMP_Text textHello, txtLevel;

        public MainUIGroup mainUIGroup;
        public GameOverGroup gameOverUIGroup;

        public PopupSetting popupSettings;
        public PopupRating popUpRating;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void OnInit()
        {
            mainUIGroup.OnInit();
            gameOverUIGroup.OnInit();

            popupSettings.OnInit();
            popUpRating.OnInit();

            OnResetAll();
        }

        public void OnResetAll()
        {
            gameOverUIGroup.gameObject.SetActive(false);
            popupSettings.gameObject.SetActive(false);
            popUpRating.gameObject.SetActive(false);

            gameOverUIGroup.SetTextGameOver();

            popupSettings.SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
        }

        public void OnGameOver(bool isActive)
        {
            gameOverUIGroup.gameObject.SetActive(isActive);
        }
    }

    public interface IUIGroup
    {
        void OnInit();
    }
}