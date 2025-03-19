using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class FreeAdsGoldPack : _BaseBehaviour
    {
        public int goldAmount = 100;
        public Button adsButton;
        
        public Transform adsGroup;
        public Transform collectedGroup;
        //public TextMeshProUGUI countdownText;

        public static bool IsAdsAvailable => PlayerData.lastTimeCollectFreeAdsGoldPack != DateTime.Today;

        private void Awake()
        {
            adsButton.onClick.AddListener(
                () =>
                {
                    Services.ShowRewardAds(() =>
                    {
                        Services.PushEvent(new TrackingEvent.SRApostition("shop_free_gold_pack"));
                        OnWatchAdsCollectGold();
                    });
                });
        }

        // Start is called before the first frame update
        void Start()
        {
            //gameObject.OnEvent(EventList.GamePlay.ON_CHOOSE_TAB, () => { });
        }

        // Update is called once per frame
        void Update()
        {
            CheckResetAdsReward();
        }

        public void OnWatchAdsCollectGold()
        {
            PlayerData.AddCurrency(ResourceType.Gold, goldAmount);
            // Locator.Main.ScreenMain.AddRewardToClaim(
            //     ResourceType.Gold, 
            //     Vector2.zero, 
            //     EffectRewardClaim.SwitchToRectTransform(Locator.Main.ScreenMain.goldCurrencyDisplay.GetComponent<RectTransform>(), Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
            //     goldAmount
            // );
            //Locator.Main.ScreenMain.AddRewardGoldToClaim(goldAmount);
            Locator.Main.ScreenMain.AddRewardClaimEffect(ResourceType.Gold, goldAmount);
            Locator.Main.ScreenMain.DoAnimAllAvailableReward();
            PlayerData.lastTimeCollectFreeAdsGoldPack = DateTime.Today;
            EventController.RaiseEvent(EventList.SAVE_DATA);
            
        }

        private void CheckResetAdsReward()
        {
            adsButton.interactable = IsAdsAvailable;
            
            adsGroup.gameObject.SetActive(IsAdsAvailable);
            collectedGroup.gameObject.SetActive(!IsAdsAvailable);
        }
    }
}
