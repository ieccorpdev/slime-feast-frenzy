using System;
using UnityEngine;

namespace AmazonAds {
    public class APSMediationUtils
    {
        public static string APS_IRON_SOURCE_NETWORK_KEY = "APS";

        public static string GetInterstitialNetworkData(string amazonSlotId, AmazonAds.AdResponse adResponse)
        {
            APSIronSourceNetworkInterstitialInputData ironSourceInputData = new APSIronSourceNetworkInterstitialInputData();
#if UNITY_ANDROID
            ironSourceInputData.bidInfo = adResponse.GetBidInfo();
#else
            ironSourceInputData.mediationHints = adResponse.GetMediationHints();
#endif
            ironSourceInputData.pricePointEncoded = adResponse.GetPricePoint();
            ironSourceInputData.uuid = amazonSlotId;

            APSIronSourceInterstitialNetworkData networkData = new APSIronSourceInterstitialNetworkData();
            networkData.interstitial = ironSourceInputData;

            string jsonData = "{ \"interstitial\" :" + JsonUtility.ToJson(ironSourceInputData) + "}";
            return jsonData;

        }

        public static string GetBannerNetworkData(string amazonSlotId, AmazonAds.AdResponse adResponse)
        {
            APSIronSourceNetworkBannerInputData ironSourceInputData = new APSIronSourceNetworkBannerInputData();
#if UNITY_ANDROID
            ironSourceInputData.bidInfo = adResponse.GetBidInfo();
#else
            ironSourceInputData.mediationHints = adResponse.GetMediationHints();
#endif
            ironSourceInputData.pricePointEncoded = adResponse.GetPricePoint();
            ironSourceInputData.uuid = amazonSlotId;
            ironSourceInputData.width = adResponse.GetWidth();
            ironSourceInputData.height = adResponse.GetHeight();

            APSIronSourceBannerNetworkData networkData = new APSIronSourceBannerNetworkData();
            networkData.banner = ironSourceInputData;

            string jsonData = "{ \"banner\" :" + JsonUtility.ToJson(ironSourceInputData) + "}";
            return jsonData;
        }

        public class APSIronSourceNetworkInterstitialInputData
        {
            public string uuid;
            public string pricePointEncoded;
#if UNITY_ANDROID
            public string bidInfo;
#else
            public string mediationHints;
#endif
        }

        public class APSIronSourceNetworkBannerInputData
        {
#if UNITY_ANDROID
            public string bidInfo;
#else
            public String mediationHints;
#endif
            public string pricePointEncoded;
            public string uuid;
            public int width;
            public int height;
        }

        public class APSIronSourceBannerNetworkData
        {
            public APSIronSourceNetworkBannerInputData banner;
        }

        public class APSIronSourceInterstitialNetworkData
        {
            public APSIronSourceNetworkInterstitialInputData interstitial;
        }

        private APSMediationUtils()
        {
        }
    }
}
