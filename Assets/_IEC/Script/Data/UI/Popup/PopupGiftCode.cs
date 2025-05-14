using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupGiftCode : BasePopup
    {
        public Button confirmButton;
        public TMP_InputField giftCodeField;

        public void Start()
        {
           SetEvent(confirmButton,ConfirmUseGiftCode);
        }

        public override void Show()
        {
            giftCodeField.text = "";
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
        }

        public void ConfirmUseGiftCode()
        {
            string giftCode = giftCodeField.text;
            Debug.LogError(giftCode);
            if (!string.IsNullOrEmpty(giftCode))
            {
                EventController.RaiseEvent(EventList.Setting.REQUEST_GIFTCODE,new EventParam(){p0 = giftCode});
            }
        }

        public void ReceiveGiftCode(List<Reward> rewards)
        {
            Hide();
            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewards,null); 
            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
        }
    }
}
