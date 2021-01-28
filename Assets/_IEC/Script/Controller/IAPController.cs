using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class IAPController : _BaseBehaviour, IController, IEventAwake, IEventListener
    {

        public const string NO_ADS = "com.gma.water.connect.puzzle.no.ads";

        public GameObject loadingIap;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            
        }


        public void OnEventRaise(string gameEvent, EventParam param)
        {
            
        }

        public void OnClickPurchaseProduct(string productId)
        {
            loadingIap.gameObject.SetActive(true);

            //Services.IAP().PurchaseProduct(productId, success =>
            //{
            //    loadingIap.gameObject.SetActive(false);
            //    print($"Purchase {productId}: {success}");
            //    if (!success) return;
            //    PurchaseProductSuccess(productId);
            //    ServicesController.PushEvent("iap_buy_packages",
            //        IapCatalog.Find(item => item.id == productId).applePriceTier, GameController.DefaultLevelParam.Concat(new Dictionary<string, object>
            //        {
            //        {"package", productId}
            //        }).ToDictionary(x => x.Key, x => x.Value));
            //});
        }

    }
}