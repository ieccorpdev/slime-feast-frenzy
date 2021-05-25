using System;
using System.Collections;
using System.Collections.Generic;
using _IECModules;
using UnityEngine;
using OgurySdk;
using IEC;

public class OguryModule : MonoBehaviour
{
    private OguryThumbnailAd _thumbnailAd;
    private int marginX;
    private int marginY;

    public void Init()
    {
        marginX = (DisplayMetricsAndroid.XDPI > 0) ? ((int) DisplayMetricsAndroid.XDPI / 2) : ((int)Screen.dpi / 2) - 90;
        Debug.LogWarning("Margin X:" + marginX);
        marginY = 0;
        Ogury.Start("OGY-8B8F812E7137", "IOS_ASSET_KEY");
        _thumbnailAd = new OguryThumbnailAd("b63dcc40-9e8c-0139-4726-0242ac120004_test", "IOS_AD_UNIT_ID");
        _thumbnailAd.OnAdLoaded += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdLoaded");
            _thumbnailAd.Show(OguryThumbnailAdGravity.BottomLeft, marginX, marginY);
        };
        _thumbnailAd.OnAdClosed += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdClosed");
            StartCoroutine(IE_LoadAd(0f));
            Controllers.Get<ServicesController>().SetActiveBanner(AdsEnum.BANNER_BOTTOM, true);
        };
        _thumbnailAd.OnAdDisplayed += ad =>
        {
            Debug.LogWarning("Thumbnail OnAdDisplayed");
            Controllers.Get<ServicesController>().SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);
        };
        _thumbnailAd.OnAdError += (ad, error) =>
        {
            Debug.LogWarning("Thumbnail OnAdError:" + error.Description);
            StartCoroutine(IE_LoadAd(1f));
        };
        _thumbnailAd.OnAdNotAvailable += ad => { Debug.LogWarning("Thumbnail OnAdNotAvailable"); };
        _thumbnailAd.OnAdNotLoaded += ad => { Debug.LogWarning("Thumbnail OnAdNotLoaded"); };
        Ogury.StartAds();
        ModuleController.cheatCode.AddCheatCodeButtonWithInput("Margin X", param =>
        {
            marginX = int.Parse(param.inputParams[0].value.ToString());
            PlayerPrefs.SetInt("OguryMarginX", marginX);
        }, new CheatCode.InputParam
        {
            value = marginX
        });
        ModuleController.cheatCode.AddCheatCodeButtonWithInput("Margin Y", param =>
        {
            marginY = int.Parse(param.inputParams[0].value.ToString());
            PlayerPrefs.SetInt("OguryMarginY", marginY);
        }, new CheatCode.InputParam
        {
            value = marginY
        });
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

        _thumbnailAd.Load(180, 180);
        Debug.LogWarning("Load Ad");
    }
}

public class DisplayMetricsAndroid
{
    // The logical density of the display
    public static float Density { get; protected set; }

    // The screen density expressed as dots-per-inch
    public static int DensityDPI { get; protected set; }

    // The absolute height of the display in pixels
    public static int HeightPixels { get; protected set; }

    // The absolute width of the display in pixels
    public static int WidthPixels { get; protected set; }

    // A scaling factor for fonts displayed on the display
    public static float ScaledDensity { get; protected set; }

    // The exact physical pixels per inch of the screen in the X dimension
    public static float XDPI { get; protected set; }

    // The exact physical pixels per inch of the screen in the Y dimension
    public static float YDPI { get; protected set; }

    static DisplayMetricsAndroid()
    {
        // Early out if we're not on an Android device
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        // The following is equivalent to this Java code:
        //
        // metricsInstance = new DisplayMetrics();
        // UnityPlayer.currentActivity.getWindowManager().getDefaultDisplay().getMetrics(metricsInstance);
        //
        // ... which is pretty much equivalent to the code on this page:
        // http://developer.android.com/reference/android/util/DisplayMetrics.html

        using (
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"),
            metricsClass = new AndroidJavaClass("android.util.DisplayMetrics")
        )
        {
            using (
                AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
                activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
                windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
                displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")
            )
            {
                displayInstance.Call("getMetrics", metricsInstance);
                Density = metricsInstance.Get<float>("density");
                DensityDPI = metricsInstance.Get<int>("densityDpi");
                HeightPixels = metricsInstance.Get<int>("heightPixels");
                WidthPixels = metricsInstance.Get<int>("widthPixels");
                ScaledDensity = metricsInstance.Get<float>("scaledDensity");
                XDPI = metricsInstance.Get<float>("xdpi");
                YDPI = metricsInstance.Get<float>("ydpi");
            }
        }
    }
}