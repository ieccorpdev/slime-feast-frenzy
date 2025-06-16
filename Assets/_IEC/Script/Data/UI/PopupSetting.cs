using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSetting : BasePopup
    {
        [SerializeField] private Button m_BtnPrivacy, m_BtnRestore, m_BtnFeedback;
        public List<SettingButton> settingButtons = new List<SettingButton>();
        private void Start()
        {
            SetEvent(m_BtnPrivacy, () =>
            {
//                Application.OpenURL(Services.config.privacyPolicy);
            });
            
            SetEvent(m_BtnRestore, () =>
            {
                EventController.RaiseEvent(EventList.Service.RESTORE_PURCHASE);
            });
            
            SetEvent(m_BtnFeedback, () =>
            {
            });
        }

        public override void Show()
        {
            base.Show();
            
            foreach (var btn in settingButtons)
            {
                btn.UpdateUI();
            }
        }
    }
}