using System;
using UnityEngine;

namespace MobileFuseAds.Ads
{
    public enum AdSize
    {
        Banner320x50,
        Banner300x50,
        Banner300x250,
        Banner728x90,
    }
    
    internal static class MobileFuseAdSizeHelpers
    {
        public static AndroidJavaObject GetJavaEnum(AdSize size)
        {
            string sizeClass;
            switch (size)
            {
                case AdSize.Banner320x50:
                    sizeClass = "BANNER_320x50";
                    break;
                case AdSize.Banner300x50:
                    sizeClass = "BANNER_300x50";
                    break;
                case AdSize.Banner300x250:
                    sizeClass = "BANNER_300x250";
                    break;
                case AdSize.Banner728x90:
                    sizeClass = "BANNER_728x90";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }

            return new AndroidJavaClass("com.mobilefuse.sdk.MobileFuseBannerAd$AdSize").GetStatic<AndroidJavaObject>(sizeClass);
        }

        public static float GetHeight(AdSize size)
        {
            switch (size)
            {
                case AdSize.Banner320x50:
                case AdSize.Banner300x50:
                    return 50f;
                case AdSize.Banner300x250:
                    return 250f;
                case AdSize.Banner728x90:
                    return 90f;
                default:
                    return 0f;
            }
        }
    }
}