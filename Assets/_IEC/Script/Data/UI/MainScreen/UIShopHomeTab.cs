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
    public PackUI freeAds, freeAdsBundle, starterPack;
    public VerticalLayoutGroup verticalLayoutGroup;
    private void Awake()
    {
        gameObject.OnEvent(EventList.GamePlay.BUY_NO_ADS, UpdateAdsPackDisplay);
        gameObject.OnEvent(EventList.GamePlay.BUY_STARTER_PACK, UpdateStarterPackDisplay);
    }

    private void Start()
    {
        UpdateAdsPackDisplay();
        UpdateStarterPackDisplay();
    }

    public void OnEnable()
    {
        UpdateAdsPackDisplay();
        UpdateStarterPackDisplay();
        RebuildLayout();
    }

    private void RebuildLayout()
    {
        StartCoroutine(SortVerticalGroups());
    }

    public IEnumerator SortVerticalGroups()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());
    }
    public void UpdateAdsPackDisplay()
    {
        freeAds.UpdateUI();
        freeAdsBundle.gameObject.SetActive(!PlayerData.isNoAds);
        RebuildLayout();

        //discountedBundle.gameObject.SetActive(PlayerData.isNoAds);
    }

    public void UpdateStarterPackDisplay()
    {
        starterPack.gameObject.SetActive(!PlayerData.isBoughtStarterPack);
        RebuildLayout();
    }
    
}
