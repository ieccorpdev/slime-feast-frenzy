using UnityEngine;
using System;
using System.Collections;

namespace IEC
{
    public class ApplovinConsentModule : IConsentService
    {
        const string IS_ACCEPT_CONSENT = "IS_ACCEPT_CONSENT";

        private ApplovinModule m_ApplovinModule;
        private AdsConfig m_ApplovinConfig;

        public void Init(AdsConfig adsConfig)
        {
            m_ApplovinConfig = adsConfig;
        }

        public void CheckConsent(Action<bool> onConsentState)
        {
            if (IsHaveConsent())
            {
                onConsentState?.Invoke(true);
                return;
            }

            var adsConfig = m_ApplovinConfig;
            var config = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;
            config.appID = config.appID.Trim();

            if (string.IsNullOrEmpty(config.appID.Trim()) == false)
            {
                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
                {
                    if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies
                            || sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Unknown)
                    {
                        onConsentState?.Invoke(false);
                    }
                    else
                    {
                        SetAcceptConsent();
                        onConsentState?.Invoke(true);
                    }
                };
            }
            else
            {
                Debug.LogError("Applovin ID null, cannot init SDK for Consent");
                onConsentState?.Invoke(false);
            }
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