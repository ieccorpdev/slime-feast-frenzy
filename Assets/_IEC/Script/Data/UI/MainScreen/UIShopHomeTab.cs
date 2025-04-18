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
    
}
