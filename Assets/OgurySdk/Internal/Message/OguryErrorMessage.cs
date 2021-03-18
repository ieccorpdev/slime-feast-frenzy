using System;

namespace OgurySdk.Internal.Message
{
    [Serializable]
    public class OguryErrorMessage
    {
        public int instanceId;
        public bool debug;
        public int errorCode;
        public string description;
    }
}