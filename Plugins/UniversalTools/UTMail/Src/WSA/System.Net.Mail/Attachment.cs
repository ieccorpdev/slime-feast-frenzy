#if !UNITY_EDITOR && UNITY_WSA

using System;
using System.Threading.Tasks;

namespace UT.System.Net.Mail
{
    public class Attachment
    {
        public Attachment(string attachmentPath)
        {
            this.attachmentPath = attachmentPath;
        }

        public Attachment(global::System.IO.Stream stream, string fileName)
            : this(MailUtil.StreamToFilePath(stream, fileName))
        {
        }

        public async Task<Windows.ApplicationModel.Email.EmailAttachment> EmailAttachment()
        {
            if (this.emailAttachment == null)
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(this.attachmentPath);
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(storageFile);
                this.emailAttachment = new Windows.ApplicationModel.Email.EmailAttachment(global::System.IO.Path.GetFileName(attachmentPath), stream);
            }

            return this.emailAttachment;
        }

        private readonly string attachmentPath;
        private Windows.ApplicationModel.Email.EmailAttachment emailAttachment;
    }
}

#endif