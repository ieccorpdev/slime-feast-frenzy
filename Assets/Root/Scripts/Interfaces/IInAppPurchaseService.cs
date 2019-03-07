using System;
using System.Collections.Generic;

namespace Root
{
    public interface IInAppPurchaseService
    {
        void ConnectInit(Action initializedCallback = null);

        bool IsProductPurchased(string productID);

        void PurchaseProduct(string productID, Action<bool> callBack);

        void RestorePurchase(Dictionary<string, Action<bool>> callbacks, Action<bool> onRestoreDone);

        string GetLocalizePrice(string productID, bool isContainCurrencyCode);

        bool IsInitialized();
    }

    [Serializable]
    public class IapConfig
    {
        public string androidKey;
        public bool isAndroidStaticTest;
        public string androidStaticTestKey;
        public IapProduct[] products;
    }

    [Serializable]
    public class IapProduct
    {
        public string Id;
        public IapType Type;
        public string AndroidId;
        public string IosId;
    }

    public enum IapType
    {
        Consume,
        NonConsume,
        Subscription
    }
}

