#if (PluginUnityIAP)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace IEC
{
    public class IapUnityModule : IInAppPurchaseService, IStoreListener
    {
        IapConfig config;
        IDataService dataService;

        Dictionary<string, Action<bool>> purchaseCallbacks;
        Dictionary<string, Action<bool>> restoreCallbacks;
        Action<bool> onRestoreDone;
        Action initializedCallback;
        bool isHaveProductRestore = false;

        private IStoreController controller;
        private IExtensionProvider extensions;
        IAppleExtensions m_AppleExtensions;

        Dictionary<string, string> introductory_info_dict;

        private IReceiptVerifyService receiptVerify;

        const string STATIC_TEST = "staticTest";

        Action<string> m_LogIapOpen;
        Action<string> m_LogSuccess1;
        Action<string> m_LogSuccess2;
        Action<string> m_LogFail;

        class PurchaseReceipt
        {
            public string Store;
            public string TransactionID;
            public string Payload;
        }

        class VerifyResult
        {
            public int status;
        }

        public IapUnityModule(IapConfig config, IDataService dataService, IReceiptVerifyService receiptVerify)
        {
            this.config = config;
            this.dataService = dataService;
            this.receiptVerify = receiptVerify;
            purchaseCallbacks = new Dictionary<string, Action<bool>>();
        }

        public void SetCallbackAnalytic(Action<string> logIapOpen, Action<string> logPurchaseSuccess1, Action<string> logPurchaseSuccess2, Action<string> logPurchaseFail)
        {
            m_LogIapOpen = logIapOpen;
            m_LogSuccess1 = logPurchaseSuccess1;
            m_LogSuccess2 = logPurchaseSuccess2;
            m_LogFail = logPurchaseFail;
        }

        void SafeInvoke(Action<string> action, string param)
        {
            if (action != null)
            {
                action.Invoke(param);
            }
        }

        public void ConnectInit(Action initializedCallback = null)
        {
            this.initializedCallback = initializedCallback;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            if (config.isAndroidStaticTest && Application.platform == RuntimePlatform.Android)
            {
                builder.AddProduct(STATIC_TEST, ProductType.Consumable, new IDs
                    {
                        { config.androidStaticTestKey, GooglePlay.Name },
                        { config.androidStaticTestKey, MacAppStore.Name }
                    });
            }
            else
            {
                for (int i = 0; i < config.products.Length; i++)
                {
                    var product = config.products[i];
                    ProductType type = ProductType.Consumable;
                    type = (product.Type == IapType.NonConsume) ? ProductType.NonConsumable : type;
                    type = (product.Type == IapType.Subscription) ? ProductType.Subscription : type;

                    builder.AddProduct(product.Id, type, new IDs
                        {
                            { product.AndroidId, GooglePlay.Name },
                            { product.IosId, AppleAppStore.Name }
                        });
                }
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsProductPurchased(string productID)
        {
            return dataService.GetBool(productID, false);
        }

        public void PurchaseProduct(string productID, Action<bool> callBack)
        {
            purchaseCallbacks[productID] = callBack;
            SafeInvoke(m_LogIapOpen, productID);
            BuyProductID(productID);
        }

        void PurchaseProductResult(string productID, bool result)
        {
            if (purchaseCallbacks.ContainsKey(productID))
            {
                purchaseCallbacks[productID](result);
                purchaseCallbacks.Remove(productID);
                if (result == true)
                    dataService.SetBool(productID, result);
            }
        }

        public bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return controller != null && extensions != null;
        }

        void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product;
                if (config.isAndroidStaticTest)
                {
                    product = controller.products.WithID(STATIC_TEST);
                }
                else
                {
                    product = controller.products.WithID(productId);
                }

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    controller.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    PurchaseProductResult(productId, false);
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
                PurchaseProductResult(productId, false);
            }
        }

        public void RestorePurchase(Dictionary<string, Action<bool>> callbacks, Action<bool> onRestoreDone)
        {
            restoreCallbacks = callbacks;
            this.onRestoreDone = onRestoreDone;
            isHaveProductRestore = false;
            Restore();
        }

        void Restore()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                onRestoreDone(false);
                return;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                foreach (string productId in restoreCallbacks.Keys)
                {
                    var product = controller.products.WithID(productId);
                    if (product != null && product.hasReceipt && !IsProductPurchased(productId))
                    {
                        restoreCallbacks[productId](true);
                        dataService.SetBool(productId, true);
                        isHaveProductRestore = true;
                    }
                    else
                    {
                        restoreCallbacks[productId](false);
                    }
                }
                onRestoreDone(isHaveProductRestore);
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = extensions.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                    {
                        // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                        // no purchases are available to be restored.
                        Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                        onRestoreDone(result);
                    });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
                onRestoreDone(false);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("Unity IAP Module Init failed:" + error.ToString());
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var productId = args.purchasedProduct.definition.id;
            SafeInvoke(m_LogSuccess1, productId);

            receiptVerify.CheckReceipt(new IapProduct(args.purchasedProduct), isValid =>
            {
                if (isValid)
                {
                    // Unlock the appropriate content here.
                    if (purchaseCallbacks.ContainsKey(productId))
                    {
                        SafeInvoke(m_LogSuccess2, productId);
                        PurchaseProductResult(productId, true);
                    }
                    else if (restoreCallbacks != null && restoreCallbacks.ContainsKey(productId))
                    {
                        isHaveProductRestore = true;
                        restoreCallbacks[productId](true);
                    }
                    else
                    {
                        Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", productId));
                    }
                }
                else
                {
                    Debug.Log(string.Format("ProcessPurchase: FAIL. Not valid puchase of product: '{0}'", productId));
                }
            });

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            var productId = i.definition.id;
            SafeInvoke(m_LogFail, productId);
            PurchaseProductResult(productId, false);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
            if (initializedCallback != null)
            {
                initializedCallback.Invoke();
            }
        }

        //public LocalizePriceInfo GetLocalizePrice(string productID, bool isNewFormat = true)
        //{
        //    var product = controller.products.WithID(productID);
        //    LocalizePriceInfo info = null;
        //    if (product != null)
        //    {
        //        var metadata = product.metadata;
        //        info = new LocalizePriceInfo
        //        {
        //            LocalPrice = metadata.localizedPrice.ToString(),
        //            LocalPriceWithSymbol = metadata.localizedPriceString,
        //            IsoCode = metadata.isoCurrencyCode
        //        };
        //    }
        //    return info;
        //}

        public LocalizePriceInfo GetLocalizePrice(string productID, bool isNewFormat = true)
        {
            var product = controller.products.WithID(productID);
            LocalizePriceInfo info = null;
            if (product != null)
            {
                var metadata = product.metadata;
                info = new LocalizePriceInfo(
                    metadata.localizedPrice,
                    metadata.isoCurrencyCode,
                    metadata.localizedPriceString,
                    isNewFormat);

            }
            return info;
        }

        public bool IsProductSubscriptionActive(string productID)
        {
            if (!IsInitialized())
            {
                return false;
            }
            var item = controller.products.WithID(productID);

            if (item.receipt != null)
            {
                if (item.definition.type == ProductType.Subscription)
                {
                    if (CheckIfProductIsAvailableForSubscriptionManager(item.receipt))
                    {
                        string intro_json = (introductory_info_dict == null
                            || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
                        SubscriptionInfo info = p.getSubscriptionInfo();
                        Debug.Log("product id is: " + info.getProductId());
                        Debug.Log("purchase date is: " + info.getPurchaseDate());
                        Debug.Log("subscription next billing date is: " + info.getExpireDate());
                        Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                        Debug.Log("is expired? " + info.isExpired().ToString());
                        Debug.Log("is cancelled? " + info.isCancelled());
                        Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                        Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                        Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                        Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                        Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                        Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                        Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());

                        return info.isSubscribed() == Result.True;
                    }
                    else
                    {
                        Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                    }
                }
                else
                {
                    Debug.Log("the product is not a subscription product");
                }
            }
            else
            {
                Debug.Log("the product should have a valid receipt");
            }

            return false;
        }

        private bool CheckIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }

        public bool IsProductCanFreeTrialSubscription(string productID)
        {
            if (!IsInitialized())
            {
                return false;
            }
            var item = controller.products.WithID(productID);

            Debug.Log("product : " + productID);
            if (item.definition.type == ProductType.Subscription)
            {
                Debug.Log("hasReceipt : " + item.hasReceipt);
                return !item.hasReceipt;
            }
            else
            {
                Debug.Log("the product is not a subscription product");
            }

            return false;
        }

        public SubscriptionInfo GetSubscriptionInfo(string productID)
        {
            if (!IsInitialized())
            {
                return null;
            }
            var item = controller.products.WithID(productID);

            if (item.receipt != null)
            {
                if (item.definition.type == ProductType.Subscription)
                {
                    if (CheckIfProductIsAvailableForSubscriptionManager(item.receipt))
                    {
                        string intro_json = (introductory_info_dict == null
                            || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
                        SubscriptionInfo info = p.getSubscriptionInfo();
                        Debug.Log("product id is: " + info.getProductId());
                        Debug.Log("purchase date is: " + info.getPurchaseDate());
                        Debug.Log("subscription next billing date is: " + info.getExpireDate());
                        Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                        Debug.Log("is expired? " + info.isExpired().ToString());
                        Debug.Log("is cancelled? " + info.isCancelled());
                        Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                        Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                        Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                        Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                        Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                        Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                        Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());

                        return info;
                    }
                    else
                    {
                        Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                    }
                }
                else
                {
                    Debug.Log("the product is not a subscription product");
                }
            }
            else
            {
                Debug.Log("the product should have a valid receipt");
            }

            return null;
        }
    }
}
#endif