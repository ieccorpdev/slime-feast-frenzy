using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IInAppPurchaseService
    {
        void ConnectInit(Action initializedCallback = null);

        bool IsProductPurchased(string productID);

        bool IsProductSubscriptionActive(string productID);

#if (PluginUnityIAP)
        UnityEngine.Purchasing.SubscriptionInfo GetSubscriptionInfo(string productID);
#endif
        bool IsProductCanFreeTrialSubscription(string productID);

        /// <summary>
        /// Action<string, bool, bool> => (productId, isSuccess, isRestorePurchase)
        /// </summary>
        /// <param name="callback"></param>
        void SetPurchaseCallback(Action<string, bool, bool> callback);


        /// <summary>
        /// Action<bool> isRestorePurchase
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="callback"></param>
        void SetReceiveProductCallback(string productId, Action<bool> callback);

        void PurchaseProduct(string productID);

        void RestorePurchase(Action<bool> onRestoreDone);

        LocalizePriceInfo GetLocalizePrice(string productID, bool isNewFormat = true);

        bool IsInitialized();

        decimal GetProductPrice(string id);

    }

    public class LocalizePriceInfo
    {
        public string LocalPrice;
        public string IsoCode;
        public string LocalPriceWithSymbol;
        public float LocalPriceValue;

        public LocalizePriceInfo(decimal price, string isoCode, string localPriceWithSymbol, bool isNewFormat)
        {
            if (isNewFormat)
            {
                this.LocalPrice = GetStringFormat((float)price);
            }
            else
            {
                this.LocalPrice = price.ToString();
            }

            this.IsoCode = isoCode;
            this.LocalPriceWithSymbol = localPriceWithSymbol;
            this.LocalPriceValue = (float)price;
        }

        static string GetStringFormat(float number)
        {
            int result = (int)((number - (int)number) * 100);
            if (result >= 10)
            {
                return number.ToString("N");
            }
            else
            {
                return ((int)number).ToString("N0");
            }
        }
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

