namespace MobileFuseAds.Proxies
{
    public class InterstitialAdListenerProxy : AdListenerProxy
    {
        public event CallbackDelegate AdClosed;
        
        public InterstitialAdListenerProxy() : base("InterstitialAd"){}
        
        // Java MobileFuseInterstitialAd.Listener implementation
        // ReSharper disable UnusedMember.Global InconsistentNaming
        
        public void onAdClosed() => AdClosed?.Invoke();
        
        // ReSharper restore UnusedMember.Global InconsistentNaming
    }
}