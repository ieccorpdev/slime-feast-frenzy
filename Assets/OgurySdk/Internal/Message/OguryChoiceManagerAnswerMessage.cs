using System;

namespace OgurySdk.Internal.Message
{
    [Serializable]
    public class OguryChoiceManagerAnswerMessage
    {
        public int instanceId;
        public bool debug;
        public int errorCode;
        public string answer;
    }
}