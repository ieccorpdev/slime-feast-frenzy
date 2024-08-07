#if (PluginUnityIAP && PluginReceiptVerify)
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace IEC
{
    public class ReceiptVerifyOffline : IReceiptVerifyService
    {
        public virtual void CheckReceipt(Product product, bool isRestore, Action<bool> callback)
        {
            callback(IsValidOffline(product));
        }

        protected bool IsValidOffline(Product product)
        {
            bool validPurchase = true;
            var productId = product.definition.id;

#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
            // Unity IAP's validation logic is only included on these platforms.
            if (Application.isMobilePlatform)
            {
                // Prepare the validator with the secrets we prepared in the Editor
                // obfuscation window.
                var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                    AppleTangle.Data(), Application.identifier);

                try
                {
                    // On Google Play, result has a single product ID.
                    // On Apple stores, receipts contain multiple products.
                    var result = validator.Validate(product.receipt);
                    // For informational purposes, we list the receipt(s)
                    Debug.Log("Receipt is valid. Contents:");

                    bool isTransactionValid = false;
                    foreach (IPurchaseReceipt productReceipt in result)
                    {
                        Debug.Log(productReceipt.productID);
                        Debug.Log(productReceipt.purchaseDate);
                        Debug.Log(productReceipt.transactionID);

                        if (product.definition.storeSpecificId == productReceipt.productID
                            && product.transactionID == productReceipt.transactionID)
                        {
                            isTransactionValid = true;
                        }
                    }

                    if (isTransactionValid == false)
                    {
                        Debug.LogError("Transaction is not valid");
                        throw new IAPSecurityException();
                    }
                }
                catch (IAPSecurityException)
                {
                    Debug.LogError("Invalid receipt, not unlocking content");
                    validPurchase = false;
                }
            }
#endif
            return validPurchase;
        }
    }
}
#endif