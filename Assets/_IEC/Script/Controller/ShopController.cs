using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ShopController : _BaseBehaviour, IController, IEventListener, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            GamePlay.SetBackground();
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
                    var isByAds = (bool)param.p0;
                    var shopTab = (ItemShopType)param.p1;
                    var totalItem = (int)param.p2;
                    OnHandleBuyItem(isByAds, shopTab, totalItem);
                    break;
                case EventList.GamePlay.SHOP_ADD_GOLD:
                    Services.ShowRewardAds(() => {
                        Game.timeLeftAddGold = 300f;
                        Services.PushEvent(new IEC.TrackingEvent.ShowRewardAddGoldInShop());
                        PlayerData.AddGold(200);
                        UI.EffectCoinClaim(200, PlayerData.gold, () => {
                            UI.OnUpdateShopData();
                        });
                    });
                    break;
                case EventList.GamePlay.END_GAME_GET_REWARD:
                    var isX5 = (bool)param.p0;
                    var coinAdd = Game.goldReward;
                    if (isX5)
                    {
                        coinAdd *= 5;
                    }
                    PlayerData.AddGold(coinAdd);
                    UI.EffectCoinClaim(coinAdd, PlayerData.gold, () => {
                        EventController.RaiseEventDelay(.2f, EventList.GamePlay.GAME_RESTART);
                    });
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

        private void OnHandleBuyItem(bool isByAds, ItemShopType type, int totalItem)
        {
            if (isByAds)
            {
                Services.ShowRewardAds(() => {
                    Services.PushEvent(new IEC.TrackingEvent.ShowRewardBuyShopItem());
                    OnBuyItem(type, totalItem);
                });
            }
            else
            {
                var gold = GetGoldItem(type);
                if (PlayerData.gold >= gold)
                {
                    PlayerData.AddGold(-gold);
                    Services.PushEvent(new IEC.TrackingEvent.BuyShopItemByGold());
                    OnBuyItem(type, totalItem);
                }
            }
        }

        public static int GetGoldItem(ItemShopType type)
        {
            if(type == ItemShopType.Theme)
            {
                return 1000;
            }
            else
            {
                return 500;
            }
        }

        private void OnBuyItem(ItemShopType type, int totalItem)
        {
            var index = 0;
            if (type == ItemShopType.Theme)
            {
                index = MathHelper.GetRandomIndexByList(PlayerData.listThemeUnlocked, totalItem);
                PlayerData.UnlockTheme(index);
            }
            else if (type == ItemShopType.Background)
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