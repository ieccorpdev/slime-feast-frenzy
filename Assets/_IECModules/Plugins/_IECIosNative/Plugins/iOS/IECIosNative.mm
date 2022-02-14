#import <Foundation/Foundation.h>

extern "C" {
    
#pragma mark - Functions
    
    void _openAppSetting() {
       [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }
}
