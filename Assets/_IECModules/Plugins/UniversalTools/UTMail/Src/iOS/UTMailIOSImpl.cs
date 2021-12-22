#if !UNITY_EDITOR && UNITY_IOS

using System.Runtime.InteropServices;

namespace UT
{
    public class MailImpl
    {
        public static void Compose(MailMessage message, string annotation)
        {
            try
            {
                string[] to = MailUtil.MailAddressCollectionToStrings(message.To);
                string[] cc = MailUtil.MailAddressCollectionToStrings(message.CC);
                string[] bcc = MailUtil.MailAddressCollectionToStrings(message.Bcc);
                string[] attachments = MailUtil.AttachmentsToFilePaths(message.Attachments);

                _UT_ComposeEmail(to, to != null ? to.Length : 0, cc, cc != null ? cc.Length : 0, bcc, bcc != null ? bcc.Length : 0, message.Subject, message.Body, message.IsBodyHtml, attachments, attachments != null ? attachments.Length : 0);
            }
            catch (global::System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        [DllImport("__Internal")]
        private static extern void _UT_ComposeEmail(string[] to, int lengthTo, string[] cc, int lengthCc, string[] bcc, int lengthBcc, string subject, string body, bool htmlBody, string[] attachments, int lengthAttachments);
    }
}

#endif