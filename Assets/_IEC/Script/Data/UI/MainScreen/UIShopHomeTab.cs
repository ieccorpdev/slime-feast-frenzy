using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        SortVerticalGroups();
    }

    public void SortVerticalGroups()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            // Force the layout rebuild for the parent of the VerticalLayoutGroup
            LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());
        });
    }
    public void UpdateAdsPackDisplay()
    {
        freeAds.UpdateUI();
        freeAdsBundle.gameObject.SetActive(!PlayerData.isNoAds);
        //discountedBundle.gameObject.SetActive(PlayerData.isNoAds);
    }
    
}
