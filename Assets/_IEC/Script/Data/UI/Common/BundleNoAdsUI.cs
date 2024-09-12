using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BundleNoAdsUI : PackUI
    {
        [SerializeField] private GameObject m_Purchased;

        protected override void Start()
        {
            base.Start();
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            m_Purchased.SetActive(PlayerData.isNoAds);
            m_BtnBuy.gameObject.SetActive(!PlayerData.isNoAds);
        }
    }
}