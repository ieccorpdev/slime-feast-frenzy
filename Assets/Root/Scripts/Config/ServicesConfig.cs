using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Root;

[CreateAssetMenu(fileName = "ServicesConfig", menuName = "Services/ServicesConfig")]
public class ServicesConfig : ScriptableObject
{
    [Header("Bundle Version")]
    public int bundleVersion;

    [Header("Analytic AB Test")]
    public ABTestConfig[] abTestConfigs;
    public bool IsTestNoAnalytic;

    [Header("Daily Mission")]
    public DailyMissionConfig[] dailyMissionConfigs;

    [Header("Ads Config")]
    public bool IsNoAds;
    public AdsConfig AppodealConfig;
    public AdsConfig AppotaxBackupConfig;
    public AdsConfig AppotaxConfig4;
    public AdsConfig AppotaxConfig5;

    [Header("Iap Config")]
    public bool IapTestFree;
    public IapConfig IapConfig;

    [Header("LeaderBoard")]
    public string DtaAppId;
    public string SecretKey;
    public bool IsLoginOldNick;

    [HideInInspector]
    public string GDPR_KEY = "Gdpr";
}
