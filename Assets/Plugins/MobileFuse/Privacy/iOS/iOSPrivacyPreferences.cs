#if UNITY_IOS
using System.Runtime.InteropServices;
using MobileFuseAds.Interfaces;

namespace MobileFuseAds.Privacy.iOS
{
    // ReSharper disable once InconsistentNaming
    public class iOSPrivacyPreferences : IPrivacyPreferences
    {
        [DllImport("__Internal")]
        private static extern uint MobileFusePrivacyPreferences_createInstance();

        [DllImport("__Internal")]
        private static extern void MobileFusePrivacyPreferences_destroyInstance(uint instanceId);

        [DllImport("__Internal")]
        private static extern void MobileFusePrivacyPreferences_setSubjectToCoppa(uint instanceId, bool coppa);

        [DllImport("__Internal")]
        private static extern void MobileFusePrivacyPreferences_setSubjectToGdpr(uint instanceId, bool gdpr);

        [DllImport("__Internal")]
        private static extern void MobileFusePrivacyPreferences_setUsPrivacyConsentString(uint instanceId,
            string consentString);

        [DllImport("__Internal")]
        private static extern void MobileFusePrivacyPreferences_setIabConsentString(uint instanceId,
            string consentString);

        [DllImport("__Internal")]
        private static extern bool MobileFusePrivacyPreferences_isSubjectToCoppa(uint instanceId);

        [DllImport("__Internal")]
        private static extern bool MobileFusePrivacyPreferences_isSubjectToGdpr(uint instanceId);

        [DllImport("__Internal")]
        private static extern string MobileFusePrivacyPreferences_getUsPrivacyConsentString(uint instanceId);

        [DllImport("__Internal")]
        private static extern string MobileFusePrivacyPreferences_getIabConsentString(uint instanceId);

        public readonly uint InstanceId;

        protected iOSPrivacyPreferences(uint instanceId)
        {
            InstanceId = instanceId;
        }

        public class Builder : IPrivacyPreferencesBuilder
        {
            private readonly uint _instanceId;

            public Builder()
            {
                _instanceId = MobileFusePrivacyPreferences_createInstance();
            }

            public IPrivacyPreferencesBuilder SetUsPrivacyConsentString(string usPrivacyConsentString)
            {
                var localString = usPrivacyConsentString;
                MobileFusePrivacyPreferences_setUsPrivacyConsentString(_instanceId, localString);
                return this;
            }

            IPrivacyPreferencesBuilder IPrivacyPreferencesBuilder.SetIabConsentString(string consentString)
            {
                var localString = consentString;
                MobileFusePrivacyPreferences_setIabConsentString(_instanceId, localString);
                return this;
            }

            public IPrivacyPreferencesBuilder SetSubjectToCoppa(bool subjectToCoppa)
            {
                MobileFusePrivacyPreferences_setSubjectToCoppa(_instanceId, subjectToCoppa);
                return this;
            }

            public IPrivacyPreferencesBuilder SetSubjectToGdpr(bool subjectToGdpr)
            {
                MobileFusePrivacyPreferences_setSubjectToGdpr(_instanceId, subjectToGdpr);
                return this;
            }

            IPrivacyPreferences IPrivacyPreferencesBuilder.Build()
            {
                return new iOSPrivacyPreferences(this._instanceId);
            }
        }
    }
}
#endif