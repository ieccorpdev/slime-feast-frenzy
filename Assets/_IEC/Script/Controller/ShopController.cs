using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ShopController : _BaseBehaviour, IController, IEventAwake, IEventListener
    {
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
            switch (gameEvent)
            {
                case EventList.GamePlay.SHOP_SELECT_ITEM:
                    var type = (ItemShopType)param.p0;
                    var index = (int)param.p1;
                    OnSelectItem(type, index);
                    break;
                case EventList.GamePlay.SHOP_BUY_ITEM:
                    var shopTab = (ItemShopType)param.p0;
                    var totalItem = (int)param.p1;
                    OnHandleBuyItem(shopTab, totalItem);
                    break;
            }
        }

        private void OnSelectItem(ItemShopType type, int index)
        {
            PlayerData.SelectShopItem(type, index);
            OnApplyItemShop();
        }

        private void OnHandleBuyItem(ItemShopType type, int totalItem)
        {
            if(type == ItemShopType.Theme)
            {
                var index = MathHelper.GetRandomIndexByList(PlayerData.listThemeUnlocked, totalItem);
                PlayerData.UnlockTheme(index);
            }else if(type == ItemShopType.Background)
            {
                var index = MathHelper.GetRandomIndexByList(PlayerData.listBackgroundUnlocked, totalItem);
                PlayerData.UnlockBackground(index);
            }

            OnApplyItemShop();
        }

        public void OnApplyItemShop()
        {
            UI.OnUpdateShopData();
        }
    }
}