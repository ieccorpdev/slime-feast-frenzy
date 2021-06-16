#if !UNITY_EDITOR && UNITY_WSA && UNITY_WSA_8_1

namespace Windows.ApplicationModel.Email
{
    using MailAddressCollection = global::System.Collections.Generic.List<EmailRecipient>;
    using EmailAttachmentCollection = global::System.Collections.Generic.List<EmailAttachment>;

    public sealed class EmailMessage
    {
        public MailAddressCollection To
        {
            get
            {
                return this.to;
            }
        }

        public MailAddressCollection CC
        {
            get
            {
                return this.cc;
            }
        }

        public MailAddressCollection Bcc
        {
            get
            {
                return this.bcc;
            }
        }

        public string Subject
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public EmailAttachmentCollection Attachments
        {
            get
            {
                return this.attachments;
            }
        }

        private readonly MailAddressCollection to = new MailAddressCollection();
        private readonly MailAddressCollection cc = new MailAddressCollection();
        private readonly MailAddressCollection bcc = new MailAddressCollection();
        private readonly EmailAttachmentCollection attachments = new EmailAttachmentCollection();
    }
}

#endif