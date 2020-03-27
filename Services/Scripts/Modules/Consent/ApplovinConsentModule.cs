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
        private Action<bool> m_OnConsentState;

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

            m_OnConsentState = onConsentState;

            if (string.IsNullOrEmpty(config.appID.Trim()) == false)
            {
                MaxSdk.SetSdkKey(config.appID);
                MaxSdk.InitializeSdk();
                MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxSdkInitCallback;
            }
            else
            {
                Debug.LogError("Applovin ID null, cannot init SDK for Consent");
                onConsentState?.Invoke(false);
            }
        }

        void OnMaxSdkInitCallback(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnMaxSdkInitCallback;

            if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies
                    || sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Unknown)
            {
                m_OnConsentState?.Invoke(false);
            }
            else
            {
                SetAcceptConsent();
                m_OnConsentState?.Invoke(true);
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