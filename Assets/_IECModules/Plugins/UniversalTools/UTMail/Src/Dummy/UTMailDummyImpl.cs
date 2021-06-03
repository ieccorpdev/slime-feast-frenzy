#if (UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS && !UNITY_WSA)) && !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX && !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN

using UnityEngine;

namespace UT
{
    public class MailImpl
    {
        public static void Compose(MailMessage message, string annotation)
        {
            NotSupported("Composing");
        }

        public static void NotSupported(string feature = null)
        {
            if (feature == null)
            {
                Debug.LogWarning("UTMail: not supported on this platform");
            }
            else
            {
                Debug.LogWarning("UTMail: " + feature + " feature is not supported on this platform");
            }
        }
    }
}

#endif