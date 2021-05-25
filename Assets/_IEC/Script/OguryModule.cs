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
        _thumbnailAd = new OguryThumbnailAd("b63dcc40-9e8c-0139-4726-0242ac120004", "IOS_AD_UNIT_ID");
        _thumbnailAd.OnAdLoaded += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdLoaded");
            _thumbnailAd.Show();
        };
        _thumbnailAd.OnAdClosed += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdClosed");
            StartCoroutine(IE_LoadAd(0f));
            Controllers.Get<ServicesController>().SetActiveBanner(AdsEnum.BANNER_BOTTOM,true);
        };
        _thumbnailAd.OnAdDisplayed += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdDisplayed");
            Controllers.Get<ServicesController>().SetActiveBanner(AdsEnum.BANNER_BOTTOM,false);
        };
        _thumbnailAd.OnAdError += (ad, error) =>
        {
            Debug.LogWarning("Thumbnail OnAdError:" + error.Description);
            StartCoroutine(IE_LoadAd(1f));
        };
        _thumbnailAd.OnAdNotAvailable += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdNotAvailable");
        };
        _thumbnailAd.OnAdNotLoaded += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdNotLoaded");
        };
        
        Ogury.StartAds();
        Debug.LogWarning("Start Ogury");
        StartCoroutine(IE_LoadAd(0f));
    }

    IEnumerator IE_LoadAd(float time)
    {
        if (time <= 0)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        _thumbnailAd.Load();
        Debug.LogWarning("Load Ad");
    }
}