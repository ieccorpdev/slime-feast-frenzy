#ifndef MobileFuseInitProxy_h
#define MobileFuseInitProxy_h

#import <MobileFuseSDK/IMFInitializationCallbackReceiver.h>

#ifdef __cplusplus
extern “C” {
    #endif
    typedef void (*NoArgsDelegate)();
    #ifdef __cplusplus
}
#endif

@interface MobileFuseInitProxy : NSObject<IMFInitializationCallbackReceiver>

@property NoArgsDelegate onSuccess;
@property NoArgsDelegate onError;

- (MobileFuseInitProxy *)init;

- (void)onInitSuccess:(NSString *)appId withPublisherId:(NSString *)publisherId; // Callback responder for init; called on success
- (void)onInitError:(NSString *)message forAppId:(NSString *)appId withPublisherId:(NSString *)publisherId; // Called if there is an error initializing the SDK

@end

#endif /* MobileFuseInitProxy_h */
