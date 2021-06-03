#import <Foundation/Foundation.h>
#import <MobileCoreServices/MobileCoreServices.h>
#import "UTMail.h"

static UTMailController* utmailController = nil;
UIViewController* UnityGetGLViewController();   // Implemented by Unity

@implementation UTMailController
- (void)mailComposeController:(MFMailComposeViewController*)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError*)error
{
    switch (result) {
        case MFMailComposeResultSent:
            break;
        case MFMailComposeResultSaved:
            break;
        case MFMailComposeResultCancelled:
            break;
        case MFMailComposeResultFailed:
            break;
        default:
            NSLog(@"An error occurred when trying to compose this email");
            break;
    }

    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion: ^
    {
        utmailController = nil;
    }];
}
@end

static NSArray<NSString*>* ToNSArray(const char* carray[], int length)
{
    NSMutableArray<NSString*>* array = [[NSMutableArray<NSString*> alloc] init];

    for (int i = 0; i < length; ++i)
    {
        [array addObject:[NSString stringWithUTF8String:carray[i]]];
    }

    return array;
}

static NSString* mimeByFileName(NSString* fileName)
{
    CFStringRef UTI = UTTypeCreatePreferredIdentifierForTag(kUTTagClassFilenameExtension, (__bridge CFStringRef)[fileName pathExtension], NULL);
    CFStringRef MIMEType = UTTypeCopyPreferredTagWithClass(UTI, kUTTagClassMIMEType);
    CFRelease(UTI);
    if (!MIMEType) {
        return @"application/octet-stream";
    }
    return (__bridge NSString *)(MIMEType);
}

void _UT_ComposeEmail(const char* to[], int lengthTo, const char* cc[], int lengthCc, const char* bcc[], int lengthBcc, const char* subject, const char* body, bool htmlBody, const char* attachments[], int lengthAttachments)
{
    if (![MFMailComposeViewController canSendMail])
    {
       NSLog(@"Mail services are not available.");
       return;
    }

    if (utmailController != nil)
    {
        NSLog(@"MFMailComposeViewController is already active!");
        return;
    }

    MFMailComposeViewController* composeViewController = [[MFMailComposeViewController alloc] init];

    utmailController = [[UTMailController alloc] init];
    composeViewController.mailComposeDelegate = utmailController;

    if (to && lengthTo > 0)
    {
        [composeViewController setToRecipients:ToNSArray(to, lengthTo)];
    }

    if (cc && lengthCc > 0)
    {
        [composeViewController setCcRecipients:ToNSArray(cc, lengthCc)];
    }

    if (bcc && lengthBcc > 0)
    {
        [composeViewController setBccRecipients:ToNSArray(bcc, lengthBcc)];
    }

    if (subject)
    {
        [composeViewController setSubject:[NSString stringWithUTF8String:subject]];
    }

    if (body)
    {
        [composeViewController setMessageBody:[NSString stringWithUTF8String:body] isHTML:htmlBody];
    }

    if (attachments)
    {
        for (int i = 0; i < lengthAttachments; ++i)
        {
            NSString* fileName = [NSString stringWithUTF8String:attachments[i]];
            [composeViewController addAttachmentData:[NSData dataWithContentsOfFile:fileName] mimeType:mimeByFileName(fileName) fileName:[fileName lastPathComponent]];
        }
    }

    [UnityGetGLViewController() presentViewController:composeViewController animated:YES completion:nil];
}
