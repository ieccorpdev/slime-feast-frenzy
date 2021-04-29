using System;
using System.Collections.Generic;
using UnityEngine;
#if (PluginUnityIAP)
using UnityEngine.Purchasing;
#endif

namespace IEC
{
    public class InAppPurchaseMock : IInAppPurchaseService
    {
        IDataService m_DataService;
        Action<string, bool, bool> purchaseResultAction;
        Dictionary<string, Action<bool>> recieveProductActions = new Dictionary<string, Action<bool>>();

        public InAppPurchaseMock(IDataService dataService)
        {
            m_DataService = dataService;
        }

        public void ConnectInit(Action initializedCallback = null)
        {
        }

        public LocalizePriceInfo GetLocalizePrice(string productID, bool isNewFormat = true)
        {
            return null;
        }


        public bool IsInitialized()
        {
            return false;
            //throw new NotImplementedException();
        }

        public bool IsProductPurchased(string productID)
        {
            return m_DataService.GetBool(productID, false);
        }

        public bool IsProductSubscriptionActive(string productID)
        {
            return false;
        }

        public bool IsProductCanFreeTrialSubscription(string productID)
        {
            return false;
        }

        public void PurchaseProduct(string productID)
        {
            IapProductResult(productID, true, false);
        }

        void IapProductResult(string productID, bool isSuccess, bool isRestore)
        {
            if (recieveProductActions.ContainsKey(productID))
            {
                if (isSuccess) m_DataService.SetBool(productID, true);

                purchaseResultAction?.Invoke(productID, isSuccess, isRestore);
                if (isSuccess && recieveProductActions.TryGetValue(productID, out var action))
                {
                    action.Invoke(isRestore);
                }
            }
        }

        public void RestorePurchase(Action<bool> onRestoreDone)
        {
            onRestoreDone(true);
        }

#if (PluginUnityIAP)
        public SubscriptionInfo GetSubscriptionInfo(string productID)
        {
            return null;
        }
#endif

        public void SetPurchaseCallback(Action<string, bool, bool> callback)
        {
            purchaseResultAction = callback;
        }

        public void SetReceiveProductCallback(string productId, Action<bool> callback)
        {
            if (string.IsNullOrWhiteSpace(productId)) return;
            recieveProductActions[productId] = callback;
        }
    }
}

