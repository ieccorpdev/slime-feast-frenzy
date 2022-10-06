namespace MobileFuseAds.Proxies
{
    public class BannerAdListenerProxy : AdListenerProxy
    {
        public event CallbackDelegate AdExpanded;
        public event CallbackDelegate AdCollapsed;
        
        public BannerAdListenerProxy() : base("BannerAd"){}
        
        // Java MobileFuseBannerAd.Listener implementation
        // ReSharper disable UnusedMember.Global InconsistentNaming
        
        public void onAdExpanded() => AdExpanded?.Invoke();
        public void onAdCollapsed() => AdCollapsed?.Invoke();
        
        // ReSharper restore UnusedMember.Global InconsistentNaming
    }
}