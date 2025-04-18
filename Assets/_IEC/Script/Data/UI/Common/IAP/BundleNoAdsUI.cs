using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BundleNoAdsUI : PackUI
    {
        [SerializeField] private GameObject m_Purchased;

        protected override void Awake()
        {
            base.Awake();
            if(m_Purchased != null)
                m_Purchased.SetActive(false);
        }


        protected override void Start()
        {
            base.Start();
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            if(m_Purchased != null)
                m_Purchased.SetActive(PlayerData.isNoAds);
            m_BtnBuy.gameObject.SetActive(!PlayerData.isNoAds);
            
        }
        
    }
}