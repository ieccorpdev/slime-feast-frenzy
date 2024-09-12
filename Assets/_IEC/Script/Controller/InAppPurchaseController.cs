using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            if (isSuccess)
            {
                //Should call common analytic tracking IAP
            }
        }
    }
}