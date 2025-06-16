using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupInsufficientCharacterShard : BasePopup
    {
        public Button shopButton;
        public Button shardShopButton;
        public Button playButton;

        private void Start()
        {
            SetEvent(shopButton, OnShopButtonClick);
            SetEvent(shardShopButton, OnShardShopButtonClick);
            SetEvent(playButton, OnPlayButtonClick);
        }

        private void OnShopButtonClick()
        {
            Hide();
            if (Locator.Main.PopupCharacterList.gameObject.activeInHierarchy)
            {
                Locator.Main.PopupCharacterList.Hide();
            }
            Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Shop).onClick?.Invoke();
        }

        private void OnShardShopButtonClick()
        {
            Hide();
            if (Locator.Main.PopupCharacterList.gameObject.activeInHierarchy)
            {
                Locator.Main.PopupCharacterList.Hide();
            }
            this.RaiseEvent(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, CharacterTabType.Shard);
        }

        private void OnPlayButtonClick()
        {
            Hide();
            if (Locator.Main.PopupCharacterList.gameObject.activeInHierarchy)
            {
                Locator.Main.PopupCharacterList.Hide();
            }
            Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Home).onClick?.Invoke();
            Locator.Main.PopupBooster.Show();
        }
    }
}
