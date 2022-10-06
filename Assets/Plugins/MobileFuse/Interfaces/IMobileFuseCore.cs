namespace MobileFuseAds.Interfaces
{
    public interface IMobileFuseCore
    {
        void Init(int publisherId, int appId, MobileFuse.NoArgsDelegate onInitSuccess, MobileFuse.NoArgsDelegate onInitError);
        void SetPrivacyPreferences(IPrivacyPreferences prefs);
        void SetSettingOverride(string setting, string value);
    }
}