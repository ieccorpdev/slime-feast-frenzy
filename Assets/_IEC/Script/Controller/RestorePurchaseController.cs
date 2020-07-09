using UnityEngine;

namespace IEC
{
    public class RestorePurchaseController : _BaseController, IController, IEventListener, IEventAwake, IEventStart
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
                    break;
            }
        }

        public void BehaviourAwake()
        {
            //UI.GroupPopup.PopupShop.InitRestoreCallbacks();
        }

        public void BehaviourStart()
        {
            //if (Game.isRestorePendingIAP)
            //{
            //    Services.IAP().SetRestoreCallbacks(UI.GroupPopup.PopupShop.dictRestoreIAP);
            //}
        }
    }
}