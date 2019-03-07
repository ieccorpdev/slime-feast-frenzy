using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
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

        public string GetLocalizePrice(string productID, bool isContainCurrencyCode)
        {
            return string.Empty;
            //throw new NotImplementedException();
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
    }
}

