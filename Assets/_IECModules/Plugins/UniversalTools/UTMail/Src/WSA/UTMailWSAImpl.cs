#if !UNITY_EDITOR && UNITY_WSA

using System;

namespace UT
{
    public class MailImpl
    {
        public static void Compose(MailMessage message, string annotation)
        {
            if (message.IsBodyHtml)
            {
                UnityEngine.Debug.LogError("Windows Store doesn't support IsBodyHtml property when composing an email. The property will be ignored.\nSee https://social.msdn.microsoft.com/Forums/en-US/8125da13-7d79-4a5a-82a5-57c8a322be78/uwp10240c-emailmessage-with-emailmessagebodykindhtml");
            }

            UnityEngine.WSA.Application.InvokeOnUIThread(() => AsyncCompose(message), false);
        }

        public static void Send(MailMessage message, string host, int port, string senderEmail, string account, string password, bool enableSsl, Mail.ResultHandler resultHandler)
        {
#if UNITY_WSA_8_1
            UnityEngine.Debug.LogWarning("Non-phone 8.1 Windows Store sending is not reliable and may not work!");
#endif

            if (port != 465)
            {
                // Modern SMTP (usually using port 587) upgrade security level after initial unsecure connection.
                // when receiving STARTTLS command. See https://en.wikipedia.org/wiki/SMTPS.
                enableSsl = false;
            }

            message = new MailMessage(message);
            global::System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    using (var smtpClient = new LightBuzz.SMTP.SmtpClient(host, port))
                    {
                        smtpClient.Init(account, password, enableSsl);

                        var result = await smtpClient.SendMailAsync(await message.EmailMessage(), senderEmail);
                        string errorMessage = null;
                        if (result != LightBuzz.SMTP.SmtpResult.OK)
                        {
                            errorMessage = result.ToString();
                            UnityEngine.Debug.LogError("UTMail: failed to send email: " + errorMessage);
                        }

                        if (resultHandler != null)
                        {
                            MailUtil.Instance.DispatchToMainThread(() =>
                            {
                                try
                                {
                                    resultHandler(message, errorMessage == null, errorMessage);
                                }
                                finally
                                {
                                    message.Dispose();
                                }
                            });
                        }
                        else
                        {
                            message.Dispose();
                        }
                    }
                }
                catch (global::System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    if (resultHandler != null)
                    {
                        string errorMessage = e.Message;
                        MailUtil.Instance.DispatchToMainThread(() =>
                        {
                            try
                            {
                                resultHandler(message, false, errorMessage);
                            }
                            finally
                            {
                                message.Dispose();
                            }
                        });
                    }
                    else
                    {
                        message.Dispose();
                    }
                }
            });
        }

        private static async void AsyncCompose(MailMessage message)
        {
            try
            {
                await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(await message.EmailMessage());
            }
            catch (global::System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }
}

#elif UNITY_EDITOR && UNITY_WSA

using UnityEditor;

[InitializeOnLoad]
public class UTMailConfigurator
{
    static UTMailConfigurator()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // Required for SMTP in WSA/UWP
            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
        }
    }
}

#endif