using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace IEC
{
    public class PackUI : ScreenBehaviour
    {
        [SerializeField]
        protected PackConfig m_Config;
        [SerializeField]
        protected Button m_BtnBuy;

        public bool isAmountDisplayContainWords = true;
        [SerializeField] 
        protected List<PackUIItemAmountDisplay> m_listItemDisplay = new();

        public Action OnPurchaseSuccess;
        public Action OnPurchaseFail;
        [SerializeField] protected TMP_Text m_TxtPrice, m_PackName;

        protected virtual void Awake()
        {
            gameObject.OnEvent(EventList.Service.INIT_IAP_DONE, SetLocalizePrice);
            gameObject.OnEvent(EventList.Service.UPDATE_IAP, SetLocalizePrice);
        }

        private void OnEnable()
        {
            SetLocalizePrice();
        }

        protected virtual void Start()
        {
            if (m_Config == null) return;
            Services.IAP().SetReceiveProductCallback(m_Config.bundleId, isRestore =>
            {
                if(isRestore) return;//Only run with buy directed, if restore, change to InAppPurchase Controller - > PurchaseProductResult
                if (m_Config.isHaveNoAds)
                {
                    Pool.Get<ServiceData>().isNoAds = true;
                    Services.ActiveNoAds(true);
                    EventController.RaiseEvent(EventList.GamePlay.BUY_NO_ADS);
                }
                UpdateUI();
                // if (isRestore) return;
                //var goldAdd = 0;
                var listResourceType = m_Config.resources.Select(x => x.type).ToList();
                m_Config.resources.ForEach(x =>
                {
                    PlayerData.hybridData.AddResource(x.type, x.value);
                    PlayerData.AddCurrency(x.type, x.value);
                    if (Locator.Main.ScreenMain != null)
                    {
                        Locator.Main.ScreenMain.AddRewardClaimEffect(x.type, x.value);
                    }
                });
                if (IapUtilities.GetProductById(m_Config.bundleId).type == ProductType.NonConsumable)
                {
                    PlayerData.AddBundleIAPPurchased(m_Config.bundleId);
                }

                Locator.Service.PopupPurchaseSuccess.SetResourceCallBack(listResourceType)
                    .AddActionClose(listResource =>
                    {
                        if (listResource.Contains(ResourceType.Gold))
                        {
                            // Locator.Service.MoneyPanel.EffectCoinClaim(new Vector2(350, -1050), goldAdd,
                            //     PlayerData.hybridData.GetResourceCount(ResourceType.Gold), () => { });
                        }
                        if (Locator.Main.ScreenMain != null)
                        {
                            Locator.Main.ScreenMain.DoAnimAllAvailableReward();
                        }
                    }).Show();
                Services.PushEvent(new IEC.TrackingEvent.BuySuccessIAP(m_Config.bundleId));
                OnPurchaseSuccess?.Invoke();
                Data.SaveData();
            }); 
                
            m_BtnBuy.onClick.AddListener(() =>
            {
                Locator.Service.PopupLoadingPurchaseIap.Show();
                Services.IAP().PurchaseProduct(m_Config.bundleId);
            });
            if(m_PackName) m_PackName.text = m_Config.packName;
            UpdateUI();
        }

        protected void SetLocalizePrice()
        {
            if(m_Config == null) return;
            var localizeInfo = Services.IAP().GetLocalizePrice(m_Config.bundleId);
            if (localizeInfo != null)
            {
                m_TxtPrice.text = $"{localizeInfo.LocalPrice} {localizeInfo.IsoCode}";
            }

        }

        public virtual void UpdateUI()
        {
            foreach (var itemDisplay in m_listItemDisplay)
            {
                int amount = m_Config.GetResourceValueByType(itemDisplay.resourceType);

                if (amount == -1)
                    continue;

                if (itemDisplay.amountText == null)
                    continue;

                string displayString = amount.ToString();
                    if(isAmountDisplayContainWords)
                    {
                        if (itemDisplay.resourceType == ResourceType.Live || itemDisplay.resourceType == ResourceType.UnlimitedLive)
                        {
                            displayString = (amount/60f) + "H";
                        } else if (itemDisplay.resourceType == ResourceType.Gold)
                        {
                            
                        }
                        else
                        {
                            displayString = "x" + displayString;
                        }
                    }

                    itemDisplay.amountText.text = displayString;
            }
            
        }
    }

    [System.Serializable]
    public class PackUIItemAmountDisplay
    {
        public ResourceType resourceType;
        public TextMeshProUGUI amountText;
    }
}