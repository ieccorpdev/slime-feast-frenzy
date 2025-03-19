using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using UnityEngine;

public class UIShopHomeTab : ScreenNavigateUI
{
    public PackUI freeAds, freeAdsBundle, discountedBundle;

    private void Awake()
    {
        gameObject.OnEvent(EventList.GamePlay.BUY_NO_ADS, UpdateAdsPackDisplay);
    }

    private void Start()
    {
        UpdateAdsPackDisplay();
    }

    public void UpdateAdsPackDisplay()
    {
        freeAds.UpdateUI();
        freeAdsBundle.gameObject.SetActive(!PlayerData.isNoAds);
        discountedBundle.gameObject.SetActive(PlayerData.isNoAds);
    }
    
}
