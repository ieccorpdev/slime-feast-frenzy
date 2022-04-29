using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IECModules;
using UnityEngine;
using IEC;

[CreateAssetMenu(fileName = "ServicesConfig", menuName = "Services/ServicesConfig")]
public class ServicesConfig : ScriptableObject
{
    [Header("Project Checklist URL")]
    public string projectChecklistUrl;
    
    [Header("[Deprecate] Bundle Version")]
    public int bundleVersion;

    [Header("Version by platform")]
    [Tooltip("android or ios")]
    public bool isVersionByPlatform;
    public string platform = "android";
    public VersionConfig versionAndroid;
    public VersionConfig versionIOS;

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

    [Header("Percentile Event Config")] public PercentileEventConfig percentileEventConfig;


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

    public void SetActiveAbByVersion()
    {
        if (!isVersionByPlatform) return;

        var versionConfig = GetPlatformVersion();

        foreach (var ab in abTestConfigs)
        {
            ab.IsActive = (ab.KeyName == versionConfig.abTest);
        }
    }

    public VersionConfig GetPlatformVersion()
    {
        return platform.ToLower() == "ios" ? versionIOS : versionAndroid;
    }

    public VersionConfig GetPlatformVersion(string platform)
    {
        return platform.ToLower() == "ios" ? versionIOS : versionAndroid;
    }

    [Serializable]
    public class PlatformConfig
    {
        public string androidID;
        public string iosID;
    }

    [Serializable]
    public class VersionConfig
    {
        public string version = "1.0.0";
        public string abTest = "AB_20220414";
    }
}
