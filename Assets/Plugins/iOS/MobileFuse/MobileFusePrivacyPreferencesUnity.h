//
//  MobileFusePrivacyPreferencesUnity.h
//  MobileFuseUnityBridge
//

#ifndef MobileFusePrivacyPreferencesUnity_h
#define MobileFusePrivacyPreferencesUnity_h

#ifdef __cplusplus
extern “C” {
#endif
    uint MobileFusePrivacyPreferences_createInstance(void);
    void MobileFusePrivacyPreferences_destroyInstance(uint instanceId);
    
    void MobileFusePrivacyPreferences_setSubjectToCoppa(uint instanceId, bool coppa);
    void MobileFusePrivacyPreferences_setSubjectToGdpr(uint instanceId, bool gdpr);
    void MobileFusePrivacyPreferences_setUsPrivacyConsentString(uint instanceId, const char* string);
    void MobileFusePrivacyPreferences_setIabConsentString(uint instanceId, const char* string);
    
    bool MobileFusePrivacyPreferences_isSubjectToCoppa(uint instanceId);
    bool MobileFusePrivacyPreferences_isSubjectToGdpr(uint instanceId);
    const char* MobileFusePrivacyPreferences_getUsPrivacyConsentString(uint instanceId);
    const char* MobileFusePrivacyPreferences_getIabConsentString(uint instanceId);

#ifdef __cplusplus
}
#endif

#endif /* MobileFusePrivacyPreferencesUnity_h */
