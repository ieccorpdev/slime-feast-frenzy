//
//  MobileFusePrivacyPreferencesUnity.m
//  MobileFuseUnityBridge
//

#import "MobileFusePrivacyPreferencesUnity.h"
#import "NativeBridge.h"

uint MobileFusePrivacyPreferences_createInstance(void) {
    return (uint)[NativeBridge registerInstance:@"MobileFusePrivacyPreferences"];
}

void MobileFusePrivacyPreferences_destroyInstance(uint instanceId) {
    [NativeBridge unregsiterInstance:instanceId];
}

/*
 - (void)setSubjectToCoppa:(BOOL)coppa;
 */
void MobileFusePrivacyPreferences_setSubjectToCoppa(uint instanceId, bool coppa) {
    NSString *selectorString = @"setSubjectToCoppa:";
    
    // Method uses primitive types (bool) so we need to specify the function pointer directly
    void (*method)(id, SEL, bool) = (void *)[NativeBridge getInstanceMethod:selectorString onInstance:instanceId];
    method([NativeBridge getInstance:instanceId], [NativeBridge getMethodSelector:selectorString], coppa);
}

/*
 - (void)setSubjectToGdpr:(BOOL)gdpr;
 */
void MobileFusePrivacyPreferences_setSubjectToGdpr(uint instanceId, bool gdpr) {
    NSString *selectorString = @"setSubjectToGdpr:";
    
    // Method uses primitive types (bool) so we need to specify the function pointer directly
    void (*method)(id, SEL, bool) = (void *)[NativeBridge getInstanceMethod:selectorString onInstance:instanceId];
    method([NativeBridge getInstance:instanceId], [NativeBridge getMethodSelector:selectorString], gdpr);
}

/*
 - (void)setUsPrivacyConsentString:(NSString *)string;
 */
void MobileFusePrivacyPreferences_setUsPrivacyConsentString(uint instanceId, const char* string) {
    [NativeBridge invokeInstanceMethod:@"setUsPrivacyConsentString:" onInstance:instanceId withArguments:[NSString stringWithUTF8String: string]];
}

/*
 - (void)setIabConsentString:(NSString *)string;
 */
void MobileFusePrivacyPreferences_setIabConsentString(uint instanceId, const char* string) {
    [NativeBridge invokeInstanceMethod:@"setIabConsentString:" onInstance:instanceId withArguments:[NSString stringWithUTF8String: string]];
}

/*
 - (BOOL)isSubjectToCoppa;
*/
bool MobileFusePrivacyPreferences_isSubjectToCoppa(uint instanceId) {
    NSString *selectorString = @"isSubjectToCoppa";
    
    // Method uses primitive types (bool) so we need to specify the function pointer directly
    bool (*method)(id, SEL) = (void *)[NativeBridge getInstanceMethod:selectorString onInstance:instanceId];
    return method([NativeBridge getInstance:instanceId], [NativeBridge getMethodSelector:selectorString]);
}

/*
 - (BOOL)isSubjectToGdpr;
*/
bool MobileFusePrivacyPreferences_isSubjectToGdpr(uint instanceId) {
    NSString *selectorString = @"isSubjectToGdpr";
    
    // Method uses primitive types (bool) so we need to specify the function pointer directly
    bool (*method)(id, SEL) = (void *)[NativeBridge getInstanceMethod:selectorString onInstance:instanceId];
    return method([NativeBridge getInstance:instanceId], [NativeBridge getMethodSelector:selectorString]);
}

/*
 - (NSString *)getUsPrivacyConsentString;
*/
const char* MobileFusePrivacyPreferences_getUsPrivacyConsentString(uint instanceId) {
    return [NativeBridge convertString:[NativeBridge invokeInstanceMethod:@"getUsPrivacyConsentString" onInstance:instanceId]];
}

/*
 - (NSString *)getIabConsentString;
*/
const char* MobileFusePrivacyPreferences_getIabConsentString(uint instanceId) {
    return [NativeBridge convertString:[NativeBridge invokeInstanceMethod:@"getIabConsentString" onInstance:instanceId]];
}
