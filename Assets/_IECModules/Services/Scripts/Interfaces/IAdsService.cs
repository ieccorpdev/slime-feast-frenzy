using System;

namespace IEC
{
    public interface IAdsService
    {
        string GetInfo(AdsEnum adsType);
        bool IsAcceptGDPR();
        void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback);
        void Show(AdsEnum adsType, Action<AdsEventArgs> showCallback);
        void SetOnClick(AdsEnum adsType, Action clickCallback);
        bool IsLoaded(AdsEnum adsType);
        void Hide(AdsEnum adsType);
        double GetEcpm(AdsEnum adsType);

        float GetBannerHeight();
    }

    public enum AdsEnum
    {
        BANNER_TOP,
        BANNER_BOTTOM,
        INTERSTITIAL,
        REWARD
    }

    [Serializable]
    public class AdsConfig
    {
        public AdsPlatformConfig android;
        public AdsPlatformConfig ios;
    }

    [Serializable]
    public class AdsPlatformConfig
    {
        public string appID;
        public string interstitialID;
        public string bannerID;
        public string rewardID;
    }

    public class AdsStateData
    {
        public Action<AdsEventArgs> onLoad;
        public Action<AdsEventArgs> onShow;
        public Action onClick;
        public bool isLoading;
        public bool isLoaded;
        public bool isWaitToShow;

        public static Action<AdsEventArgs> SafeInvoke(Action<AdsEventArgs> callback, AdsEventArgs param)
        {
            UnityToolbag.Dispatcher.InvokeAsync(() =>
            {
                if (callback != null)
                {
                    callback.Invoke(param);
                }
            });
            return null;
        }

        public static Action SafeInvoke(Action callback)
        {
            UnityToolbag.Dispatcher.InvokeAsync(() =>
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
            return null;
        }

        public static void DebugLog(string s)
        {
            UnityEngine.Debug.Log(s);
        }

        public void Reset()
        {
            isLoading = false;
            isLoaded = false;
            isWaitToShow = false;
        }
    }

    public class AdsEventArgs : EventArgs
    {
        public static readonly AdsEventArgs FAIL = new AdsEventArgs(false, "");
        public static readonly AdsEventArgs FAIL_NO_ID = new AdsEventArgs(false, "Empty ads ID");
        public static readonly AdsEventArgs FAIL_NOT_LOADED = new AdsEventArgs(false, "Empty ads ID");
        public static readonly AdsEventArgs DONE = new AdsEventArgs(true, "");
        public static readonly AdsEventArgs DONE_SKIP = new AdsEventArgs(true, "", true);
        public static readonly AdsEventArgs DONE_NO_SKIP = new AdsEventArgs(true, "", false);

        public readonly string message;
        public readonly bool isSuccess;
        public readonly bool isSkip;

        public AdsEventArgs(bool isSuccess, string message)
        {
            this.isSuccess = isSuccess;
            this.message = message;
            isSkip = true;
        }

        public AdsEventArgs(bool isSuccess, string message, bool isSkip)
        {
            this.isSuccess = isSuccess;
            this.message = message;
            this.isSkip = isSkip;
        }
    }
}

