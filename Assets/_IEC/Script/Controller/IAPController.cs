using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IEC
{
    public class IAPController : _BaseBehaviour, IController, IEventListener
    {
        private const string NO_ADS = "com.gma.water.connect.puzzle.no.ads";

        public List<ProductCatalogItem> IapCatalog =>
    SingleCreateIfNotExist(() => ProductCatalog.LoadDefaultCatalog().allProducts.ToList());

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.IAP_BUY_PACKAGES:
                    var iapPackages = (IapPackages)param.p0;
                    OnClickPurchaseProduct(GetProductId(iapPackages));
                    break;
                case EventList.GamePlay.IAP_RESTORE_NO_ADS:
                    Restore();
                    break;
            }
        }

        private void OnClickPurchaseProduct(string productId)
        {
            UI.loadingObj.gameObject.SetActive(true);
            Services.IAP().PurchaseProduct(productId, success =>
            {
                UI.loadingObj.gameObject.SetActive(false);
                print($"Purchase {productId}: {success}");
                if (!success) return;
                PurchaseProductSuccess(productId);
                Services.PushEvent("iap_buy_packages",
                    IapCatalog.Find(item => item.id == productId).applePriceTier, new Dictionary<string, object>
                    {
                        {"level", PlayerData.level},
                        {"package", productId}
                    });
            });
        }

        private void PurchaseProductSuccess(string productId)
        {
            switch (productId)
            {
                case NO_ADS:
                    BuyNoAds();
                    break;
            }
        }

        private void BuyNoAds()
        {
            PlayerData.isNoAds = true;
            Services.ActiveNoAds(true);
            UI.mainUIGroup.SetActiveBtnNoAds();
            Services.PushEvent(new IEC.TrackingEvent.BuyNoAds());
        }

        private string GetProductId(IapPackages packages)
        {
            string productID = "";
            switch (packages)
            {
                case IapPackages.NoAds:
                    productID = NO_ADS;
                    break;
            }

            return productID;
        }

        private void Restore()
        {
            var actionRestoreNoAds = new Action<bool>((rs) =>
            {
                if (rs)
                {
                    BuyNoAds();
                }
            });

            var dictRestoreNoAds = new Dictionary<string, Action<bool>>();
            dictRestoreNoAds[NO_ADS] = actionRestoreNoAds;

            Services.IAP().RestorePurchase(dictRestoreNoAds, success => { print($"Restore Result: {success}"); });
        }

    }
}