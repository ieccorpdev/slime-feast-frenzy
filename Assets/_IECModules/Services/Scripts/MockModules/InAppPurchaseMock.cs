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

        public void PurchaseProduct(string productID, Action<bool> callBack)
        {
            m_DataService.SetBool(productID, true);
            callBack(true);
        }

        public void RestorePurchase(Dictionary<string, Action<bool>> callbacks, Action<bool> onRestoreDone)
        {
            foreach (var item in callbacks)
            {
                m_DataService.SetBool(item.Key, true);
                item.Value(true);
            }
            onRestoreDone(true);
        }

#if (PluginUnityIAP)
        public SubscriptionInfo GetSubscriptionInfo(string productID)
        {
            return null;
        }
#endif
    }
}

