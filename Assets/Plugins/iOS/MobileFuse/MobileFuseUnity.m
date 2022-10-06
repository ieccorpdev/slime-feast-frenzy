//
//  MobileFuseUnity.m
//  MobileFuseUnityBridge
//

#import "MobileFuseUnity.h"
#import "NativeBridge.h"
#import "MobileFuseInitProxy.h"

MobileFuseInitProxy *initProxy;
/*
 + (void)initWithAppId:(NSString*)appId withPublisherId:(NSString *)publisherId withDelegate:(id<IMFInitializationCallbackReceiver>)delegate;
 */
void MobileFuse_initWithAppIdWithPublisherId(const char* appId, const char *publisherId, NoArgsDelegate onSuccess, NoArgsDelegate onError) {
    NSLog(@"UNITYPLUGIN Run init");
    initProxy = [[MobileFuseInitProxy alloc] init];
    initProxy.onSuccess = onSuccess;
    initProxy.onError = onError;
    
    [NativeBridge invokeClassMethod:@"initWithAppId:withPublisherId:withDelegate:" onClass:@"MobileFuse" withArguments:[NSString stringWithUTF8String: appId], [NSString stringWithUTF8String: publisherId], initProxy];
}

/*
 + (BOOL)isReady;
 */
bool MobileFuse_isReady(void) {
    return [NativeBridge invokeClassMethod:@"isReady" onClass:@"MobileFuse"];
}

/*
 + (void)setPrivacyPreferences(uint instanceId);
 */
void MobileFuse_setPrivacyPreferences(uint instanceId) {
    [NativeBridge invokeClassMethod:@"setPrivacyPreferences:" onClass:@"MobileFuse" withArguments:[NativeBridge getInstance:instanceId]];
}
