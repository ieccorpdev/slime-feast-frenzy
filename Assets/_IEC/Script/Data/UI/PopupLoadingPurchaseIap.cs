using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace IEC
{
    public enum PopupLoadingPurchaseIapMode
    {
        Purchase,
        GiftCode
    }
    
    public class PopupLoadingPurchaseIap : BasePopup
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI contentText;

        public void Show(PopupLoadingPurchaseIapMode mode = PopupLoadingPurchaseIapMode.Purchase)
        {
            SetPopupMode(mode);
            base.Show();
        }

        private void SetPopupMode(PopupLoadingPurchaseIapMode mode = PopupLoadingPurchaseIapMode.Purchase)
        {
            switch (mode)
            {
                case PopupLoadingPurchaseIapMode.Purchase:
                    contentText.text = "Loading purchase...";
                    break;
                case PopupLoadingPurchaseIapMode.GiftCode:
                    contentText.text = "Loading gift code...";
                    break;
            }
        }
    }
}