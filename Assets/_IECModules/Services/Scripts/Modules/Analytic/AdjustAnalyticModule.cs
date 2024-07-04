#if (PluginAdjust)

using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using UnityEngine;

namespace IEC
{
    public class AdjustAnalyticModule : IAnalyticEvent
    {
        public bool IsDisable { get; set; }

        public Dictionary<string, string> DictEvent;
        private List<string> m_ListEventRev = new List<string>();

        public const string ADJUST_SANDBOX = "ADJUST_SAND_BOX";

        public void Init()
        {

        }

        public void InitAdjust(string androidID, string iosID)
        {
            var appID = (Application.platform == RuntimePlatform.Android) ? androidID : iosID;
            Debug.LogWarning("Adjust AppID: " + (string.IsNullOrWhiteSpace(appID) ? "null" : appID));

            var isEnableSandBox = PlayerPrefs.GetInt(ADJUST_SANDBOX, 0) == 1;
            
            var adjustConfig = new AdjustConfig(appID, (isEnableSandBox) ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production, true);
#if UNITY_ANDROID
            adjustConfig.setPreinstallTrackingEnabled(true);
#endif
            adjustConfig.setLogLevel(AdjustLogLevel.Suppress);
            adjustConfig.setSendInBackground(false);
            adjustConfig.setEventBufferingEnabled(false);
            adjustConfig.setLaunchDeferredDeeplink(false);
            Adjust.start(adjustConfig);
            EnableThirdPartySharing();
            Debug.LogWarning("Adjust Version:" + Adjust.getSdkVersion());
        }

        public void SetListEventRev(List<string> listEventRev)
        {
            m_ListEventRev = listEventRev;
        }


        public void PushCachedEvents()
        {
            if (IsDisable) return;
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null, UnityEngine.Purchasing.Product product = null)
        {
            if (IsDisable || DictEvent == null || !DictEvent.ContainsKey(eventName)) return;
            var adjustEvent = new AdjustEvent(DictEvent[eventName]);
            var adjustLog = $"Push Adjust Event:{eventName}__{DictEvent[eventName]}";
            if (m_ListEventRev != null && m_ListEventRev.Contains(eventName))
            {
                adjustEvent.setRevenue(value, "USD");
                adjustLog += $"__Rev:{value}";
            }
            if (product != null)
            {
                AdjustVerifyIAP.SetupIAPEvent(adjustEvent,product);
            }
            Adjust.trackEvent(adjustEvent);
            Debug.Log(adjustLog);
        }

        public void SetUserProperty(string name, string property)
        {
            if (IsDisable) return;
        }

        public void PushEventFullAd()
        {
            return;
            if (IsDisable) return;
        }

        public void PushEventRewardAd()
        {
            return;
            if (IsDisable) return;
        }

        public void EnableThirdPartySharing()
        {
            AdjustThirdPartySharing adjustThirdPartySharing = new AdjustThirdPartySharing(true);
            Adjust.trackThirdPartySharing(adjustThirdPartySharing);
            Debug.LogWarning("Adjust enable third party sharing");
        }


    }

    public class AdjustVerifyIAP
    {
        [System.Serializable]
        public class PurchaseReceipt
        {
            public string Store;
            public string TransactionID;
            public string Payload;
        }
        [System.Serializable]
        public class ReceiptPayload
        {
            public string json;
            public string signature;
            public string[] skuDetails;
        }
        [System.Serializable]
        public class ReceiptPayloadJson
        {
            public string orderId;
            public string packageName;
            public string productId;
            public string purchaseToken;
        }

        public static void SetupIAPEvent(AdjustEvent adjustEvent, UnityEngine.Purchasing.Product product)
        {
            var receipt = product.receipt;
            PurchaseReceipt purchaseReceipt = JsonUtility.FromJson<PurchaseReceipt>(receipt);
            var payload = JsonUtility.FromJson<ReceiptPayload>(purchaseReceipt.Payload);
            var payloadJson = JsonUtility.FromJson<ReceiptPayloadJson>(payload.json);

            adjustEvent.setProductId(payloadJson.productId); // Android & iOS
#if UNITY_IOS
            adjustEvent.setTransactionId(purchaseReceipt.TransactionID); // iOS only
            adjustEvent.setReceipt(receipt); // iOS only
#endif
#if UNITY_ANDROID
            adjustEvent.setPurchaseToken(payloadJson.purchaseToken); // Android only
#endif
        }
    }
}

#endif
