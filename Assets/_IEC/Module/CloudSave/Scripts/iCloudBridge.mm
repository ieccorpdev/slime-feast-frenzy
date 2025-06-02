#import <Foundation/Foundation.h>
#import <Foundation/NSUbiquitousKeyValueStore.h>
#import "UnityInterface.h"
#import <CloudKit/CloudKit.h>           // Optional for CloudKit
#import <UIKit/UIKit.h>
#import <sys/utsname.h>
#import <CoreFoundation/CoreFoundation.h>
#import <objc/runtime.h>
#import <Foundation/NSFileManager.h>     // ✅ Add this line

static void (*icloudSyncCallback)() = nil;

extern "C" {
    void Synchronize() {
        [[NSUbiquitousKeyValueStore defaultStore] synchronize];
    }

    void SaveToiCloud(const char* key, const char* value) {
        NSString* nsKey = [NSString stringWithUTF8String:key];
        NSString* nsValue = [NSString stringWithUTF8String:value];
        [[NSUbiquitousKeyValueStore defaultStore] setString:nsValue forKey:nsKey];
        [[NSUbiquitousKeyValueStore defaultStore] synchronize];
    }

    const char* LoadFromiCloud(const char* key) {
        NSString* nsKey = [NSString stringWithUTF8String:key];
        NSString* result = [[NSUbiquitousKeyValueStore defaultStore] stringForKey:nsKey];
        return result ? strdup([result UTF8String]) : "";
    }

    void RemoveFromiCloud(const char* key) {
        NSString* nsKey = [NSString stringWithUTF8String:key];
        [[NSUbiquitousKeyValueStore defaultStore] removeObjectForKey:nsKey];
        [[NSUbiquitousKeyValueStore defaultStore] synchronize];
    }

    bool iCloudAvailable() {
        return [NSUbiquitousKeyValueStore defaultStore] != nil;
    }

    void RegisteriCloudSyncCallback(void (*callback)()) {
        icloudSyncCallback = callback;

        [[NSNotificationCenter defaultCenter] addObserverForName:NSUbiquitousKeyValueStoreDidChangeExternallyNotification
                                                          object:[NSUbiquitousKeyValueStore defaultStore]
                                                           queue:[NSOperationQueue mainQueue]
                                                      usingBlock:^(NSNotification * _Nonnull note) {
            if (icloudSyncCallback) {
                icloudSyncCallback();
            }
        }];
    }
}
