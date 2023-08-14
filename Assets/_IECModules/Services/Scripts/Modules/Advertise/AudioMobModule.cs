#if (PluginAudioMob)
using System;
using AudioMob;

namespace IEC
{
    public class AudioMobModule
    {
        bool m_IsInit = false;
        bool m_AdsAvailable;

        public void Init(Action<bool> afterInitCallback)
        {

            if (AudioMobPlugin.Instance == null)
            {
                afterInitCallback?.Invoke(false);
                return;
            }

            AudioMobPlugin.Instance.GetAdAvailability((response) =>
            {
                m_AdsAvailable = response.AdsAvailable; // ALWAYS RETURN FALSE?
            });

            m_IsInit = true;
            afterInitCallback?.Invoke(true);
        }

        public void Hide()
        {
            AudioMobPlugin.Instance.StopAd();
        }

        public void Show(AdsEnum adsType, Action<IAudioAd> startCallback, Action<AdPlaybackResult> closeCallback)
        {
            if (!m_IsInit)
            {
                return;
            }

            switch (adsType)
            {
                case AdsEnum.AUDIO:
                    ShowAudio(startCallback, closeCallback);
                    break;
                default:
                    break;
            }
        }

        private void ShowAudio(Action<IAudioAd> startCallback, Action<AdPlaybackResult> closeCallback)
        {
            AudioMobPlugin.Instance.OnAdPlaybackStarted = startCallback;
            AudioMobPlugin.Instance.OnAdPlaybackCompleted = closeCallback;
            AudioMobPlugin.Instance.RequestAndPlayAd();
        }
    }
}
#endif