using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupHardLevel : BasePopup
    {
        public Button allBgBtn;
        public Button normalCloseBtn;
        public Button btnPlay, btnSkip, btnSkipAds;

        public TextMeshProUGUI rewardText;

        protected override void Awake()
        {
            base.Awake();
            SetEvent(allBgBtn, OnHideClick);
            
            
            btnSkipAds.gameObject.SetActive(false);
        }

        private void OnSkip()
        {
            Hide();
            PlayerData.lastHardLevel = PlayerData.level;
            //PlayerData.isPlayLevelHardFirstTime = true;
            PlayerData.currentCountLoseHardLevel = 0;
            PlayerData.currentHardLevel++;
            Locator.Main.HomeScreenTab.UpdateMainHome();
        }

        public override void Hide()
        {
            base.Hide();
            
        }

        public override void Show()
        {
            base.Show();
            SetupData();
        }

        public void SetupData()
        {
            var loseBeforeAllowSkip = Game.isOverrideHardLevelConfig
                ? Game.loseBeforeAllowSkipHardLevelOverride
                : PlayConfig.loseBeforeAllowSkipHardLevel;
            btnSkip.gameObject.SetActive(PlayerData.currentCountLoseHardLevel >= loseBeforeAllowSkip);
            rewardText.text = "x" + PlayConfig.amountGoldRewardMultiply;

            if (Game.isAllowSkipHardLevelAds)
            {
                btnSkipAds.gameObject.SetActive(true);
                btnSkip.gameObject.SetActive(false);
            }
        }
    }
}
