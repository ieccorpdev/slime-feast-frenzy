using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSettingInHome : PopupSettingIngame
    {
        public Image mainBg;
        public Button giftCode;
        private int countGiftCodeCheck;
        public SettingButton soundSettingButton;
        private Vector2 baseSettingSize;
        private int countShowGiftCode = 6;
        protected override void Awake()
        {
            base.Awake();
            baseSettingSize = mainBg.rectTransform.sizeDelta;
        }
        protected override void Start()
        {
            base.Start();
            SetEvent(giftCode,OnGiftCodeClick);
            soundSettingButton.actionClickSetting += OnSetSound;
        }

        public void OnGiftCodeClick()
        {
            Locator.Main.PopupGiftCode.Show();
        }

        public void OnSetSound()
        {
            countGiftCodeCheck++;
            if (countGiftCodeCheck >= countShowGiftCode)
            {
                // each time on off count 1
                ShowGiftCodeButton(true);
            }
        }

        public void ShowGiftCodeButton(bool stt)
        {
            if (baseSettingSize == Vector2.zero)
            {
                baseSettingSize = mainBg.rectTransform.sizeDelta;
            }
            Vector2 sizeD = stt ? new Vector2(baseSettingSize.x, baseSettingSize.y + 250) : baseSettingSize;
            giftCode.gameObject.SetActive(stt);
            mainBg.rectTransform.sizeDelta = sizeD;
        }
        public override void Show()
        {
            countGiftCodeCheck = 0;
            ShowGiftCodeButton(false);
            base.Show();
        }
        public override void Hide()
        {
            countGiftCodeCheck = 0;
            base.Hide();
        }
    }
}
