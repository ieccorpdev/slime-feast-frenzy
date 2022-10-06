using MobileFuseAds.Interfaces;
using MobileFuseAds.Privacy.Android;
#if UNITY_IOS
using MobileFuseAds.Privacy.iOS;
#endif

namespace MobileFuseAds.Privacy
{
    public static class MobileFusePrivacyPreferences
    {
        public class Builder : IPrivacyPreferencesBuilder
        {
            private readonly IPrivacyPreferencesBuilder _implementation;

            public Builder()
            {
#if UNITY_ANDROID
                _implementation = new AndroidPrivacyPreferences.Builder();
#elif UNITY_IOS
                _implementation = new iOSPrivacyPreferences.Builder();
#endif
            }
            
            public IPrivacyPreferencesBuilder SetUsPrivacyConsentString(string usPrivacyConsentString) => _implementation.SetUsPrivacyConsentString(usPrivacyConsentString);
            public IPrivacyPreferencesBuilder SetIabConsentString(string consentString) => _implementation.SetIabConsentString(consentString);
            public IPrivacyPreferencesBuilder SetSubjectToCoppa(bool subjectToCoppa) => _implementation.SetSubjectToCoppa(subjectToCoppa);
            public IPrivacyPreferencesBuilder SetSubjectToGdpr(bool subjectToGdpr) => _implementation.SetSubjectToGdpr(subjectToGdpr);
            public IPrivacyPreferences Build() => _implementation.Build();
        }
    }
}