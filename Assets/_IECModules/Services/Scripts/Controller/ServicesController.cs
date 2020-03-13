using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class ServicesController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventUpdate, IEventPauseApp, IEventQuitApp
    {
        #region properties

        const string SERVICE_DATA_KEY = "ServiceData";
        const string SERVICE_FIRST_TIME = "ServiceFirstTime";
        public const string SERVER_URL_KEY = "ServerUrl";

        public ServicesConfig config;
        [SerializeField]
        private ServiceData m_ServiceData;
        private bool m_IsRewardLoaded;

        bool isInit = false;
        bool isInitAllServices = false;
        bool isInitAwake = false;

        float lastFullAdsTime = -1f;

        bool m_IsNewInstall = true;
        WaitForSecondsRealtime oneSec = new WaitForSecondsRealtime(1f);

        private bool m_IsInitializing = true;
        private List<string> m_InitEvents = new List<string>();

        EventCountModule m_EventCount;
        RetentionModule m_Retention;

        #endregion

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.Service.LOAD_SCENE:
                    break;
                case EventList.Service.FACEBOOK_LOADED:
                    PushCachedEvents();
                    break;
                case EventList.Service.FIREBASE_LOADED:
                    PushCachedEvents();
                    m_DataRemote.Load(() =>
                    {
                        RaiseInitEvent(EventList.Service.REMOTE_CONFIG_LOADED);
                    });
                    break;
            }
        }

        public void BehaviourAwake()
        {
            if (isInitAwake) return;
            UnityToolbag.Dispatcher.Awake();
            Init();

            InitServices();

            var serverUrl = DataSecure().GetString(SERVER_URL_KEY, TimeServerModule.REQUEST_URL);
            TimeServer().SetServerURL(serverUrl);
            TimeServer().SetCheckType(m_ServiceData.timeCheckType);
            TimeServer().RequestTimeAuto(60f, (result) =>
            {
                if (result)
                {
                    m_Retention.UpdateTimeNow();
                    EventController.RaiseEvent(EventList.Service.UPDATE_TIME_SERVER);
                }
            });
            StartCoroutine(IE_CountOneSec());
            isInitAwake = true;
        }

        public void BehaviourStart()
        {
            m_IsInitializing = false;
            foreach(var sEvent in m_InitEvents)
            {
                EventController.RaiseEvent(sEvent, new EventParam(this));
            }
            m_InitEvents.Clear();
        }

        public void BehaviourUpdate(float deltaTime)
        {
            UnityToolbag.Dispatcher.Update();
        }

        public void BehaviourPauseApp(bool isPaused)
        {
            if (isPaused)
            {
                SaveData();
            }
        }

        public void BehaviourQuitApp()
        {
            SaveData();
        }

        public void InitServices()
        {
            Init();
            if (isInitAllServices)
                return;
            isInitAllServices = true;

            m_IsNewInstall = m_DataNormal.GetBool(SERVICE_FIRST_TIME, true);
            if (m_IsNewInstall)
            {
                m_DataNormal.SetBool(SERVICE_FIRST_TIME, false);
            }

            m_SetUpFacebook = new SetUpSdkMock();
            m_SetUpFirebase = new SetUpSdkMock();

#if (PluginFacebook)
            m_SetUpFacebook = new SetUpFacebookModule();
            m_SetUpFacebook.Init((isLoaded) =>
            {
                RaiseInitEvent(EventList.Service.FACEBOOK_LOADED);
            });
#endif

#if (PluginFirebase)
            m_SetUpFirebase = new SetUpFirebaseModule();
            m_SetUpFirebase.Init((isLoaded) =>
            {
                if (isLoaded)
                {
                    RaiseInitEvent(EventList.Service.FIREBASE_LOADED);
                }
            });
#endif
            m_IAP = new InAppPurchaseMock(m_DataSecure);
            m_ReceiptVerify = new ReceiptMock();

#if (PluginReceiptVerify)
            m_ReceiptVerify = new ReceiptVerifyOffline();
#endif
#if (PluginUnityIAP)
            var iapUnity = new IapUnityModule(config.IapConfig, m_DataSecure, m_ReceiptVerify);
            m_IAP = iapUnity;
            iapUnity.SetCallbackAnalytic(
            (p) =>
            {
                PushEvent("iap_debug_open", 1, new Dictionary<string, object>() { { "package", p } });
            },
            (p) =>
            {
                PushEvent("iap_debug_success_1", 1, new Dictionary<string, object>() { { "package", p } });
            },
            (p) =>
            {
                PushEvent("iap_debug_success_2", 1, new Dictionary<string, object>() { { "package", p } });
            },
            (p) =>
            {
                PushEvent("iap_debug_fail", 1, new Dictionary<string, object>() { { "package", p } });
            });
#endif
            m_Ads = new AdsMock(false);
            m_IAP.ConnectInit(() =>
            {
                RaiseInitEvent(EventList.Service.INIT_IAP_DONE);
            });
            m_Login = new LoginMock();
            m_LeaderBoard = new LeaderBoardMock();

            m_Score = new ScoreModule(m_DataSecure, m_LeaderBoard);
            m_ABTest = new AnalyticABTestModule(DataNormal());

            if (config.IapTestFree || config.IsTestNoAnalytic || Application.isEditor)
            {
                m_ListAnalytics.Add(new AnalyticEventMock());
            }
            else
            {
#if (PluginFacebook)
                m_ListAnalytics.Add(new FBAnalyticEvent());
#endif

#if (PluginFirebaseAnalytic)
                m_ListAnalytics.Add(new FirebaseAnalyticModule(m_SetUpFirebase));
#endif

#if (PluginKochava)
                m_KochavaAnalytic = new KochavaAnalyticModule(config.kochavaConfig.androidID, config.kochavaConfig.iosID);
                m_ListAnalytics.Add(m_KochavaAnalytic);
#endif
            }

            m_DataRemote = m_DataNormal;
#if (PluginFirebaseRemote)
            m_DataRemote = new FirebaseRemoteConfigModule(m_Json, m_SetUpFirebase, m_DataNormal, config.isTestRemoteConfig, config.remoteConfigs);
#endif

            InitAdsAfterGDPR(true);

            foreach (var analytic in m_ListAnalytics)
            {
                analytic.Init();
            }

            foreach (var config in config.abTestConfigs)
            {
                SetUpAbTest(config, this.config.bundleVersion);
            }
        }

        void RaiseInitEvent(string sEvent)
        {
            if (m_IsInitializing)
            {
                m_InitEvents.Add(sEvent);
            }
            else
            {
                EventController.RaiseEvent(sEvent, new EventParam(this));
            }
        }

        void Init()
        {
            if (isInit)
                return;
            isInit = true;

            InitBindingInjection();
        }

        void InitBindingInjection()
        {
            m_Json = new JsonDotNetModule();
            m_Time = new TimeModule();

            m_DataNormal = new PlayerPrefsModule(m_Json);
            if (Application.isEditor)
            {
                m_DataSecure = new SecurePlayerPrefsUtilModule(m_Json);
            }
            else
            {
                m_DataSecure = new SecurePlayerPrefsModule(m_Json);
            }

            m_Location = new UserLocationIP(this, m_Json);
            m_TimeServer = new TimeServerModule(this, m_Json);
            m_EventCount = new EventCountModule(m_DataNormal);
            m_Retention = new RetentionModule(m_DataNormal, m_TimeServer);

            m_ServiceData = DataSecure().GetObject(SERVICE_DATA_KEY, new ServiceData());
            Pool.Set(m_ServiceData);
        }

        void InitAdsAfterGDPR(bool isAcceptConsent)
        {
            bool isNoAds = config.IsNoAds || Application.isEditor;

            if (Application.isEditor)
            {
                var adsMock = new AdsMock(config.IsEditorAdsLoaded);
                m_Ads = adsMock;
            }
            else
            {
#if (PluginAppodeal)
                var disableNetworks = new string[] {
                    AppodealAds.Unity.Api.AppodealNetworks.CHARTBOOST,
                    AppodealAds.Unity.Api.AppodealNetworks.FLURRY,
                    AppodealAds.Unity.Api.AppodealNetworks.STARTAPP,
                    AppodealAds.Unity.Api.AppodealNetworks.YANDEX,
                    AppodealAds.Unity.Api.AppodealNetworks.MINTEGRAL
                };

                var adsAppodeal = new AppodealModule();
                adsAppodeal.InitAppodeal(config.AppodealConfig, disableNetworks, isAcceptConsent, !isNoAds, !isNoAds);
                m_Ads = adsAppodeal;
#endif

#if (PluginGoogleAds)
                var ecpmConfigs = config.GetEcpmAdsWaterfall();
                var listAdsEcpm = new List<IAdsService>();
                if (ecpmConfigs.Length > 0)
                {
                    for (int i = 0; i < ecpmConfigs.Length; i++)
                    {
                        var adsEcpm = new AdmobModule();
                        adsEcpm.Init(ecpmConfigs[i].Config, isAcceptConsent, false, ecpmConfigs[i].Ecpm);
                        listAdsEcpm.Add(adsEcpm);
                    }
                }
                var ecpmAds = new EcpmAdsModule(this, adsAppodeal, listAdsEcpm);
                ecpmAds.SetUseEcpm(AdsEnum.BANNER_BOTTOM, false);
                ecpmAds.SetUseEcpm(AdsEnum.BANNER_TOP, false);
                ecpmAds.SetUseEcpm(AdsEnum.INTERSTITIAL, true);
                ecpmAds.SetUseEcpm(AdsEnum.REWARD, false);

                m_Ads = ecpmAds;
#endif
#if (PluginApplovin)
                var applovinAds = new ApplovinModule();
                applovinAds.Init(config.ApplovinConfig, true, true, (result) =>
                  {
                      if (result)
                      {
                          applovinAds.Load(AdsEnum.BANNER_BOTTOM, null);
                          applovinAds.Load(AdsEnum.INTERSTITIAL, null);
                          applovinAds.Load(AdsEnum.REWARD, null);
                          RaiseInitEvent(EventList.Service.INIT_AD_DONE);
                      }
                  });

                m_Ads = applovinAds;
#endif

                Debug.Log("Ads:" + m_Ads);
                m_Ads.SetOnClick(AdsEnum.BANNER_BOTTOM, () =>
                {
                    PushEvent("click_banner_ads");
                });
            }
        }

        void DoActionIfNotNull(object obj, Action action)
        {
            if (obj != null)
            {
                action();
            }
        }

        void SaveData()
        {
            Init();
            DataSecure().SetObject(SERVICE_DATA_KEY, m_ServiceData);
            m_DataNormal.Save();
            m_DataSecure.Save();
        }

        public float GetBannerHeight(UnityEngine.UI.CanvasScaler canvasScaler)
        {
            float unityScale = canvasScaler.transform.localScale.y;
            Debug.Log("m_Ads.GetBannerHeight() : " + m_Ads.GetBannerHeight());
            Debug.Log("unityScale: " + unityScale);
            return Screen.safeArea.y + m_Ads.GetBannerHeight() / unityScale;
        }

        public void SetUpAbTest(ABTestConfig abTest, int versionCode)
        {
            m_ABTest.SetUpTest(abTest, versionCode, m_IsNewInstall, (s) =>
            {
                Debug.Log("AB Test event:" + s);
                PushEvent(abTest.KeyName, 1, new Dictionary<string, object>
                {
                    {"version", s}
                });
                SetUserProperty("AB_Version", s);
            });
        }

        public void PushEventIapKochava(string packName, float price)
        {
#if (PluginKochava)
            Kochava.Event myEvent = new Kochava.Event(Kochava.EventType.Purchase);
            myEvent.name = packName;
            myEvent.price = price;
            Kochava.Tracker.SendEvent(myEvent);
#endif
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            if (param == null) param = new Dictionary<string, object>();
            param["count"] = m_EventCount.IncreaseCount(eventName);
            param["Dx"] = m_Retention.GetCountDate();
            param["Dy"] = m_Retention.GetCountDay();
            foreach (var analytic in m_ListAnalytics)
            {
                analytic.PushEvent(eventName, value, param);
            }
        }

        public void PushCachedEvents()
        {
            foreach (var analytic in m_ListAnalytics)
            {
                analytic.PushCachedEvents();
            }
        }

        public void SetUserProperty(string name, string property)
        {
            foreach (var analytic in m_ListAnalytics)
            {
                analytic.SetUserProperty(name, property);
            }
        }

        public bool IsCanShowFullAds()
        {
            return m_Ads.IsLoaded(AdsEnum.INTERSTITIAL);
        }

        public void ShowFullAds(Action closeCallback = null, Action canShowCallback = null, Action cantShowCallback = null)
        {
            if (config.IsNoAds || Application.isEditor)
            {
                SafeInvoke(closeCallback);
                return;
            }

            m_Ads.Show(AdsEnum.INTERSTITIAL, (param) =>
            {
                if (param.isSuccess)
                {
                    PushEvent("show_full_ad");
#if (PluginKochava)
                    m_KochavaAnalytic?.PushEventFullAd();
#endif
                    lastFullAdsTime = UnityEngine.Time.realtimeSinceStartup;
                    var fullAdCount = m_EventCount.GetCount("show_full_ad");
                    if (fullAdCount == 50)
                    {
                        PushEvent("inter_50");
                    }
                    SafeInvoke(canShowCallback);
                }
                else
                {
                    SafeInvoke(cantShowCallback);
                }

                SafeInvoke(closeCallback);
            });
        }

        public bool IsCanShowFullAdsWithTime(float timeCondition)
        {
            return (lastFullAdsTime < 0 || UnityEngine.Time.realtimeSinceStartup - lastFullAdsTime >= timeCondition) && IsCanShowFullAds();
        }

        public bool ShowFullAdsWithTime(float timeCondition, Action closeCallback = null, Action canShowCallback = null)
        {
            if (config.IsNoAds)
            {
                SafeInvoke(closeCallback);
                return false;
            }

            if (lastFullAdsTime < 0 || UnityEngine.Time.realtimeSinceStartup - lastFullAdsTime >= timeCondition)
            {
                ShowFullAds(closeCallback, canShowCallback);
                return true;
            }

            SafeInvoke(closeCallback);

            return false;
        }

        public void ShowRewardAds(Action closeCallback, Action skipCallback = null)
        {
            var serviceData = Pool.Get<ServiceData>();
            if (Application.isEditor || config.IsNoAds)
            {
                SafeInvoke(closeCallback);
                return;
            }
            Debug.Log("Show reward ads");
            if (m_Ads.IsLoaded(AdsEnum.REWARD))
            {
                m_Ads.Show(AdsEnum.REWARD, (param) =>
                {
                    if (param.isSuccess)
                    {
                        lastFullAdsTime = UnityEngine.Time.realtimeSinceStartup;
                        //PushEvent("show_reward_ad");
#if (PluginKochava)
                        m_KochavaAnalytic?.PushEventRewardAd();
#endif
                        PushEvent("show_reward_ad", 1);

                        var rewardCount = m_EventCount.GetCount("show_reward_ad");
                        if (rewardCount == 15)
                        {
                            PushEvent("reward_15");
                        }
                    }

                    if ((!param.isSuccess) || param.isSkip)
                    {
                        SafeInvoke(skipCallback);
                    }
                    else
                    {
                        SafeInvoke(closeCallback);
                    }
                });
            }
            else if (serviceData.isInterFallbackForReward)
            {
                ShowFullAds(null, closeCallback, skipCallback);
            }
            else
            {
                SafeInvoke(skipCallback);
            }
        }

        void SafeInvoke(Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        public bool IsRewardLoaded()
        {
            if (Application.isEditor)
            {
                return m_IsRewardLoaded;
            }
            return Ads().IsLoaded(AdsEnum.REWARD);
        }

        public bool IsConfigNoAds()
        {
            Debug.Log("config:" + config);
            return config.IsNoAds;
        }

        public void ActiveNoAds(bool isActive)
        {
            if (isActive)
            {
                Ads().Hide(AdsEnum.BANNER_BOTTOM);
            }
            else
            {
                Ads().Show(AdsEnum.BANNER_BOTTOM, null);
            }
        }

        public void RequestTimeServer()
        {
            TimeServer().RequestTimeNow((result) =>
            {
                if (result)
                {
                    m_Retention.UpdateTimeNow();
                    EventController.RaiseEvent(EventList.Service.UPDATE_TIME_SERVER);
                }
            });
        }

        IEnumerator IE_CountOneSec()
        {
            while (true)
            {
                yield return oneSec;
                EventController.RaiseEvent(EventList.Service.COUNT_ONE_SEC, new EventParam(this));
            }
        }

        #region services

        ISetUpSdkService m_SetUpFacebook;
        ISetUpSdkService m_SetUpFirebase;

        IDataService m_DataNormal;

        public IDataService DataNormal()
        {
            Init();
            return m_DataNormal;
        }

        IDataService m_DataSecure;

        public IDataService DataSecure()
        {
            Init();
            return m_DataSecure;
        }

        IDataService m_DataRemote;

        public IDataService DataRemote()
        {
            Init();
            return m_DataRemote;
        }

        IJsonService m_Json;

        public IJsonService Json()
        {
            Init();
            return m_Json;
        }

        ITimeService m_Time;

        public ITimeService Time()
        {
            Init();
            return m_Time;
        }

        ITimeServer m_TimeServer;

        public ITimeServer TimeServer()
        {
            Init();
            return m_TimeServer;
        }

        IAdsService m_Ads;
        public IAdsService Ads()
        {
            Init();
            return m_Ads;
        }

        IInAppPurchaseService m_IAP;

        public IInAppPurchaseService IAP()
        {
            Init();
            return m_IAP;
        }

        ILeaderBoardService m_LeaderBoard;

        public ILeaderBoardService LeaderBoard()
        {
            Init();
            return m_LeaderBoard;
        }

        IScoreService m_Score;

        public IScoreService Score()
        {
            Init();
            return m_Score;
        }

        IUserLocation m_Location;

        public IUserLocation Location()
        {
            Init();
            return m_Location;
        }

        IAnalyticABTestService m_ABTest;

        public IAnalyticABTestService ABTest()
        {
            Init();
            return m_ABTest;
        }

        List<IAnalyticEvent> m_ListAnalytics = new List<IAnalyticEvent>();

        ILoginService m_Login;

        public ILoginService Login()
        {
            Init();
            return m_Login;
        }

        IReceiptVerifyService m_ReceiptVerify;
        public IReceiptVerifyService ReceiptVerify()
        {
            Init();
            return m_ReceiptVerify;
        }

#if (PluginKochava)
        KochavaAnalyticModule m_KochavaAnalytic;
#endif
        #endregion
    }
}
