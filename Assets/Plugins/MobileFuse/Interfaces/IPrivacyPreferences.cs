namespace MobileFuseAds.Interfaces
{
    public interface IPrivacyPreferences
    {
        
    }

    public interface IPrivacyPreferencesBuilder
    {
        IPrivacyPreferencesBuilder SetUsPrivacyConsentString(string usPrivacyConsentString);
        IPrivacyPreferencesBuilder SetIabConsentString(string consentString);
        IPrivacyPreferencesBuilder SetSubjectToCoppa(bool subjectToCoppa);
        IPrivacyPreferencesBuilder SetSubjectToGdpr(bool subjectToGdpr);
        IPrivacyPreferences Build();
    }
}