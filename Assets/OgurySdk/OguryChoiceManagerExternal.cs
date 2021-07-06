using System;
using System.Runtime.InteropServices;
using UnityEngine;
// ReSharper disable ClassNeverInstantiated.Global

namespace OgurySdk
{
    public class OguryChoiceManagerExternal
    {
#if UNITY_ANDROID
        private static AndroidJavaObject _choiceManager = null;
        private static AndroidJavaObject ChoiceManager =>
            _choiceManager ?? (_choiceManager =
                new AndroidJavaObject("com.ogury.unity.cm.OguryChoiceManager"));
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void ogury_choiceManager_setConsent(bool consentBoolean, string consentManagerName);
#endif

        public static void SetConsent(bool booleanConsent, string consentManagerName)
        {
#if UNITY_EDITOR
            // No-op
#elif UNITY_ANDROID
                ChoiceManager.Call("setConsent", booleanConsent, consentManagerName);
#elif UNITY_IOS
                ogury_choiceManager_setConsent(booleanConsent, consentManagerName);
#endif
        }
        
        public class TcfV2
        {
#if UNITY_ANDROID
            private static AndroidJavaObject _choiceManagerTcfV2 = null;
            private static AndroidJavaObject ChoiceManagerTcfV2 =>
                _choiceManagerTcfV2 ?? (_choiceManagerTcfV2 =
                    new AndroidJavaObject("com.ogury.unity.cm.OguryChoiceManagerTcfV2"));
#endif
            
#if UNITY_IOS
            [DllImport("__Internal")]
            private static extern void ogury_choiceManagerTcfV2_setConsent(string iabString, 
                int nonIabVendorIdsAcceptedCount, int[] nonIabVendorIdsAccepted);
#endif

            public static void SetConsent(string iabString)
            {
#pragma warning disable 618
                SetConsent(iabString, new int[0]);
#pragma warning restore 618
            }

            [Obsolete("The second parameter is deprecated and will be ignored, replace by SetConsent(string iabString).")]
            public static void SetConsent(string iabString, int[] nonIabVendorIdsAccepted)
            {
#if UNITY_EDITOR
                // No-op
#elif UNITY_ANDROID
                ChoiceManagerTcfV2.Call("setConsent", iabString, nonIabVendorIdsAccepted);
#elif UNITY_IOS
                var nonIabVendorIdsAcceptedCount = 0;
                if (nonIabVendorIdsAccepted != null)
                {
                    nonIabVendorIdsAcceptedCount = nonIabVendorIdsAccepted.Length;
                }

                ogury_choiceManagerTcfV2_setConsent(iabString, nonIabVendorIdsAcceptedCount, nonIabVendorIdsAccepted);
#endif
            }
        }
    }
}