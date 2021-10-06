#if !UNITY_EDITOR && UNITY_WSA && UNITY_WSA_8_1

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Windows.ApplicationModel.Email
{
    using MailAddressCollection = global::System.Collections.Generic.List<EmailRecipient>;
    
    public sealed class EmailManager
    {
        public static Windows.Foundation.IAsyncAction ShowComposeNewEmailAsync(EmailMessage message)
        {
            UnityEngine.Debug.LogWarning("Non-phone 8.1 Windows Store doesn't provide any email composing API. Falling back to \"mailto:\" solution...");
            if (message.Attachments.Count > 0)
            {
                // See http://stackoverflow.com/questions/24646815/send-email-in-windows-universal-app.
                Debug.LogError("Non-phone 8.1 Windows Store composing doesn't support attachments!");
            }

            string uri = string.Format("mailto:{0}?{1}{2}{3}{4}", ToAddressString(message.To), ToArgument("cc", message.CC), ToArgument("bcc", message.Bcc), ToArgument("subject", message.Subject), ToArgument("body", message.Body));
            UnityEngine.WSA.Application.InvokeOnAppThread(() => { Application.OpenURL(uri); }, false);

            return Task.Run(() => {}).AsAsyncAction();
        }

        private static string ToAddressString(MailAddressCollection addressCollection)
        {
            if (addressCollection == null || addressCollection.Count == 0)
            {
                return "";
            }
            else
            {
                string result = addressCollection[0].Address;
                for (int i = 1; i < addressCollection.Count; ++i)
                {
                    result = result + ";" + addressCollection[i];
                }

                return result;
            }
        }

        private static string ToArgument(string argumentName, MailAddressCollection addressCollection)
        {
            if (addressCollection == null || addressCollection.Count == 0)
            {
                return "";
            }
            else
            {
                return argumentName + "=" + ToAddressString(addressCollection) + "&";
            }
        }

        private static string ToArgument(string argumentName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                return argumentName + "=" + WWW.EscapeURL(value).Replace("+", " ") + "&";
            }
        }
    }
}

#endif