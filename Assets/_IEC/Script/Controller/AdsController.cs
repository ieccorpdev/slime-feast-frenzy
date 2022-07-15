using System.Collections;
using UnityEngine;

namespace IEC
{
    public class AdsController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.END_LEVEL_ADS:
                    Services.ShowFullAdsWithTime(Game.timeDelayFullAds, () =>
                    {
                        EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                    });
                    break;
                case EventList.Service.INIT_AD_DONE:
                    UpdateWhiteBannerSize();
                    break;

            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            if (Application.isEditor)
            {
                UpdateWhiteBannerSize();
            }
        }

        public void UpdateWhiteBannerSize()
        {
            Debug.LogWarning("UpdateWhiteBannerSize");
            var bannerHeight = (Application.isEditor ? 150 : Services.Ads().GetBannerHeight());

            Debug.LogWarning("screen DPI: " + Screen.dpi);
            if (Application.platform == RuntimePlatform.Android)
            {
                var nativeDPI = GetNativeDPI();
                Debug.LogWarning("native xdpi: " + nativeDPI.x);
                Debug.LogWarning("native ydpi: " + nativeDPI.y);
            }
            
            Debug.LogWarning("bannerHeight: " + bannerHeight);
            Debug.LogWarning("Screen.safeArea.height: " + Screen.safeArea.height);
            Debug.LogWarning("screen height: " + Screen.height);
            Debug.LogWarning("canvasScaler ref height: " + UI.canvasScaler.referenceResolution.y);
            var ratio = UI.canvasScaler.referenceResolution.y / Screen.height;
            bannerHeight *= ratio;
            bannerHeight += 10;
            Debug.LogWarning("scaleBannerHeight: " + bannerHeight);

            var listBannerAdsUI = UI.GetComponentsInChildren<BannerAdsUI>(true);
            foreach (var bannerAds in listBannerAdsUI)
            {
                bannerAds.SetSize(bannerHeight);
            }
        }

        Vector2 GetNativeDPI()
        {
            AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
            activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);

            return new Vector2(metrics.Get<float>("xdpi"), metrics.Get<float>("ydpi"));
        }
    }
}