#if (PluginAudioMob)
using System;
using System.Collections;
using System.Collections.Generic;
using AudioMob;
using UnityEngine;

namespace IEC
{
    public class AudioMobModule
    {
        bool m_IsInit = false;
        bool m_AdsAvailable;

        AdsStateData m_AudioAdsState = new AdsStateData();

        public void Init(Action<bool> afterInitCallback)
        {

            if (AudioMobPlugin.Instance == null)
            {
                afterInitCallback?.Invoke(false);
                return;
            }

            AudioMobPlugin.Instance.GetAdAvailability((response) =>
            {
                m_AdsAvailable = response.AdsAvailable;
            });

            m_IsInit = true;
            afterInitCallback?.Invoke(true);
        }
  
        public void Show(AdsEnum adsType, Action<AdsEventArgs> showCallback)
        {
            if (!m_IsInit)
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
                return;
            }

            switch (adsType)
            {
                case AdsEnum.AUDIO:
                    ShowAudio(showCallback);
                    break;
                default:
                    break;
            }
        }

        private void ShowAudio(Action<AdsEventArgs> showCallback)
        {
            var adsState = m_AudioAdsState;
            adsState.isWaitToShow = true;
           
            if (m_AdsAvailable)
            {
                adsState.onShow = showCallback;
                AudioMobPlugin.Instance.RequestAndPlayAd();
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }
    }
}

#endif