using UnityEngine;
using System;
using System.Collections;

namespace IEC
{
    public class ConsentMock : IConsentService
    {
        const string IS_ACCEPT_CONSENT = "IS_ACCEPT_CONSENT";

        public void CheckConsent(Action<bool> onConsentState)
        {
            if (IsHaveConsent())
            {
                onConsentState?.Invoke(true);
                return;
            }

            onConsentState?.Invoke(false);
        }

        public bool IsHaveConsent()
        {
            return PlayerPrefs.HasKey(IS_ACCEPT_CONSENT);
        }

        public void SetAcceptConsent()
        {
            PlayerPrefs.SetInt(IS_ACCEPT_CONSENT, 1);
            PlayerPrefs.Save();
        }
    }
}