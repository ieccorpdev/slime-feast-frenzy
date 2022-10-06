//
//  NativeBridge.m
//  MobileFuseUnityBridge
//

#import "NativeBridge.h"

#include <stdarg.h>

@implementation NativeBridge

static NSMutableDictionary *_instances;
static NSUInteger _instanceId;

+ (char *)convertString:(NSString *)string {
    const char* stringUtf8 = string.UTF8String;
    
    // Allocate strings on the heap
    char *cString = (char *)malloc(strlen(stringUtf8) + 1); // Extra byte for NULL terminator
    strcpy(cString, stringUtf8);
    return cString;
}

+ (SEL)getMethodSelector:(NSString *)method {
    return NSSelectorFromString(method);
}
    
+ (IMP)getClassMethod:(NSString *)method onClass:(NSString *)className {
    // Build a selector so we can invoke a method
    SEL selector = [self getMethodSelector:method];
    
    if ([NSClassFromString(className) respondsToSelector:selector]) {
        // Create a function pointer that we can return if the class contains the selector
        return [NSClassFromString(className) methodForSelector:selector]; // Turn selector into function IMP
    }
    
    return NULL;
}

+ (id)invokeClassMethod:(NSString *)method onClass:(NSString *)className withArray:(NSArray *)arguments {
    // Build a selector so we can invoke a method. Arguments will be sent as id
    SEL selector = [self getMethodSelector:method];
    
    // Create a function pointer to our selector and execute the function call
    IMP selectorImplementation = [NSClassFromString(className) methodForSelector:selector]; // Turn selector into function IMP
    
    if ([NSClassFromString(className) respondsToSelector:selector]) {
        if ([arguments count] == 0) {
            id (*selectorFunction0)(id, SEL) = (void *)selectorImplementation;
            return selectorFunction0(NSClassFromString(className), selector);
        } else if([arguments count] == 1) {
            id (*selectorFunction1)(id, SEL, id) = (void *)selectorImplementation;
            return selectorFunction1(NSClassFromString(className), selector, arguments[0]);
        } else if([arguments count] == 2) {
            id (*selectorFunction2)(id, SEL, id, id) = (void *)selectorImplementation;
            return selectorFunction2(NSClassFromString(className), selector, arguments[0], arguments[1]);
        } else if([arguments count] == 3) {
            id (*selectorFunction3)(id, SEL, id, id, id) = (void *)selectorImplementation;
            return selectorFunction3(NSClassFromString(className), selector, arguments[0], arguments[1], arguments[2]);
        } else if([arguments count] == 4) {
            id (*selectorFunction4)(id, SEL, id, id, id, id) = (void *)selectorImplementation;
            return selectorFunction4(NSClassFromString(className), selector, arguments[0], arguments[1], arguments[2], arguments[3]);
        } else if([arguments count] == 5) {
            id (*selectorFunction5)(id, SEL, id, id, id, id, id) = (void *)selectorImplementation;
            return selectorFunction5(NSClassFromString(className), selector, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
        } else {
            id (*selectorFunction6)(id, SEL, id, id, id, id, id, id) = (void *)selectorImplementation;
            return selectorFunction6(NSClassFromString(className), selector, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
        }
    } else {
        NSLog(@"[MobileFuse NativeBridge] Warning: Invalid selector sent to instance with class %@ and method %@", className, method);
    }
    
    return NULL;
}

+ (id)invokeClassMethod:(NSString *)method onClass:(NSString *)className {
    return [NativeBridge invokeClassMethod:method onClass:className withArray:@[ ]];
}

+ (id)invokeClassMethod:(NSString *)method onClass:(NSString *)className withArguments:(id)argument, ... {
    NSMutableArray *arguments = [[NSMutableArray alloc] init];
    NSRegularExpression *regex = [NSRegularExpression regularExpressionWithPattern:@":" options:NSRegularExpressionCaseInsensitive error:NULL];
    NSUInteger argumentCount = [regex numberOfMatchesInString:method options:0 range:NSMakeRange(0, [method length])];
    
    if (argument != NULL) {
        va_list vaArgs;
        va_start(vaArgs, argument);
        id arg = argument;
        
        // We have to be careful with how we access the args in va_arg
        // Stop iterating 1 before the end and add out final argument to our array after the loop ends
        for(NSUInteger i = 0; i < argumentCount - 1; i++) {
            [arguments addObject:arg];
            arg = va_arg(vaArgs, id); // Prepare next argument
        }
        [arguments addObject:arg];

        va_end(vaArgs);
    }
    
    return [NativeBridge invokeClassMethod:method onClass:className withArray:arguments];
}

+ (IMP)getInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId {
    // Build a selector so we can invoke a method
    SEL selector = [self getMethodSelector:method];
    
    NSObject *instance = [NativeBridge getInstance:instanceId];
    if ([instance respondsToSelector:selector]) {
        return [instance methodForSelector:selector]; // Turn selector into function IMP and return it
    }
    
    return NULL;
}

+ (id)invokeInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId withArray:(NSArray *)arguments {
    // Build a selector so we can invoke a method. Arguments will be sent as id
    SEL selector = [self getMethodSelector:method];
    
    NSObject *instance = [NativeBridge getInstance:instanceId];
    
    // Create a function pointer to our selector and execute the function call
    IMP selectorImplementation = [instance methodForSelector:selector]; // Turn selector into function IMP
    if ([instance respondsToSelector:selector]) {
        if ([arguments count] == 0) {
            id (*selectorFunction0)(id, SEL) = (void *)selectorImplementation;
            return selectorFunction0(instance, selector);
        } else if([arguments count] == 1) {
            id (*selectorFunction1)(id, SEL, id) = (void *)selectorImplementation;
            return selectorFunction1(instance, selector, arguments[0]);
        } else if([arguments count] == 2) {
            id (*selectorFunction2)(id, SEL, id, id) = (void *)selectorImplementation;
            return selectorFunction2(instance, selector, arguments[0], arguments[1]);
        } else if([arguments count] == 3) {
            id (*selectorFunction3)(id, SEL, id, id, id) = (void *)selectorImplementation;
            return selectorFunction3(instance, selector, arguments[0], arguments[1], arguments[2]);
        } else if([arguments count] == 4) {
            id (*selectorFunction4)(id, SEL, id, id, id, id) = (void *)selectorImplementation;
            return selectorFunction4(instance, selector, arguments[0], arguments[1], arguments[2], arguments[3]);
        } else if([arguments count] == 5) {
            id (*selectorFunction5)(id, SEL, id, id, id, id, id) = (void *)selectorImplementation;
            return selectorFunction5(instance, selector, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
        } else {
            id (*selectorFunction6)(id, SEL, id, id, id, id, id, id) = (void *)selectorImplementation;
            return selectorFunction6(instance, selector, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
        }
    } else {
        NSLog(@"[MobileFuse NativeBridge] Warning: Invalid selector sent to instance %@,method %@", [instance description], method);
    }
    
    return NULL;
}

+ (id)invokeInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId {
    return [NativeBridge invokeInstanceMethod:method onInstance:instanceId withArray:@[ ]];
}

+ (id)invokeInstanceMethod:(NSString *)method onInstance:(NSUInteger)instanceId withArguments:(id)argument, ... {
    NSMutableArray *arguments = [[NSMutableArray alloc] init];
    NSRegularExpression *regex = [NSRegularExpression regularExpressionWithPattern:@":" options:NSRegularExpressionCaseInsensitive error:NULL];
    NSUInteger argumentCount = [regex numberOfMatchesInString:method options:0 range:NSMakeRange(0, [method length])];
    
    if (argument != NULL) {
        va_list vaArgs;
        va_start(vaArgs, argument);
        id arg = argument;
        
        // We have to be careful with how we access the args in va_arg
        // Stop iterating 1 before the end and add out final argument to our array after the loop ends
        for(NSUInteger i = 0; i < argumentCount - 1; i++) {
            [arguments addObject:arg];
            arg = va_arg(vaArgs, id); // Prepare next argument
        }
        [arguments addObject:arg];

        va_end(vaArgs);
    }
    
    return [NativeBridge invokeInstanceMethod:method onInstance:instanceId withArray:(NSArray *)arguments];
}

+ (NSUInteger)registerInstance:(NSString *)className {
    NSObject *instance = [[NSClassFromString(className) alloc] init];
    
    if(_instances == NULL) _instances = [[NSMutableDictionary alloc] init];
    
    [_instances setObject:instance forKey:[NSString stringWithFormat:@"%ld", (unsigned long)_instanceId++]];
    return [_instances count] - 1;
}

+ (NSObject *)getInstance:(NSUInteger)instanceId {
    return [_instances objectForKey:[NSString stringWithFormat:@"%ld", (unsigned long)instanceId]];
}

+ (void)unregsiterInstance:(NSUInteger)instanceId {
    [_instances removeObjectForKey:[NSString stringWithFormat:@"%ld", (unsigned long)instanceId]];
}

@end
