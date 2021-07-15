using System;
#if !UNITY_EDITOR && UNITY_WSA
using AttachmentCollection = global::System.Collections.Generic.IList<UT.System.Net.Mail.Attachment>;
using MailAddress = Windows.ApplicationModel.Email.EmailRecipient;
using MailAddressCollection = global::System.Collections.Generic.ICollection<Windows.ApplicationModel.Email.EmailRecipient>;
#else
using AttachmentCollection = UT.System.Net.Mail.AttachmentCollection;
using MailAddress = UT.System.Net.Mail.MailAddress;
using MailAddressCollection = UT.System.Net.Mail.MailAddressCollection;
#endif

namespace UT
{
    /// <summary>
    /// Describes an email message to be composed or sent directly.
    /// </summary>
    /// <remarks>
    /// Implements IDisposable, so you have to either call Dispose() or wrap MailMessages with using. F.e., see UTMailSample.
    /// </remarks>
    public class MailMessage : UT.System.Net.Mail.MailMessage, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UT.MailMessage"/> class.
        /// </summary>
        public MailMessage()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UT.MailMessage"/> class.
        /// </summary>
        /// <param name="mailMessage">Mail message to copy from.</param>
        public MailMessage(MailMessage mailMessage)
            : this()
        {
            CopyCollection(this.To, mailMessage.To);
            CopyCollection(this.CC, mailMessage.CC);
            CopyCollection(this.Bcc, mailMessage.Bcc);
            this.Subject = mailMessage.Subject;
            this.Body = mailMessage.Body;
            this.IsBodyHtml = mailMessage.IsBodyHtml;
            CopyCollection(this.Attachments, mailMessage.Attachments);
        }

        /// <summary>
        /// Adds "To" recipient.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="emailAddress">Email address.</param>
        public MailMessage AddTo(string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                To.Add(new MailAddress(emailAddress));
            }

            return this;
        }

        /// <summary>
        /// A collection of "To" recipients.
        /// </summary>
        public new MailAddressCollection To
        {
            get
            {
                return base.To;
            }
        }

        /// <summary>
        /// Adds "Cc" recipient ("send as copy").
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="emailAddress">Email address.</param>
        /// <remarks>
        /// macOS email composing API doesn't support specifying "Cc" recipients - emailAddress will be added as "To" when composing on macOS.
        /// </remarks>
        public MailMessage AddCC(string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                CC.Add(new MailAddress(emailAddress));
            }

            return this;
        }

        /// <summary>
        /// A collection of "To" recipients.
        /// </summary>
        /// <remarks>
        /// macOS email composing API doesn't support specifying "Cc" recipients - they will be added as "To" when composing on macOS.
        /// </remarks>
        public new MailAddressCollection CC
        {
            get
            {
                return base.CC;
            }
        }

        /// <summary>
        /// Adds "Bcc" recipient ("send as hidden copy").
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="emailAddress">Email address.</param>
        /// <remarks>
        /// macOS email composing API doesn't support specifying "Bcc" recipients - emailAddress will be added as "To" when composing on macOS.
        /// </remarks>
        public MailMessage AddBcc(string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                Bcc.Add(new MailAddress(emailAddress));
            }

            return this;
        }

        /// <summary>
        /// A collection of "To" recipients.
        /// </summary>
        /// <remarks>
        /// macOS email composing API doesn't support specifying "Bcc" recipients - they will be added as "To" when composing on macOS.
        /// </remarks>
        public new MailAddressCollection Bcc
        {
            get
            {
                return base.Bcc;
            }
        }

        /// <summary>
        /// Sets the email message subject.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="subject">Subject.</param>
        public MailMessage SetSubject(string subject)
        {
            Subject = subject;
            return this;
        }

        /// <summary>
        /// Gets or sets email message subject.
        /// </summary>
        /// <value>Subject.</value>
        public new string Subject
        {
            get
            {
                return base.Subject;
            }

            set
            {
                base.Subject = value;
            }
        }

        /// <summary>
        /// Sets the email message body.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="body">Body. May contain HTML tags if IsBodyHtml is enabled (not supported in Windows and Windows Store builds).</param>
        public MailMessage SetBody(string body)
        {
            Body = body;
            return this;
        }

        /// <summary>
        /// Gets or sets email message body.
        /// </summary>
        /// <value>Body.</value>
        public new string Body
        {
            get
            {
                return base.Body;
            }

            set
            {
                base.Body = value;
            }
        }

        /// <summary>
        /// Specifies whether Body should be interpreted as HTML.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="isBodyHtml">Specifies whether HTML is enabled or not.</param>
        /// <remarks>
        /// Not supported in Windows and Windows Store builds. For more details please see:
        ///  https://support.microsoft.com/en-us/help/268440/info-mapi-is-not-suitable-for-html-messages
        /// and
        ///  https://social.msdn.microsoft.com/Forums/en-US/8125da13-7d79-4a5a-82a5-57c8a322be78/uwp10240c-emailmessage-with-emailmessagebodykindhtml
        /// </remarks>
        public MailMessage SetBodyHtml(bool isBodyHtml)
        {
            IsBodyHtml = isBodyHtml;
            return this;
        }

        /// <summary>
        /// Specifies whether Body should be interpreted as HTML.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <value>Specifies whether HTML is enabled or not.</value>
        /// <remarks>
        /// Not supported in Windows and Windows Store builds. For more details please see:
        ///  https://support.microsoft.com/en-us/help/268440/info-mapi-is-not-suitable-for-html-messages
        /// and
        ///  https://social.msdn.microsoft.com/Forums/en-US/8125da13-7d79-4a5a-82a5-57c8a322be78/uwp10240c-emailmessage-with-emailmessagebodykindhtml
        /// </remarks>
        public new bool IsBodyHtml
        {
            get
            {
                return base.IsBodyHtml;
            }

            set
            {
                base.IsBodyHtml = value;
            }
        }

        /// <summary>
        /// Adds an attachment from a file specified by full path.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="attachmentPath">A full path of a file to attach.</param>
        public MailMessage AddAttachment(string attachmentPath)
        {
            Attachments.Add(new UT.System.Net.Mail.Attachment(attachmentPath));
            return this;
        }

        /// <summary>
        /// Adds an attachment from System.IO.Stream.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="stream">System.IO.Stream to write as an attachment.</param>
        /// <param name="fileName">The attachment file name to be displayed for recipients.</param>
        public MailMessage AddAttachment(global::System.IO.Stream stream, string fileName)
        {
            Attachments.Add(new UT.System.Net.Mail.Attachment(stream, fileName));
            return this;
        }

        /// <summary>
        /// Adds an attachment from a byte buffer, i.e. byte[].
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="byteBuffer">A byte buffer to write as an attachment.</param>
        /// <param name="fileName">The attachment file name to be displayed for recipients.</param>
        public MailMessage AddAttachment(byte[] byteBuffer, string fileName)
        {
            return AddAttachment(new global::System.IO.MemoryStream(byteBuffer), fileName);
        }

        /// <summary>
        /// Adds an attachment from text.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="text">A text to write as an attachment.</param>
        /// <param name="targetEncoding">Specifies the target attachment file encoding.</param>
        /// <param name="fileName">The attachment file name to be displayed for recipients.</param>
        public MailMessage AddAttachment(string text, global::System.Text.Encoding targetEncoding, string fileName)
        {
            return AddAttachment(targetEncoding.GetBytes(text), fileName);
        }

        /// <summary>
        /// Adds an attachment from text.
        /// </summary>
        /// <returns>The same MailMessage object for convenient chain initialization.</returns>
        /// <param name="text">A text to write as an attachment.</param>
        /// <param name="fileName">The attachment file name to be displayed for recipients.</param>
        /// <remarks>
        /// The target attachment file encoding will be UTF-8.
        /// </remarks>
        public MailMessage AddAttachment(string text, string fileName)
        {
            return AddAttachment(text, global::System.Text.Encoding.UTF8, fileName);
        }

        /// <summary>
        /// A collection of all attachments.
        /// </summary>
        /// <value>The attachments.</value>
        public new AttachmentCollection Attachments
        {
            get
            {
                return base.Attachments;
            }
        }

        private static void CopyCollection<T>(global::System.Collections.Generic.ICollection<T> copyTo, global::System.Collections.Generic.ICollection<T> copyFrom)
        {
            copyTo.Clear();        
            foreach (var it in copyFrom)
            {
                if (it is IDisposable)
                {
                    copyTo.Add((T)Activator.CreateInstance(typeof(T), new object[] { it }));
                }
                else
                {
                    copyTo.Add(it);
                }
            }
        }
    }
}