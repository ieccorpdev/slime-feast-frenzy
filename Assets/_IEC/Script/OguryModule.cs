using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OgurySdk;
using IEC;

public class OguryModule : MonoBehaviour
{
    private OguryThumbnailAd _thumbnailAd;
    private void Start()
    {
        Ogury.Start("OGY-8B8F812E7137", "IOS_ASSET_KEY");
        _thumbnailAd = new OguryThumbnailAd("b63dcc40-9e8c-0139-4726-0242ac120004_test", "IOS_AD_UNIT_ID");
        _thumbnailAd.OnAdLoaded += ad =>
        {
            Debug.LogWarning("On thumbnail loaded");
            _thumbnailAd.Show(OguryThumbnailAdGravity.BottomLeft, 0, 0); 
        };
        _thumbnailAd.OnAdClosed += ad =>
        {
            Debug.LogWarning("On thumbnail closed");
            Controllers.Get<ServicesController>().SetActiveBanner(AdsEnum.BANNER_BOTTOM, true);
            StartCoroutine(IE_LoadAd(0));
        };
        _thumbnailAd.OnAdDisplayed += ad =>
        {
            Debug.LogWarning("On thumbnail displayed");
            Controllers.Get<ServicesController>().SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);
        };
        _thumbnailAd.OnAdError += (ad, error) =>
        {
            Debug.LogWarning("On thumbnail error:" + error.Description);
            StartCoroutine(IE_LoadAd(1f));
        };
        _thumbnailAd.OnAdNotLoaded += ad =>
        {
            Debug.LogWarning("On thumbnail not loaded");
        };
        _thumbnailAd.OnAdNotAvailable += ad =>
        {
            Debug.LogWarning("On thumbnail not available");
        };
        Ogury.StartAds();
        StartCoroutine(IE_LoadAd(0));
        Debug.LogWarning("Start Ogury");
    }

    IEnumerator IE_LoadAd(float timeDelay)
    {
        if (timeDelay <= 0)
        {
            yield return null;
        }
        else
        {
            var wait = new WaitForSeconds(timeDelay);
            yield return wait;
        }
        _thumbnailAd.Load();
        Debug.LogWarning("Load Thumbnail");
    }
}