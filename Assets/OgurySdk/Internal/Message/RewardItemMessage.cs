using System;

namespace OgurySdk.Internal.Message
{
    [Serializable]
    public class RewardItemMessage
    {
        public int instanceId;
        public bool debug;
        public string name;
        public string value;
    }
}