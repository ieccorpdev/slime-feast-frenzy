using System;
using System.Runtime.InteropServices;
using UnityEngine;

// ReSharper disable UseNullPropagation
// ReSharper disable ClassNeverInstantiated.Global

namespace OgurySdk
{
    public class OguryChoiceManager
    {
#if UNITY_ANDROID
        private static AndroidJavaObject _choiceManager = null;
        private static AndroidJavaObject ChoiceManager =>
            _choiceManager ?? (_choiceManager =
                new AndroidJavaObject("com.ogury.unity.cm.OguryChoiceManager"));
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void ogury_choiceManager_updateOguryCmConfig(string config);

        [DllImport("__Internal")]
        private static extern void ogury_choiceManager_ask();

        [DllImport("__Internal")]
        private static extern void ogury_choiceManager_edit();

        [DllImport("__Internal")]
        private static extern bool ogury_choiceManager_editAvailable();

        [DllImport("__Internal")]
        private static extern bool ogury_choiceManager_gdprApplies();
        
        [DllImport("__Internal")]
        private static extern bool ogury_choiceManager_ccpaApplies();

        [DllImport("__Internal")]
        private static extern bool ogury_choiceManager_hasPaid();
#endif

        public static event ChoiceManagerCompleteEventHandler OnAskComplete;
        public static event ChoiceManagerErrorEventHandler OnAskError;
        public static event ChoiceManagerCompleteEventHandler OnEditComplete;
        public static event ChoiceManagerErrorEventHandler OnEditError;

        public delegate void ChoiceManagerCompleteEventHandler(Answer answer);

        public delegate void ChoiceManagerErrorEventHandler(OguryError error);

        public static void UpdateOguryCmConfig(OguryCmConfig config)
        {
            SetupCallbacks();

            string sConfig = JsonUtility.ToJson(config);
#if UNITY_EDITOR
            // No-op
#elif UNITY_ANDROID
            ChoiceManager.Call("updateOguryCmConfig", sConfig);
#elif UNITY_IOS
            ogury_choiceManager_updateOguryCmConfig(sConfig);
#endif
        }

        public static void Ask()
        {
            SetupCallbacks();
#if UNITY_EDITOR
            if (OnAskComplete != null)
            {
                OnAskComplete(Answer.Refusal);
            }
#elif UNITY_ANDROID
            ChoiceManager.Call("ask");
#elif UNITY_IOS
            ogury_choiceManager_ask();
#endif
        }

        public static void Edit()
        {
            SetupCallbacks();
#if UNITY_EDITOR
            if (OnEditComplete != null)
            {
                OnEditComplete(Answer.Refusal);
            }
#elif UNITY_ANDROID
            ChoiceManager.Call("edit");
#elif UNITY_IOS
            ogury_choiceManager_edit();
#endif
        }

        public static bool EditAvailable
        {
            get
            {
#if UNITY_EDITOR
                return true;
#elif UNITY_ANDROID
                return ChoiceManager.Call<bool>("isEditAvailable");
#elif UNITY_IOS
                return ogury_choiceManager_editAvailable();
#endif
            }
        }

        public static bool GdprApplies
        {
            get
            {
#if UNITY_EDITOR
                return true;
#elif UNITY_ANDROID
                return ChoiceManager.Call<bool>("gdprApplies");
#elif UNITY_IOS
                return ogury_choiceManager_gdprApplies();
#endif
            }
        }

        public static bool CcpaApplies
        {
            get
            {
#if UNITY_EDITOR
                return true;
#elif UNITY_ANDROID
                return ChoiceManager.Call<bool>("ccpaApplies");
#elif UNITY_IOS
                return ogury_choiceManager_ccpaApplies();
#endif
            }
        }

        public static bool HasPaid
        {
            get
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManager.Call<bool>("hasPaid");
#elif UNITY_IOS
                return ogury_choiceManager_hasPaid();
#endif
            }
        }

        private static bool _areCallbacksSetup = false;

        private static void SetupCallbacks()
        {
            if (_areCallbacksSetup || !OguryCallbacks.CheckIfPresent())
            {
                return;
            }

            _areCallbacksSetup = true;

            OguryCallbacks.Instance.OnAskComplete += answer =>
            {
                if (OnAskComplete != null)
                {
                    OnAskComplete.Invoke(answer);
                }
            };
            OguryCallbacks.Instance.OnAskError += (instanceId, error) =>
            {
                if (OnAskError != null)
                {
                    OnAskError.Invoke(error);
                }
            };
            OguryCallbacks.Instance.OnEditComplete += answer =>
            {
                if (OnEditComplete != null)
                {
                    OnEditComplete.Invoke(answer);
                }
            };
            OguryCallbacks.Instance.OnEditError += (instanceId, error) =>
            {
                if (OnEditError != null)
                {
                    OnEditError.Invoke(error);
                }
            };
        }

        public enum Answer
        {
            FullApproval,
            PartialApproval,
            Refusal,
            SaleAllowed,
            SaleDenied,
            NoAnswer
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
            private static extern string ogury_choiceManagerTcfV2_iabString();

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isAccepted(int vendorId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isPurposeAccepted(int purposeId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isVendorAndItsPurposesAccepted(int vendorId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isSpecialFeatureAccepted(int vendorId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isLiVendorMet(int vendorId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isLiPurposeMet(int purposeId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isVendorsLiAndLiPurposeMet(int vendorId);

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerTcfV2_isAllVendorConditionsMet(int vendorId);
#endif

            public static string IabString
            {
                get
                {
#if UNITY_EDITOR
                    return "";
#elif UNITY_ANDROID
                    return ChoiceManagerTcfV2.Call<string>("getIABString");
#elif UNITY_IOS
                    return ogury_choiceManagerTcfV2_iabString();
#endif
                }
            }

            public static bool IsAccepted(int vendorId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isAccepted", vendorId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isAccepted(vendorId);
#endif
            }

            public static bool IsPurposeAccepted(int purposeId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isPurposeAccepted", purposeId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isPurposeAccepted(purposeId);
#endif
            }

            public static bool IsVendorAndItsPurposesAccepted(int vendorId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isVendorAndItsPurposesAccepted", vendorId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isVendorAndItsPurposesAccepted(vendorId);
#endif
            }

            public static bool IsSpecialFeatureAccepted(int specialFeatures)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isSpecialFeatureAccepted", specialFeatures);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isSpecialFeatureAccepted(specialFeatures);
#endif
            }

            public static bool IsLiVendorMet(int vendorId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isLiVendorMet", vendorId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isLiVendorMet(vendorId);
#endif
            }

            public static bool IsLiPurposeMet(int purposeId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isLiPurposeMet", purposeId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isLiPurposeMet(purposeId);
#endif
            }

            public static bool IsVendorsLiAndLiPurposeMet(int vendorId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isVendorsLiAndLiPurposeMet", vendorId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isVendorsLiAndLiPurposeMet(vendorId);
#endif
            }

            public static bool IsAllVendorConditionsMet(int vendorId)
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return ChoiceManagerTcfV2.Call<bool>("isAllVendorConditionsMet", vendorId);
#elif UNITY_IOS
                return ogury_choiceManagerTcfV2_isAllVendorConditionsMet(vendorId);
#endif
            }

            public class Purpose
            {
                public static readonly int StoreInformation = 2;
                public static readonly int SelectBasicAds = 4;
                public static readonly int CreatePersonalisedAd = 8;
                public static readonly int SelectPersonalisedAd = 16;
                public static readonly int CreatePersonalisedContent = 32;
                public static readonly int SelectPersonalisedContent = 64;
                public static readonly int MeasureAdPerformance = 128;
                public static readonly int MeasureContentPerformance = 256;
                public static readonly int MarketResearch = 512;
                public static readonly int DevelopAndImproveProducts = 1024;
            }

            public class SpecialFeature
            {
                public static readonly int PreciseGeolocation = 1;
                public static readonly int ScanDeviceCharacteristics = 2;
            }
        }

        public class CcpafV1
        {
#if UNITY_ANDROID
            private static AndroidJavaObject _choiceManagerCcpafV1 = null;
            private static AndroidJavaObject ChoiceManagerCcpafV1 =>
                _choiceManagerCcpafV1 ?? (_choiceManagerCcpafV1 =
                    new AndroidJavaObject("com.ogury.unity.cm.OguryChoiceManagerCcpafV1"));
#endif

#if UNITY_IOS
            [DllImport("__Internal")]
            private static extern string ogury_choiceManagerCcpafV1_uspString();

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerCcpafV1_hasSeenNotice();

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerCcpafV1_isOptOutSale();

            [DllImport("__Internal")]
            private static extern bool ogury_choiceManagerCcpafV1_isLspa();
#endif

            public static string UspString
            {
                get
                {
#if UNITY_EDITOR
                    return "";
#elif UNITY_ANDROID
                    return ChoiceManagerCcpafV1.Call<string>("getUSPString");
#elif UNITY_IOS
                    return ogury_choiceManagerCcpafV1_uspString();
#endif
                }
            }

            public static bool HasSeenNotice
            {
                get
                {
#if UNITY_EDITOR
                    return false;
#elif UNITY_ANDROID
                    return ChoiceManagerCcpafV1.Call<bool>("hasSeenNotice");
#elif UNITY_IOS
                    return ogury_choiceManagerCcpafV1_hasSeenNotice();
#endif
                }
            }

            public static bool IsOptOutSale
            {
                get
                {
#if UNITY_EDITOR
                    return false;
#elif UNITY_ANDROID
                    return ChoiceManagerCcpafV1.Call<bool>("isOptOutSale");
#elif UNITY_IOS
                    return ogury_choiceManagerCcpafV1_isOptOutSale();
#endif
                }
            }

            public static bool IsLspa
            {
                get
                {
#if UNITY_EDITOR
                    return false;
#elif UNITY_ANDROID
                    return ChoiceManagerCcpafV1.Call<bool>("isLspa");
#elif UNITY_IOS
                    return ogury_choiceManagerCcpafV1_isLspa();
#endif
                }
            }
        }
    }
}