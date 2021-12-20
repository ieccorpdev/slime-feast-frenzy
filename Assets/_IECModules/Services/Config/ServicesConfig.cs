using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;

[CreateAssetMenu(fileName = "ServicesConfig", menuName = "Services/ServicesConfig")]
public class ServicesConfig : ScriptableObject
{
    [Header("Project Checklist URL")]
    public string projectChecklistUrl;
    
    [Header("Bundle Version")]
    public int bundleVersion;

    [Header("Analytic AB Test")]
    public ABTestConfig[] abTestConfigs;
    public bool IsTestNoAnalytic;

    [Header("Daily Mission")]
    public DailyMissionConfig[] dailyMissionConfigs;

    [Header("Ads Config")]
    public bool IsNoAds;
    public bool IsEditorAdsLoaded;
    public AdsConfig AppodealConfig;

    public EcpmAdConfig[] EcpmAds;

    EcpmAdConfig[] EcpmAdsWaterfall;

    public AdsConfig ApplovinConfig;
    public AdsConfig IronSourceConfig;
    public OguryAdsConfig OguryConfig;

    [Header("Iap Config")]
    public bool IapTestFree;
    public IapConfig IapConfig;

    [Header("LeaderBoard")]
    public string DtaAppId;
    public string SecretKey;
    public bool IsLoginOldNick;

    [Header("Kochava")]
    public PlatformConfig kochavaConfig;

    [Header("Adjust")]
    public PlatformConfig adjustConfig;
    public AdjustAnalyticEventConfig adjustEventConfig;

    [Header("RemoteCustomEventFirebase")]
    public RemoteConfigData remoteCustomEventFirebase;


    [Header("RemoteConfig")]
    public bool isTestRemoteConfig;
    public List<RemoteConfigData> remoteConfigs;
    [Header("Email Support")]
    public string emailSupport;
    
    [Header("SDK Version")]
    public string firebaseSDKVersion;
    public string firebaseRemoteSDKVersion;

    [HideInInspector]
    public string GDPR_KEY = "Gdpr";

    public EcpmAdConfig[] GetEcpmAdsWaterfall()
    {
        if (EcpmAdsWaterfall == null || EcpmAdsWaterfall.Length == 0)
        {
            EcpmAdsWaterfall = EcpmAds;
            for (int i = 0; i < EcpmAdsWaterfall.Length - 1; i++)
            {
                for (int j = i + 1; j < EcpmAdsWaterfall.Length; j++)
                {
                    if (EcpmAdsWaterfall[i].Ecpm < EcpmAdsWaterfall[j].Ecpm)
                    {
                        var tmp = EcpmAdsWaterfall[i];
                        EcpmAdsWaterfall[i] = EcpmAdsWaterfall[j];
                        EcpmAdsWaterfall[j] = tmp;
                    }
                }
            }
        }

        return EcpmAdsWaterfall;
    }

    public List<RemoteConfigData> GetRemoteConfigs()
    {
        if (remoteConfigs == null)
        {
            remoteConfigs = new List<RemoteConfigData>();
        }

        if(remoteCustomEventFirebase != null && !remoteConfigs.Contains(remoteCustomEventFirebase))
        {
            remoteConfigs.Add(remoteCustomEventFirebase);
        }
        return remoteConfigs;
    }

    [Serializable]
    public class PlatformConfig
    {
        public string androidID;
        public string iosID;
    }
}
