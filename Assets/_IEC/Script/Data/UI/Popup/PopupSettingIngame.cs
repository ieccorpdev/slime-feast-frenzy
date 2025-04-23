using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSettingIngame : BasePopup
    {
        public Button btnContinue;
        [FormerlySerializedAs("btnGoHome")] public Button btnQuit;
        public Button privacyPolicy;
        public Button restorePurchaseBtn;
        public Button feedBackBtn;
        public Button closeBtn2;
        public List<SettingButton> settingButtons = new();

        private void Start()
        {
            SetEvent(btnContinue, () =>
            {
                // EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS, new EventParam(this, name));
                // Services.PushEvent(new IEC.TrackingEvent.ClickBtnReplay());
                Hide();
            });
            if (privacyPolicy)
            {
            }
            if (restorePurchaseBtn)
            {
                SetEvent(restorePurchaseBtn, () =>
                {
                   
                });
            }

            if (feedBackBtn)
            {
                SetEvent(feedBackBtn, () =>
                {
                });
            }
            SetEvent(btnQuit, () =>
            {
                //EventController.RaiseEvent(EventList.GamePlay.GO_MAIN);
              

            });
            SetEvent(closeBtn2, () => { Hide(); });
            SetEvent(btnContinue, HideButtonFunction);
        }
        
        void HideButtonFunction()
        {
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            Hide();
        }
        public override void Show()
        {
            base.Show();
            foreach (var btn in settingButtons)
            {
                btn.UpdateUI();
            }
            this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
        }
        public override void Hide()
        {
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            base.Hide();
        }
    }
}