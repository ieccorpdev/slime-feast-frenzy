using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PackUI : _BaseBehaviour
    {
        [SerializeField]
        protected PackConfig m_Config;
        [SerializeField]
        protected Button m_BtnBuy;

        [SerializeField] protected TMP_Text m_TxtPrice;

        private void Start()
        {
            if (m_Config == null) return;
            Services.IAP().SetReceiveProductCallback(m_Config.bundleId, isRestore =>
            {
                if (isRestore) return;
                m_Config.resources.ForEach(x =>
                {
                    PlayerData.hybridData.AddResource(x.type, x.value);
                });
            }); 
                
            m_BtnBuy.onClick.AddListener(() =>
            {
                Services.IAP().PurchaseProduct(m_Config.bundleId);
            });
        }

        protected void SetLocalizePrice()
        {
            
        }
    }
}