#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>

static void AppendToNSArray(const char* carray[], int length, NSMutableArray<NSString*>* array)
{
    for (int i = 0; i < length; ++i)
    {
        [array addObject:[NSString stringWithUTF8String:carray[i]]];
    }
}

void _UT_ComposeEmail(const char* to[], int lengthTo, const char* cc[], int lengthCc, const char* bcc[], int lengthBcc, const char* subject, const char* body, bool htmlBody, const char* attachments[], int lengthAttachments)
{
    if (!body)
    {
        body = "";
    }
    
    if (!subject)
    {
        subject = "";
    }
    
    NSAttributedString* bodyAttributedString = nil;
    if (htmlBody)
    {
        NSString* htmlBodyText = [NSString stringWithUTF8String:body];
        NSData* htmlBodyData = [htmlBodyText dataUsingEncoding:NSUnicodeStringEncoding];
        bodyAttributedString = [[NSAttributedString alloc] initWithHTML:htmlBodyData documentAttributes:nil];
    }
    else
    {
        bodyAttributedString = [[NSAttributedString alloc] initWithString:[NSString stringWithUTF8String:body]];
    }
    //NSURL* attachmentFileURL = <...>;
    
    NSSharingService* mailShare = [NSSharingService sharingServiceNamed:NSSharingServiceNameComposeEmail];
    
    NSMutableArray<NSString*>* recipients = [[NSMutableArray<NSString*> alloc] init];
    if (to && lengthTo > 0)
    {
        AppendToNSArray(to, lengthTo, recipients);
    }
    if (cc && lengthCc > 0)
    {
        NSLog(@"CC recipients setting is not supported on OSX. Adding them as \"To:\"");
        AppendToNSArray(cc, lengthCc, recipients);
    }
    if (bcc && lengthBcc > 0)
    {
        NSLog(@"BCC recipients setting is not supported on OSX. Adding them as \"To:\"");
        AppendToNSArray(bcc, lengthBcc, recipients);
    }
    [mailShare setRecipients:recipients];
    
    [mailShare setSubject:[NSString stringWithUTF8String:subject]];
    
    NSMutableArray* shareItems = [NSMutableArray arrayWithCapacity:1 + lengthAttachments];
    [shareItems addObject:bodyAttributedString];
    if (attachments && lengthAttachments > 0)
    {
        for (int i = 0; i < lengthAttachments; ++i)
        {
            [shareItems addObject:[NSURL fileURLWithPath:[NSString stringWithUTF8String:attachments[i]]]];
        }
    }
    
    [mailShare performWithItems:shareItems];
}
