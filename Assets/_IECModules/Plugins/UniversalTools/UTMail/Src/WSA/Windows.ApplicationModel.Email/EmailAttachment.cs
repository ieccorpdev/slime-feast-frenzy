#if !UNITY_EDITOR && UNITY_WSA && UNITY_WSA_8_1

namespace Windows.ApplicationModel.Email
{
    public sealed class EmailAttachment
    {
        public EmailAttachment(string fileName, Windows.Storage.Streams.RandomAccessStreamReference data)
        {
            this.fileName = fileName;
            this.data = data;
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        public Windows.Storage.Streams.RandomAccessStreamReference Data
        {
            get
            {
                return this.data;
            }
        }

        private string fileName;
        private Windows.Storage.Streams.RandomAccessStreamReference data;
    }
}

#endif