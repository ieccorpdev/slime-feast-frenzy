using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IEC
{
    public class InAppPurchaseController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
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
                case EventList.NONE:
                    Debug.Log("event none");
                    break;
            }
        }

        public void BehaviourAwake()
        {
            Services.IAP().SetPurchaseCallback(PurchaseProductResult);

            //Set single receive callback for each IAP items from here, should call to single item script
            // UI.ScreenShopTest.SetCallbackIAP();
        }

        public void BehaviourStart()
        {

        }

        //Should place common IAP callback script here
        void PurchaseProductResult(string productId, bool isSuccess, bool isRestore)
        {
            Debug.LogWarningFormat("PurchaseProductResult {0} , {1} , {2}", productId, isSuccess, isRestore);

            //Should call hide loading IAP panel
            Locator.Service.PopupLoadingPurchaseIap.Hide();
            
            if (isSuccess)
            {
                //Should call common analytic tracking IAP
                Services.PushEvent(CustomIapAdjustName(productId));
                // Debug.LogError("PurchaseProductResult");
                // var listPayOut = IapUtilities.GetProductPayOuts(productId);
                // if (listPayOut != null)
                // {
                //     foreach (var payOutItem in listPayOut)
                //     {
                //         switch (payOutItem.type)
                //         {
                //             case ProductCatalogPayout.ProductCatalogPayoutType.Other:
                //                 if (payOutItem.subtype == "noads")
                //                 {
                //                     HandleNoAdsPayOut();
                //                 }
                //
                //                 break;
                //             case ProductCatalogPayout.ProductCatalogPayoutType.Currency:
                //                 if (productId != IapUtilities.PRODUCT_REMOVE_ADS_BUNDLE && isRestore)
                //                 {
                //                     continue;
                //                 }
                //             
                //                 var currencyType = IapUtilities.ConvertCurrencyType(payOutItem.subtype);
                //                 HandleCurrencyPayOut(currencyType, (int) payOutItem.quantity);
                //                 break;
                //         }
                //     }
                // }
                // //Should call common analytic tracking IAP
                if (isRestore)
                {
                    SetCallBackIAP(productId);
                    Locator.Main.HomeScreenTab.UpdateMainHome();
                }
                //this.RaiseEvent(EventList.GamePlay.IAP_SUCCESS_PRODUCT, productId);

            }
        }
        void SetCallBackIAP(string id)
        {
            PackConfig configIap = Config.GetConfigPackIap(id);
            if (configIap.isHaveNoAds)
            {
                Services.ActiveNoAds(true);
                Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false); //Disable banner Ads
                Controllers.Get<AdsController>().UpdateWhiteBannerSize();
            }

            if (id == "com.shark.frenzy.force.starter.pack")
            {
                PlayerData.isBoughtStarterPack = true;
                //PlayerData.timeEndStaredPack = DateTime.Now.AddMinutes(Game.timeSkipAdsStartPackage);
                //Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);//Disable banner Ads
                if(Locator.Main.HomeScreenTab) Locator.Main.HomeScreenTab.UpdateMainHome();
                Services.SaveData();
            }            
            if (id == "com.shark.frenzy.force.piggy.bank")
            {
                Locator.Main.PopupPiggyBank.BuySuccessPiggyBank();
            }
            if(!PlayerData.WasBuyIapPackNonConsume(configIap.bundleId))
            {
                var goldAdd = 0;
                //var listResourceType = configIap.resources.Select(x => x.type).ToList();
                foreach (var rsc in configIap.resources)
                {
                    PlayerData.AddCurrency(rsc.type,rsc.value);
                    if (rsc.type == ResourceType.Gold)
                    {
                        goldAdd += rsc.value;
                    }
                }
                if (IapUtilities.GetProductById(configIap.bundleId).type == ProductType.NonConsumable)
                {
                    PlayerData.AddBundleIAPPurchased(configIap.bundleId);
                }
            }
            //meomew need wait screen main shown then show effect, temporary skip
            //Locator.Main.ScreenMain.AddRewardClaimEffect(ResourceType.Gold, goldAdd);
        }
        private void HandleNoAdsPayOut()
        {
            //PlayerData.isNoAds = true;
            Pool.Get<ServiceData>().isNoAds = true;
            Services.ActiveNoAds(true);
            EventController.RaiseEvent(EventList.GamePlay.BUY_NO_ADS);
        }
        
        private void HandleCurrencyPayOut(ResourceType subtype, int number)
        {
            PlayerData.AddCurrency(subtype, number);
        }

        public string CustomIapAdjustName(string id)
        {
            string result = "iap_" + id;
            result = result.Replace(".", "_");
            return result;
        }
    }
}