#if !UNITY_EDITOR && UNITY_ANDROID

using System.Text;
using UnityEngine;

namespace UT
{
    public class MailImpl
    {
        public static void Compose(MailMessage message, string annotation)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.mail.Manager"))
                {
                    manager.CallStatic("compose", annotation, MailUtil.MailAddressCollectionToStrings(message.To), MailUtil.MailAddressCollectionToStrings(message.CC), MailUtil.MailAddressCollectionToStrings(message.Bcc), message.Subject, message.Body, message.IsBodyHtml, MailUtil.AttachmentsToFilePaths(message.Attachments));
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }
    }
}

#elif UNITY_EDITOR && UNITY_ANDROID

using UnityEditor;

[InitializeOnLoad]
public class UTMailConfigurator
{
    static UTMailConfigurator()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // Required to send attachments
            PlayerSettings.Android.forceSDCardPermission = true;
        }
    }
}

#endif