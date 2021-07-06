using System.Runtime.InteropServices;
using UnityEngine;
// ReSharper disable ClassNeverInstantiated.Global

namespace OgurySdk
{
    public class OguryChoiceManagerExternal
    {

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