using UnityEngine;

namespace MobileFuseAds.Proxies
{
    public class AdListenerProxy : AndroidJavaProxy
    {
        public delegate void CallbackDelegate();
        public delegate void ErrorCallbackDelegate(string error);

        public event CallbackDelegate AdLoaded;
        public event CallbackDelegate AdNotFilled;
        public event CallbackDelegate AdRendered;
        public event CallbackDelegate AdClicked;
        public event CallbackDelegate AdExpired;
        public event ErrorCallbackDelegate AdError;
        
        public AdListenerProxy(string type) : base($"com.mobilefuse.sdk.MobileFuse{type}$Listener"){}
        
        // Java Listener implementation
        // ReSharper disable UnusedMember.Global InconsistentNaming
        
        public void onAdLoaded() => AdLoaded?.Invoke();
        public void onAdNotFilled() => AdNotFilled?.Invoke();
        public void onAdRendered() => AdRendered?.Invoke();
        public void onAdClicked() => AdClicked?.Invoke();
        public void onAdExpired() => AdExpired?.Invoke();
        public void onAdError(AndroidJavaObject adError) => AdError?.Invoke(adError.Get<string>("errorMessage"));
        
        // ReSharper restore UnusedMember.Global InconsistentNaming
    }
}