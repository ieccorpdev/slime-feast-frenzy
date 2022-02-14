# Introduction
UTMail is Unity extension allowing developers composing and sending emails
directly from C# code. It provides common cross-platform API and works on
multiple platforms: macOS, Windows, Windows Store (Windows Phone 8.1,
    Windows 8.1/10, Universal 8.1, Universal 10), Android, iOS.

It's an essential tool for integrating "Contact Support" functionality, and
is also a very effective way of getting feedback for beta versions and internal
test sessions. It may also be used for automatic or half-automatic reporting on
crashes and other events.

Features:
- Compose: opens a system email client with a content defined by your app.
    End-user can then optionally modify the message and send it. The email is
    sent from the end-user's email address so you can give them a reply.
- Send: uses SMTP to send emails directly, without showing any windows or
    popups.
- Attachments.
- Multiple To, Cc and Bcc recipients; Subject, Body, HTML Body are supported for
    both composing and sending.
- A sample & test scene, which is completely functional even right in the Unity
    Editor.

# Getting Started
It's recommended that you take a look at the provided sample scene
`Assets/UniversalTools/UTMail/Sample/UTMailSample.unity` and script
UT.MailSample.UTMailSample to get acquainted with UTMail functionality and API.

Please configure section "SMTP Settings for Sample Sending" in `UTMail Sample`
GameObject Inspector to be able to send emails directly (without composing).

You can find the complete documentation here:
https://universal-tools.github.io/UTMail/index.html.

## Building UT.MailMessage
UT.MailMessage object defines an email content to be sent or composed. You can
add a number of "To", "Cc" and "Bcc" recipients, set the message subject and
body and define whether the body should be considered as HTML. You can also add
any number of attachments. All the fields are optional, f.e it is possible to
compose a completely empty UT.MailMessage.

Please note that UT.MailMessage is an `IDisposable` object and should be either
disposed or wrapped with `using` block. For more details on using `IDisposable`
objects in C#, see https://msdn.microsoft.com/en-us/library/system.idisposable(v=vs.110).aspx.

Example:
```
using (var message = new UT.MailMessage()
                        .AddTo("utmail-test@universal-tools.com")
                        .AddTo("my-owesome-email@my-owesome-domain.com")
                        .AddCc("one-more-email@my-owesome-domain.com")
                        .SetSubject("Hello world!")
                        .SetBody("<p>... the <a href=\"https://unity3d.com\">Unity</a> World!</p>"))
                        .SetBodyHtml(true)
                        .AddAttachment("Text as attachment", "Attachment.txt")
                        .AddAttachment(Application.temporaryCachePath + "/GameLog.txt"))
{
    // <compose or send message>
}
```

Note: HTML bodies are not supported in Windows and Windows Store builds.

See also API Reference for class UT.MailMessage.

## Composing
Composing shows UT.MailMessage in a system email client, so end-user can modify
and send the message from their own email address.

Example:
```
using (var message = new UT.MailMessage()
                        .AddTo("support@mygame.com")
                        .SetSubject("Support request from user #12345")
                        .AddAttachment(Application.temporaryCachePath + "/GameLog.txt"))
{
    // annotation is used as a title of an email app picking dialog on Android, it's ignored on other platforms.
    UT.Mail.Compose(message, annotation: "Choose an app to send email to My Game Support Team");
}
```

Note: macOS email composing API doesn't support specifying "Cc" & "Bcc"
recipients, they will be added as "To" when composing on macOS instead.

See also API Reference for UT.Mail.Compose.

## Sending
SMTP protocol is used to send UT.MailMessage directly. It requires at least one
recipient specified as "To", "Cc" or "Bcc".

Example:
```
using (var message = new UT.MailMessage()
                        .AddTo("event-handling@mygame.com")
                        .SetSubject("Event #123")
                        .AddAttachment(Application.temporaryCachePath + "/GameLog.txt"))
{
    UT.Mail.Send(message, "smtp.gmail.com", "mygame@gmail.com", "mypa$$w0rd", true);
}
```

Note that in order to send emails directly from a Gmail account, you'll have to
enable "Allow less secure apps" toggle in your gmail account security settings:
https://myaccount.google.com/security.

See also API Reference for each of 4 overloads of UT.Mail.Send.

# Contacts
If you got any questions please feel free to contact us: utmail@universal-tools.com.

If you liked using UTMail, please rate it, but any criticism is also welcome:
please help us make the asset better.

Thank you for using UTMail!

Your Universal Tools team.
