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

        public void Pause()
        {
            AudioMobPlugin.Instance.PauseAd();
        }

        public void Resume()
        {
            AudioMobPlugin.Instance.ResumePausedAd();
        }

        public void SetPauseOnStart(Action<IAudioAd> callback)
        {
            AudioMobPlugin.Instance.OnAdPlaybackStarted += callback;
        }

        public void Show(AdsEnum adsType, Action requestedCallback, Action<IAudioAd> startCallback, Action<AdPlaybackResult> closeCallback,
            Action<AdPauseReason> pauseCallback, Action resumeCallback, Action skippableCallback)
        {
            if (!m_IsInit)
            {
                return;
            }

            switch (adsType)
            {
                case AdsEnum.AUDIO:
                    ShowAudio(requestedCallback, startCallback, closeCallback, pauseCallback, resumeCallback, skippableCallback);
                    break;
                default:
                    break;
            }
        }

        private void ShowAudio(Action requestedCallback, Action<IAudioAd> startCallback, Action<AdPlaybackResult> closeCallback,
            Action<AdPauseReason> pauseCallback, Action resumeCallback, Action skippableCallback)
        {
            AudioMobPlugin.Instance.OnAdPlaybackStarted += startCallback;
            AudioMobPlugin.Instance.OnAdPlaybackCompleted += closeCallback;
            AudioMobPlugin.Instance.OnAdPaused += pauseCallback;
            AudioMobPlugin.Instance.OnAdResumed += resumeCallback;
            AudioMobPlugin.Instance.OnSkipButtonAvailable += skippableCallback;
            AudioMobPlugin.Instance.OnAdPlaybackCompleted += ResetEvents;
            AudioMobPlugin.Instance.RequestAndPlayAd();
            requestedCallback?.Invoke();
        }

        private void ResetEvents(AdPlaybackResult result)
        {
            AudioMobPlugin.Instance.OnAdPlaybackStarted = null;
            AudioMobPlugin.Instance.OnAdPlaybackCompleted = null;
            AudioMobPlugin.Instance.OnAdPaused = null;
            AudioMobPlugin.Instance.OnAdResumed = null;
            AudioMobPlugin.Instance.OnSkipButtonAvailable = null;
        }
    }
}
#endif