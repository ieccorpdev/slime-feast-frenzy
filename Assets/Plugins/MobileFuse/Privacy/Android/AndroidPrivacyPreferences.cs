using MobileFuseAds.Interfaces;
using Plugins.MobileFuse.Helpers;
using UnityEngine;

namespace MobileFuseAds.Privacy.Android
{
    public class AndroidPrivacyPreferences : IPrivacyPreferences
    {
        public readonly AndroidJavaObject JavaObject;
        protected AndroidPrivacyPreferences(AndroidJavaObject obj)
        {
            JavaObject = obj;
        }

        public class Builder : ProxiedJavaInstance, IPrivacyPreferencesBuilder
        {
            public Builder() : base("com.mobilefuse.sdk.privacy.MobileFusePrivacyPreferences$Builder"){}

            public IPrivacyPreferencesBuilder SetUsPrivacyConsentString(string usPrivacyConsentString)
            {
                Call<AndroidJavaObject>("setUsPrivacyConsentString", usPrivacyConsentString);
                return this;
            }

            IPrivacyPreferencesBuilder IPrivacyPreferencesBuilder.SetIabConsentString(string consentString)
            {
                Call<AndroidJavaObject>("setIabConsentString", consentString);
                return this;
            }

            public IPrivacyPreferencesBuilder SetSubjectToCoppa(bool subjectToCoppa)
            {
                Call<AndroidJavaObject>("setSubjectToCoppa", subjectToCoppa);
                return this;
            }

            public IPrivacyPreferencesBuilder SetSubjectToGdpr(bool subjectToGdpr)
            {
                Call<AndroidJavaObject>("setSubjectToGdpr", subjectToGdpr);
                return this;
            }

            IPrivacyPreferences IPrivacyPreferencesBuilder.Build()
            {
                return new AndroidPrivacyPreferences(Call<AndroidJavaObject>("build"));
            }

        }
    }
}