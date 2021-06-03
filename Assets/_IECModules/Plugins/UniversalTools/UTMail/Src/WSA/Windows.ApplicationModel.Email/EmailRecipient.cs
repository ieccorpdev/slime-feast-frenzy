#if !UNITY_EDITOR && UNITY_WSA && UNITY_WSA_8_1

namespace Windows.ApplicationModel.Email
{
    public sealed class EmailRecipient
    {
        public EmailRecipient(string address)
        {
            this.address = address;
        }

        public string Address
        {
            get
            {
                return this.address;
            }
        }

        public string Name
        {
            get
            {
                if (this.address != null && this.address.Contains("@"))
                {
                    return this.address.Substring(0, this.address.IndexOf('@'));
                }
                else
                {
                    return this.address;
                }
            }
        }

        private string address;
    }
}

#endif