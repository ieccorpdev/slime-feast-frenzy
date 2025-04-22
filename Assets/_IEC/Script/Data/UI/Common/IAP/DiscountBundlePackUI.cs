using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class DiscountBundlePackUI : PackUI
    {
        [SerializeField] private GameObject m_Purchased;

        protected override void Awake()
        {
            base.Awake();
            OnPurchaseSuccess += () =>
            {
                PlayerData.isDiscountedPackBought = true;
                UpdateUI();
            };
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            if(m_Purchased != null)
                m_Purchased.SetActive(PlayerData.isDiscountedPackBought);
            m_BtnBuy.gameObject.SetActive(!PlayerData.isDiscountedPackBought);
        }
    }
}
