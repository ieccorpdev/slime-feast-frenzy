using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSetting : BasePopup
    {
        [SerializeField] private ToggleSetting m_ToggleSound, m_ToggleMusic, m_ToggleVibrate;
        [SerializeField] private Button m_BtnPrivacy, m_BtnRestore, m_BtnFeedback;

        private void Start()
        {
            SetEvent(m_ToggleSound.btn, () =>
            {
                PlayerData.isSound = !PlayerData.isSound;
                m_ToggleSound.SetStateUI(PlayerData.isSound);
            });
            
            SetEvent(m_ToggleMusic.btn, () =>
            {
                PlayerData.isMusic = !PlayerData.isMusic;
                m_ToggleMusic.SetStateUI(PlayerData.isMusic);
            });

            SetEvent(m_ToggleVibrate.btn, () =>
            {
                PlayerData.isVibrate = !PlayerData.isVibrate;
                m_ToggleVibrate.SetStateUI(PlayerData.isVibrate);
            });
            
            SetEvent(m_BtnPrivacy, () =>
            {
                Application.OpenURL(Services.config.privacyPolicy);
            });
            
            SetEvent(m_BtnRestore, () =>
            {
                EventController.RaiseEvent(EventList.Service.RESTORE_PURCHASE);
            });
            
            SetEvent(m_BtnFeedback, () =>
            {
                Services.SendEmail();
            });
        }

        public override void Show()
        {
            base.Show();
            m_ToggleSound.SetStateUI(PlayerData.isSound);
            m_ToggleMusic.SetStateUI(PlayerData.isMusic);
            m_ToggleVibrate.SetStateUI(PlayerData.isVibrate);
        }
    }
}