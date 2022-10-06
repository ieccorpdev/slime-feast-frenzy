#import "MobileFuseInitProxy.h"

#include <stdarg.h>

@interface MobileFuseInitProxy ()

@end

@implementation MobileFuseInitProxy

@synthesize onError;
@synthesize onSuccess;

- (MobileFuseInitProxy *)init {
    self = [super init];
    NSLog(@"UNITYPLUGIN Create init proxy");
    return self;
}

// Called if there is an error initializing the SDK
- (void)onInitError:(NSString *)message forAppId:(NSString *)appId withPublisherId:(NSString *)publisherId {
    NSLog(@"UNITYPLUGIN onInitError for app ID %@ and publisher ID %@ with message: %@", appId, publisherId, message);
    if (onError == NULL) {
        NSLog(@"OnError is null");
    } else {
        NSLog(@"Calling OnError");
        onError();
    }
}

// IMFInitializationCallbackReceiver delegate implementation
// Callback responder for init; called on success
- (void)onInitSuccess:(NSString *)appId withPublisherId:(NSString *)publisherId {
    NSLog(@"UNITYPLUGIN onInitSuccess for app ID %@ and publisher ID %@", appId, publisherId);
    onSuccess();
}

@end
