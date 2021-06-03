#if UNITY_STANDALONE_WIN || (UNITY_EDITOR_WIN && !UNITY_STANDALONE_OSX)

namespace UT
{
    public class MailImpl
    {
        public static void Compose(MailMessage message, string annotation)
        {
            if (message.IsBodyHtml)
            {
                UnityEngine.Debug.LogError("Windows MAPI doesn't support IsBodyHtml property when composing an email. The property will be ignored (though an email client may still interpret the body as HTML).\nSee https://social.msdn.microsoft.com/Forums/en-US/8125da13-7d79-4a5a-82a5-57c8a322be78/uwp10240c-emailmessage-with-emailmessagebodykindhtml");
            }

            try
            {
                using (var mapiMessage = ToMAPIMailMessage(message))
                {
                    mapiMessage.Send();
                }
            }
            catch (global::System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        private static MAPIWrapper.MailMessage ToMAPIMailMessage(MailMessage message)
        {
            var msg = new MAPIWrapper.MailMessage();

            if (message.To != null)
            {
                foreach (var it in message.To)
                {
                    msg.AddToAddress(it.Address);
                }
            }

            if (message.CC != null)
            {
                foreach (var it in message.CC)
                {
                    msg.AddCCAddress(it.Address);
                }
            }

            if (message.Bcc != null)
            {
                foreach (var it in message.Bcc)
                {
                    msg.AddBCCAddress(it.Address);
                }
            }

            if (!string.IsNullOrEmpty(message.Subject))
            {
                msg.Subject(message.Subject);
            }

            if (!string.IsNullOrEmpty(message.Body))
            {
                msg.Body(message.Body);
            }

            if (message.Attachments != null)
            {
                foreach (var it in UT.MailUtil.AttachmentsToFilePaths(message.Attachments))
                {
                    msg.AddAttachment(it);
                }
            }

            return msg;
        }
    }
}

#endif