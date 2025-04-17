using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using UnityEngine;
using UnityEngine.UI;

public class UIShopHomeTab : ScreenNavigateUI
{
    public PackUI freeAds, freeAdsBundle;
    public VerticalLayoutGroup verticalLayoutGroup;
    private void Awake()
    {
        gameObject.OnEvent(EventList.GamePlay.BUY_NO_ADS, UpdateAdsPackDisplay);
    }

    private void Start()
    {
        UpdateAdsPackDisplay();
    }

    public void OnEnable()
    {
        UpdateAdsPackDisplay();
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());
    }

    public void UpdateAdsPackDisplay()
    {
        freeAds.UpdateUI();
        freeAdsBundle.gameObject.SetActive(!PlayerData.isNoAds);
        //discountedBundle.gameObject.SetActive(PlayerData.isNoAds);
    }
    
}
