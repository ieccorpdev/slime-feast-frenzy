using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupGiftCode : BasePopup
    {
        public Button confirmButton;
        public TMP_InputField giftCodeField;
        public TextMeshProUGUI notiText;
        public void Start() 
        {
           SetEvent(confirmButton,ConfirmUseGiftCode);
           gameObject.OnEvent<EventParam>(EventList.Setting.ERROR_GIFTCODE, param =>
           {
               string codeError = param.p0.ToString();
               OnErrorGifCode(codeError);
           });
        }

        public override void Show()
        {
            notiText.text = "";
            giftCodeField.text = "";
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
        }
        
        public void OnErrorGifCode(string err)
        {
            notiText.text = err;
            
            var color = Color.white;
            color.a = 1f;
            notiText.color = color;
            
            DOTween.Kill(notiText.GetInstanceID());

            Sequence seq = DOTween.Sequence()
                .SetId(notiText.GetInstanceID()) 
                .AppendInterval(2f)
                .Append(notiText.DOFade(0f, 1f))
                .OnComplete(() =>
                {
                    notiText.text = "";
                });
        }
        public void ConfirmUseGiftCode()
        {
            string giftCode = giftCodeField.text;
            Debug.LogError("Input" + giftCode);
            if (!string.IsNullOrEmpty(giftCode))
            {
                Locator.Service.PopupLoadingPurchaseIap.Show(PopupLoadingPurchaseIapMode.GiftCode);
                EventController.RaiseEvent(EventList.Setting.REQUEST_GIFTCODE,new EventParam(){p0 = giftCode});
            }
            else
            {
                OnErrorGifCode("Empty Gift Code");
            }
            giftCodeField.text = "";
        }

        public void ReceiveGiftCode(List<Reward> rewards)
        {
            Hide();
            if (Locator.Main.PopupSettingInHome.gameObject.activeInHierarchy)
            {
                Locator.Main.PopupSettingInHome.Hide();
            }
            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewards,null); 
            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
        }

        void OnDestroy()
        {
            DOTween.Kill(notiText.GetInstanceID());
        }
    }
}
