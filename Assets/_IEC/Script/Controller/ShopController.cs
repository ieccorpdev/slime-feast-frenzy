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
            if(PlayerData.SelectShopItem(type, index))
            {
                OnApplyItemShop(type);
            }
        }

        private void OnHandleBuyItem(ItemShopType type, int totalItem)
        {
            var index = 0;
            if(type == ItemShopType.Theme)
            {
                index = MathHelper.GetRandomIndexByList(PlayerData.listThemeUnlocked, totalItem);
                PlayerData.UnlockTheme(index);
            }
            else if(type == ItemShopType.Background)
            {
                index = MathHelper.GetRandomIndexByList(PlayerData.listBackgroundUnlocked, totalItem);
                PlayerData.UnlockBackground(index);
            }

            OnApplyItemShop(type);
            UI.popupShop.EffectBuyItem(type, index);
        }

        public void OnApplyItemShop(ItemShopType type)
        {
            UI.OnUpdateShopData();

            if(type == ItemShopType.Theme)
            {
                LevelController.SetupTheme(true);
            }
            else if(type == ItemShopType.Background)
            {
                GamePlay.SetBackground();
            }
        }
    }
}