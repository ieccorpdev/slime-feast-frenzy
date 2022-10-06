//
//  MobileFuseUnity.h
//  MobileFuseUnityBridge
//

#ifndef MobileFuseUnity_h
#define MobileFuseUnity_h

#ifdef __cplusplus
extern “C” {
#endif
    typedef void (*NoArgsDelegate)();
    void MobileFuse_initWithAppIdWithPublisherId(const char* appId, const char *publisherId, NoArgsDelegate onSuccess, NoArgsDelegate onError);
    bool MobileFuse_isReady(void);
    void MobileFuse_setPrivacyPreferences(uint instanceId);
#ifdef __cplusplus
}
#endif

#endif /* MobileFuseUnity_h */
