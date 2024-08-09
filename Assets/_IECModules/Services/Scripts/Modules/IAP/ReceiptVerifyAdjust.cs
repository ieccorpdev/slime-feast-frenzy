#if (PluginUnityIAP && PluginReceiptVerify && PluginReceiptVerifyAdjust)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using com.adjust.sdk;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;

namespace IEC
{
    public class ReceiptVerifyAdjust : ReceiptVerifyOffline
    {
        private AdjustAnalyticModule adjustAnalyticModule;
        private IJsonService jsonConverter;
        private MonoBehaviour monoBehaviour;

        private ServicesController Services => Controllers.Get<ServicesController>();
        private string AdjustVerifyUrl => Services.config.IapAdjustVerifyUrl;

        [Serializable]
        public class UserDeviceInfo
        {
            public UserDeviceInfo()
            {
                deviceModel = SystemInfo.deviceModel;
                os = SystemInfo.operatingSystem;
            }

            public string deviceModel;
            public string os;
        }

        [Serializable]
        public class PurchaseReceipt
        {
            public string Store;
            public string TransactionID;
            public string Payload;
        }

        [Serializable]
        public class ReceiptPayload
        {
            public string json;
            public string signature;
            public string[] skuDetails;
        }

        [Serializable]
        public class ReceiptPayloadJson
        {
            public string orderId;
            public string packageName;
            public string productId;
            public string purchaseToken;
        }

        [Serializable]
        public class ReceiptData
        {
            public string packageName;
            public string productId;
            public string purchaseToken;
            public string signature;
        }

        [Serializable]
        public class AdjustData
        {
            public string idfa;
            public string event_token;
            public string app_token;
            public string adid;
            public string user_agent;
            public string environment;
            public int s2s;
            public string ip_address;
            public DateTime created_at;
            public float revenue;
            public string currency;
            public string callback_params;
            public string partner_params;
        }

        [Serializable]
        public class VerifyPaymentData
        {
            public ReceiptData receipt;
            public AdjustData adjust;
            // public string itemCode;
        }

        [Serializable]
        public class VerifyPaymentIosData
        {
            public string receipt;
            public AdjustData adjust;
            // public string itemCode;
        }

        [Serializable]
        public class Response
        {
            public string statusCode;
            public string messenger;
        }

        [Serializable]
        public class VerifyPaymentResponse : Response
        {
            public VerifyPaymentResponseData data;
        }

        [Serializable]
        public class VerifyPaymentResponseData
        {
            public string transactionId;
            public float amount;
            public int itemId;
        }

        public ReceiptVerifyAdjust(AdjustAnalyticModule adjustAnalyticModule, IJsonService jsonConverter, MonoBehaviour monoBehaviour)
        {
            this.adjustAnalyticModule = adjustAnalyticModule;
            this.jsonConverter = jsonConverter;
            this.monoBehaviour = monoBehaviour;
        }


        public override void CheckReceipt(Product product, bool isPurchasing, Action<bool> callback)
        {
            if (!isPurchasing)
            {
                callback.Invoke(true);
                return;
            }
            
#if UNITY_EDITOR
            // VerifyPayment(product, callback); // Enable this line to test in editor
            callback.Invoke(true);
            return;
#endif
            bool validPurchaseOffline = true;

            if (validPurchaseOffline)
            {
                VerifyPayment(product, callback);
            }
            else
            {
                callback(false);
            }
        }

        public void VerifyPayment(Product product, Action<bool> callback)
        {
            var receipt = product.receipt;
            PurchaseReceipt purchaseReceipt = jsonConverter.FromJson<PurchaseReceipt>(receipt);
            AdjustData adjust = GetAdjustData(product);

#if UNITY_EDITOR
            ReceiptData checkingObjectEditor = new ReceiptData()
            {
                packageName = UnityEditor.PlayerSettings.applicationIdentifier,
                productId = product.definition.id,
            };

            VerifyPayment(checkingObjectEditor, adjust, callback);
            return;
#endif
#if UNITY_ANDROID
            var payload = jsonConverter.FromJson<ReceiptPayload>(purchaseReceipt.Payload);
            var payloadJson = jsonConverter.FromJson<ReceiptPayloadJson>(payload.json);
            ReceiptData checkingObject = new ReceiptData()
            {
                packageName = payloadJson.packageName,
                productId = payloadJson.productId,
                purchaseToken = payloadJson.purchaseToken,
                signature = payload.signature
            };

            VerifyPayment(checkingObject, adjust, callback);
            return;
#endif
            VerifyPaymentIOS(purchaseReceipt.Payload, adjust, product, callback);
        }

        void VerifyPayment(ReceiptData receipt, AdjustData adjust, Action<bool> callback)
        {
            var dataCheck = new VerifyPaymentData()
            {
                receipt = receipt,
                adjust = adjust,
            };

            Debug.Log("Verify Adjust Data: " + jsonConverter.ToJson(dataCheck));

            if (!string.IsNullOrEmpty(AdjustVerifyUrl))
            {
                monoBehaviour.StartCoroutine(DoPostData<VerifyPaymentResponse>(AdjustVerifyUrl, jsonConverter.ToJson(dataCheck),
                    (res) =>
                    {
                        // bool isValid = VerifyPaymentHandle(res);
                        // callback.Invoke(isValid);
                    }));
            }
                
            callback.Invoke(true);
        }

        void VerifyPaymentIOS(string iosPayment, AdjustData adjust, UnityEngine.Purchasing.Product product,
            Action<bool> callback)
        {
            var dataCheck = new VerifyPaymentIosData()
            {
                receipt = iosPayment,
                adjust = adjust,
            };
            
            Debug.Log("Verify Adjust Data (ios): " + jsonConverter.ToJson(dataCheck));
            
            if (!string.IsNullOrEmpty(AdjustVerifyUrl))
            {
                monoBehaviour.StartCoroutine(DoPostData<VerifyPaymentResponse>(AdjustVerifyUrl, jsonConverter.ToJson(dataCheck),
                    (res) =>
                    {
                        // bool isValid = VerifyPaymentHandle(res);
                        // callback.Invoke(isValid);
                    }));
            }
                
            callback.Invoke(true);
        }

        IEnumerator DoPostData<T>(string url, string bodyJsonString, Action<T> callback) where T : class
        {
            using var request = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 10; // If request take more than 10s, it will be canceled
            SetRequestHeader(request);
            yield return request.SendWebRequest();
            T res = HandleResponse<T>(request);
            callback.Invoke(res);
        }

        T HandleResponse<T>(UnityWebRequest request) where T : class
        {
            Debug.Log($"Response {request.method}: {request.url}");

            if (request.responseCode == 201 || request.responseCode == 200)
            {
                Debug.Log("Success: " + request.downloadHandler.text);
                var text = request.downloadHandler.text;
                try
                {
                    var thing = jsonConverter.FromJson<T>(text);
                    return thing;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Invalid json response");
                    Debug.LogError(ex.ToString());
                    return null;
                }
            }

            Debug.LogError("Status Code: " + request.responseCode);
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);

            if (request.responseCode == 401)
            {
                Debug.LogError("Login FAIL");
                // PopupError.ShowLoginFail();
                return null;
            }

            return null;
        }

        bool VerifyPaymentHandle(VerifyPaymentResponse res)
        {
            if (res == null) return false;
            Debug.LogError("VerifyPaymentHandle " + jsonConverter.ToJson(res));
            return true;
        }

        void SetRequestHeader(UnityWebRequest request)
        {
            var headers = GetDataHeaders();
            foreach (var h in headers)
            {
                request.SetRequestHeader(h.Key, h.Value);
            }
        }

        Dictionary<string, string> GetDataHeaders()
        {
            var isDebug = Debug.unityLogger.filterLogType == LogType.Log;

            var headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" },
                // { "is_debug", isDebug.ToString().ToLower() },
            };

            return headers;
        }

        private AdjustData GetAdjustData(Product product)
        {
            var services = Controllers.Get<ServicesController>();
            var deviceInfo = new UserDeviceInfo();
            AdjustData adjust = new AdjustData
            {
                idfa = services.IsHaveIDFA ? services.UserIDFA : Adjust.getIdfa(),
                app_token = (Application.platform == RuntimePlatform.Android) ? services.config.adjustConfig.androidID : services.config.adjustConfig.iosID,
                event_token = adjustAnalyticModule.GetIAPEventId(),
                adid = Adjust.getAdid(),
                user_agent = services.Json().ToJson(deviceInfo),
                ip_address = GetLocalIPAddress(),
                created_at = DateTime.UtcNow,
                revenue = product == null ? 0 : InAppPurchaseController.GetProductPrice(product.definition.id),
                currency = "USD",
            };

            return adjust;
        }

        private string GetLocalIPAddress()
        {
            List<string> ipAddresses = new List<string>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork ||
                            ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            ipAddresses.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            // Return the first available IP address
            return ipAddresses.Count > 0 ? ipAddresses[0] : "";
        }
    }
}

#endif