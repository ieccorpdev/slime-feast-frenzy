using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PackUI : ScreenBehaviour
    {
        [SerializeField]
        protected PackConfig m_Config;
        [SerializeField]
        protected Button m_BtnBuy;

        [SerializeField] protected TMP_Text m_TxtPrice, m_PackName;

        protected virtual void Awake()
        {
            gameObject.OnEvent(EventList.Service.INIT_IAP_DONE, SetLocalizePrice);
        }

        protected virtual void Start()
        {
            if (m_Config == null) return;
            Services.IAP().SetReceiveProductCallback(m_Config.bundleId, isRestore =>
            {
                if (m_Config.isHaveNoAds)
                {
                    Pool.Get<ServiceData>().isNoAds = true;
                }
                UpdateUI();
                // if (isRestore) return;
                var goldAdd = 0;
                var listResourceType = m_Config.resources.Select(x => x.type).ToList();
                m_Config.resources.ForEach(x =>
                {
                    PlayerData.hybridData.AddResource(x.type, x.value);
                    if (x.type == ResourceType.Gold)
                    {
                        goldAdd = (int)x.value;
                    }
                });
                Locator.Service.PopupPurchaseSuccess.SetResourceCallBack(listResourceType)
                    .AddActionClose(listResource =>
                    {
                        if (listResource.Contains(ResourceType.Gold))
                        {
                            Locator.Service.MoneyPanel.EffectCoinClaim(new Vector2(350, -1050), goldAdd,
                                PlayerData.hybridData.GetResourceCount(ResourceType.Gold), () => { });
                        }
                    }).Show();
                
                Data.SaveData();
            }); 
                
            m_BtnBuy.onClick.AddListener(() =>
            {
                Services.IAP().PurchaseProduct(m_Config.bundleId);
            });
            m_PackName.text = m_Config.packName;
        }

        protected void SetLocalizePrice()
        {
            if(m_Config == null) return;
            var localizeInfo = Services.IAP().GetLocalizePrice(m_Config.bundleId);
            m_TxtPrice.text = $"{localizeInfo.LocalPrice} {localizeInfo.IsoCode}";
        }

        protected virtual void UpdateUI()
        {
            
        }
    }
}