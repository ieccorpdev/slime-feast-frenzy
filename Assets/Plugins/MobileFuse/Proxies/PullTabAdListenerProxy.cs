namespace MobileFuseAds.Proxies
{
    public class PullTabAdListenerProxy : AdListenerProxy
    {
        public event CallbackDelegate AdClosed;
        public event CallbackDelegate AdPulled;
        
        public PullTabAdListenerProxy() : base("PullTabAd"){}
        
        // Java MobileFuseBannerAd.Listener implementation
        // ReSharper disable UnusedMember.Global InconsistentNaming
        
        public void onAdClosed() => AdClosed?.Invoke();
        public void onAdPulled() => AdPulled?.Invoke();
        
        // ReSharper restore UnusedMember.Global InconsistentNaming
    }
}