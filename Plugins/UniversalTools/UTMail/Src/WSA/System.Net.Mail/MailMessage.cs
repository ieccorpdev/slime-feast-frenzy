#if !UNITY_EDITOR && UNITY_WSA

using System.Threading.Tasks;
using AttachmentCollection = global::System.Collections.Generic.IList<UT.System.Net.Mail.Attachment>;
using MailAddress = Windows.ApplicationModel.Email.EmailRecipient;
using MailAddressCollection = global::System.Collections.Generic.ICollection<Windows.ApplicationModel.Email.EmailRecipient>;

namespace UT.System.Net.Mail
{
    public class MailMessage : global::System.IDisposable
    {
        public MailMessage()
        {
            this.emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
        }

        public MailAddressCollection To
        {
            get
            {
                return this.emailMessage.To;
            }
        }

        public MailAddressCollection CC
        {
            get
            {
                return this.emailMessage.CC;
            }
        }

        public MailAddressCollection Bcc
        {
            get
            {
                return this.emailMessage.Bcc;
            }
        }

        public string Subject
        {
            get
            {
                return this.emailMessage.Subject;
            }

            set
            {
                this.emailMessage.Subject = value;
            }
        }

        public string Body
        {
            get
            {
                return this.emailMessage.Body;
            }

            set
            {
                this.emailMessage.Body = value;
            }
        }

        public bool IsBodyHtml
        {
            get;
            set;
        }

        public AttachmentCollection Attachments
        {
            get
            {
                return this.attachments;
            }
        }

        public async Task<Windows.ApplicationModel.Email.EmailMessage> EmailMessage()
        {
            // Setup underlying emailMessage's Attachments
            this.emailMessage.Attachments.Clear();
            foreach (var it in Attachments)
            {
                this.emailMessage.Attachments.Add(await it.EmailAttachment());
            }

            return this.emailMessage;
        }

        // Windows.ApplicationModel.Email.EmailMessage is not IDisposable, so no need to actually Dispose on WSA.
        public void Dispose()
        {
        }

        private readonly global::System.Collections.Generic.List<Attachment> attachments = new global::System.Collections.Generic.List<Attachment>();
        private readonly Windows.ApplicationModel.Email.EmailMessage emailMessage;
    } 
}

#endif // !UNITY_EDITOR && UNITY_WSA