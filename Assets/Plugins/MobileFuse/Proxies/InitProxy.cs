using UnityEngine;

namespace MobileFuseAds.Proxies
{
    internal class InitProxy : AndroidJavaProxy
    {
        public delegate void CallbackDelegate();

        public event CallbackDelegate InitSuccess;
        public event CallbackDelegate InitError;
            
        public InitProxy() : base("com.mobilefuse.sdk.SdkInitListener"){}
        
        // Java SdkInitListener implementation
        // ReSharper disable UnusedMember.Global InconsistentNaming
        
        public void onInitSuccess() => InitSuccess?.Invoke();
        public void onInitError() => InitError?.Invoke();
        
        // ReSharper restore UnusedMember.Global InconsistentNaming
    }
}