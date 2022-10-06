//
//  NativeBridge.h
//  MobileFuseUnityBridge
//

#ifndef NativeBridge_h
#define NativeBridge_h

@interface NativeBridge : NSObject
+ (char *)convertString:(NSString *)string;
+ (SEL)getMethodSelector:(NSString *)method;

+ (IMP)getClassMethod:(NSString *)method onClass:(NSString *)className;
+ (id)invokeClassMethod:(NSString *)method onClass:(NSString *)className;
+ (id)invokeClassMethod:(NSString *)method onClass:(NSString *)className withArguments:(id)argument1, ...;

+ (IMP)getInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId;
+ (id)invokeInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId;
+ (id)invokeInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId withArguments:(id)argument1, ...;

+ (NSUInteger)registerInstance:(NSString *)className;
+ (NSObject *)getInstance:(NSUInteger)instanceId;
+ (void)unregsiterInstance:(NSUInteger) instanceId;

@end

#endif /* NativeBridge_h */
