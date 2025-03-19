#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C" {
    
#pragma mark - Functions
    
    void _openAppSetting() {
       [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }

	void _openURL(const char* urlString) {
        NSString *urlStringObj = [NSString stringWithUTF8String:urlString];
        NSURL *url = [NSURL URLWithString:urlStringObj];
        if ([[UIApplication sharedApplication] canOpenURL:url]) {
            [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
        } else {
            NSLog(@"URL is invalid or cannot be opened.");
        }
    }
}
