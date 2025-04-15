using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if(PluginCMP)
using GoogleMobileAds.Ump.Api;
#endif
#if(PluginFacebook)
using Facebook.Unity;
#endif
#if PluginAdjust
using com.adjust.sdk;
#endif
using IEC.Linq;
using Sabresaurus.PlayerPrefsExtensions;
using UnityEngine;
using UnityEngine.Networking;
#if PluginInAppRating
#if UNITY_ANDROID
using Google.Play.Review;
#endif
#endif

#if UNITY_IOS
using System.Runtime.InteropServices;
using Balaso;
#endif

#if PluginCMP

#endif

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
        [SerializeField] private AnalyticSaveData m_AnalyticSaveData;
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

        IAnalyticEvent m_FirebaseAnalytic;

        private Dictionary<RevAdType, List<int>> m_DictListMilestoneAd = new Dictionary<RevAdType, List<int>>();
        public Action<string> onPushEvent;
        public AdsEnum bannerPos = AdsEnum.BANNER_BOTTOM;
        private string m_TicketId => SystemInfo.deviceUniqueIdentifier.Substring(0,12);
        #endregion

#if PluginByteDanceChinaIOS
        [DllImport("__Internal")]
        private static extern int _initByteDance(string appId, string appName);
#endif
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _openAppSetting();
#endif

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
                        SetPushCustomEventFirebase();

                    });
#if PluginFirebaseAnalytic
                    ((FirebaseAnalyticModule)m_FirebaseAnalytic).RequestPseudoId(pseudoId =>
                    {
                        m_ServiceData.firebasePseudoId = pseudoId;
                        Debug.LogWarning("Firebase Pseudo Id:" + pseudoId);
                    });
#endif
                    break;
                case EventList.Service.INIT_AD_DONE:
#if UNITY_IOS
                    //ShowWhiteBanner();
#endif
                    Ads().Load(bannerPos, null);
                    SetActiveBanner(bannerPos, true);
                    StartCoroutine(IE_LoadAd());
                    break;
                case EventList.Service.ON_ADD_ANALYTIC_SAVE_DATA:
                    SaveData();
                    break;
            }
        }

        public void BehaviourAwake()
        {
            if (isInitAwake) return;
            
            Application.logMessageReceived += (condition, trace, type) =>
            {
                var log = new Logs
                {
                    condition = condition,
                    stackTrace = trace,
                    type = type,
                    dateTime = DateTime.Now.ToString()
                };
                SaveLog.AddLog(log);
            };
            
            UnityToolbag.Dispatcher.Awake();
            Init();
#if UNITY_IOS
            AppTrackingTransparency.RegisterAppForAdNetworkAttribution();
            AppTrackingTransparency.UpdateConversionValue(3);
#endif
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
#if PluginInAppRating
#if UNITY_ANDROID
           m_Review = new ReviewManager();
           StartCoroutine(IE_GetPlayReviewInfo());
#endif
#endif
            isInitAwake = true;
        }

        public void BehaviourStart()
        {
            m_IsInitializing = false;
            foreach (var sEvent in m_InitEvents)
            {
                EventController.RaiseEvent(sEvent, new EventParam(this));
            }
            m_InitEvents.Clear();

            m_IAP.ConnectInit(() =>
            {
                RaiseInitEvent(EventList.Service.INIT_IAP_DONE);
            });
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
            m_Ads = new AdsMock(false);

            m_Login = new LoginMock();
            m_LeaderBoard = new LeaderBoardMock();

            m_Score = new ScoreModule(m_DataSecure, m_LeaderBoard);
            m_ABTest = new AnalyticABTestModule(DataNormal(), config.abTestConfigs);

            m_ListAnalytics.Add(new AnalyticEventMock());
            {
#if (PluginFacebook)
                m_ListAnalytics.Add(new FBAnalyticEvent());
#endif

#if (PluginFirebaseAnalytic)
                var listIgnoredEvent = new List<string>();
                if(config.adjustEventConfig != null)
                {
                    listIgnoredEvent = config.adjustEventConfig.listData
                        .Where(x =>
                        {
                            return !x.PlatformParam.isPushEventFirebase;
                        })
                        .Select(x => x.gameEvent).ToList();
                }
                
                m_FirebaseAnalytic = new FirebaseAnalyticModule(m_SetUpFirebase, listIgnoredEvent);
                m_ListAnalytics.Add(m_FirebaseAnalytic);
                Debug.LogWarning($"Firebase Analytic SDK Version: {config.firebaseSDKVersion}");
#endif
            }

            m_DataRemote = m_DataNormal;
#if (PluginFirebaseRemote)
            m_DataRemote = new FirebaseRemoteConfigModule(m_Json, m_SetUpFirebase, m_DataNormal, config.isTestRemoteConfig, config.GetRemoteConfigs());
            SetPushCustomEventFirebase();
            Debug.LogWarning($"Firebase Remote Config SDK Version: {config.firebaseRemoteSDKVersion}");
#endif
#if PluginByteDanceChinaIOS
            _initByteDance(config.appId, config.appName);
#endif
#if PluginAdjust
            m_AdjustAnalytic = new AdjustAnalyticModule();
            m_AdjustAnalytic.DictEvent = (config.adjustEventConfig != null) ? config.adjustEventConfig.GetDictEvent() : null;
            if (config.adjustEventConfig != null)
            {
                var listRevEvent = config.adjustEventConfig.listData.Where(x =>
                {
                    return x.PlatformParam.isAdjustRev;
                }).Select(x => x.gameEvent).ToList();
                m_AdjustAnalytic.SetListEventRev(listRevEvent);
            }
            m_ListAnalytics.Add(m_AdjustAnalytic);
#endif
            m_IAP = new InAppPurchaseMock(m_DataSecure);
            m_ReceiptVerify = new ReceiptMock();

#if (PluginReceiptVerify)
            m_ReceiptVerify = new ReceiptVerifyOffline();
#endif
#if (PluginReceiptVerifyAdjust)
            m_ReceiptVerify = new ReceiptVerifyAdjust(m_AdjustAnalytic, m_Json, this, m_ServiceData);
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
            iapUnity.SetPurchaseVerifyCallback((productID, isValid) =>
            {
                var eventName = (isValid) ? "in_app_purchase_success_ww" : "in_app_purchase_fail_ww";
                var param = new Dictionary<string, object>
                {
                    ["productID"] = productID
                };
                PushEvent(eventName,1,param);
            } );
#endif
            
            RequestCMP();

            InitAnalyticAndSetupAbTest();
        }

        public void RequestCMP()
        {
            if (Application.isEditor)
                return;
#if (PluginCMP)
            var deviceTestId = PlayerPrefs.GetString(_IECModules.CheatCode.CMP_DEVICE_HASHED_ID, "");
            
            Debug.LogWarning("CMP Test Id:" + deviceTestId);
            
            var requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
            
            if (!string.IsNullOrWhiteSpace(deviceTestId))
            {
                Debug.LogWarning("Enable CMP Test");
                var debugSettings = new ConsentDebugSettings
                {
                    DebugGeography = DebugGeography.EEA,
                    TestDeviceHashedIds = new List<string>
                    {
                        PlayerPrefs.GetString(_IECModules.CheatCode.CMP_DEVICE_HASHED_ID, "")
                    }
                
                };
                requestParameters.ConsentDebugSettings = debugSettings;
            }
            m_CheckConsentStatus = StartCoroutine(IE_WaitConsent());
            ConsentInformation.Update(requestParameters, OnConsentInfoUpdated);
#else
            RequestATT();
#endif
        }

        public void RequestATT()
        {
#if UNITY_IOS
            AppTrackingTransparency.OnAuthorizationRequestDone += (AppTrackingTransparency.AuthorizationStatus status) =>
           {
               switch (status)
               {
                   case AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED:
                       Debug.LogWarning("AuthorizationStatus: NOT_DETERMINED");
                       break;
                   case AppTrackingTransparency.AuthorizationStatus.RESTRICTED:
                       Debug.LogWarning("AuthorizationStatus: RESTRICTED");
                       break;
                   case AppTrackingTransparency.AuthorizationStatus.DENIED:
                       Debug.LogWarning("AuthorizationStatus: DENIED");
                       break;
                   case AppTrackingTransparency.AuthorizationStatus.AUTHORIZED:
                       Debug.LogWarning("AuthorizationStatus: AUTHORIZED");
                       PushEvent("att_145_allow");
                       SendATTAnalytic("att_success");
                       break;
               }
               // Obtain IDFA
               Debug.LogWarning(string.Format("IDFA: {0}", AppTrackingTransparency.IdentifierForAdvertising()));
           };

            AppTrackingTransparency.AuthorizationStatus currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;
            Debug.LogWarning(string.Format("Current authorization status: {0}", currentStatus.ToString()));

            var deviceVer = UnityEngine.iOS.Device.systemVersion;

            Debug.LogWarning("device ver:" + deviceVer);
            
            var isNeedShowAtt = !Application.isEditor && CompareVersion.IsHigherOrEqual(deviceVer, "14.5");

            if (!isNeedShowAtt)
            {
                InitSDKAfterATT(true);
            }
            else if (currentStatus == AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED)
            {
                Debug.LogWarning("Requesting authorization...");
                AppTrackingTransparency.RequestTrackingAuthorization();
                StartCoroutine(IE_WaitATT());
                SendATTAnalytic("att_show");
            }
            else
            {
                InitSDKAfterATT(AppTrackingTransparency.TrackingAuthorizationStatus == AppTrackingTransparency.AuthorizationStatus.AUTHORIZED);
            }
#else
            InitSDKAfterATT(true);
#endif

        }

#if UNITY_IOS
        IEnumerator IE_WaitATT()
        {
            yield return new WaitUntil(() => AppTrackingTransparency.TrackingAuthorizationStatus != AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED);
            Debug.LogWarning("Init ad after ATT");
            InitSDKAfterATT(AppTrackingTransparency.TrackingAuthorizationStatus == AppTrackingTransparency.AuthorizationStatus.AUTHORIZED);
        }
#endif
#if (PluginCMP)

        bool m_IsCMPDontShow = false;
        IEnumerator IE_WaitConsent()
        {
            yield return new WaitUntil(() => (m_IsCMPDontShow || (ConsentInformation.ConsentStatus == ConsentStatus.Obtained)));
            RequestATT();
        }

        private Coroutine m_CheckConsentStatus;
        void OnConsentInfoUpdated(FormError error)
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogError(error);
                m_IsCMPDontShow = true;
                return;
            }

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            if (ConsentInformation.IsConsentFormAvailable())
            {
                LoadForm();
            }
            else
            {
                m_IsCMPDontShow = true;
            }
        }
        
        void LoadForm()
        {
            ConsentForm.Load((form, loadError) =>
            {
                if (loadError != null)
                {
                    // Handle the error.
                    m_IsCMPDontShow = true;
                    Debug.LogError(loadError);
                    return;
                }

                // The consent form was loaded.
                // Save the consent form for future requests.
                // You are now ready to show the form.
                if(ConsentInformation.ConsentStatus == ConsentStatus.Required)
                {
                    form.Show(showError =>
                    {
                        if (showError == null) return;
                        m_IsCMPDontShow = true;
                        Debug.LogError(showError);

                        // Handle dismissal by reloading form.
                        // LoadForm();
                    });
                }
            });
        }
#endif

        void InitSDKAfterATT(bool isHaveIDFA)
        {
#if (PluginKochava)
            if (Application.isEditor)
            {
                Debug.LogWarning("Kochava Android ID: " + config.kochavaConfig.androidID);
                Debug.LogWarning("Kochava iOS ID: " + config.kochavaConfig.iosID);
            }
            m_KochavaAnalytic = new KochavaAnalyticModule(config.kochavaConfig.androidID, config.kochavaConfig.iosID);
            m_ListAnalytics.Add(m_KochavaAnalytic);
#endif
#if (PluginAdjust)
            m_AdjustAnalytic.InitAdjust(config.adjustConfig.androidID, config.adjustConfig.iosID);
#endif
#if (PluginFacebook)
            m_SetUpFacebook = new SetUpFacebookModule();
            m_SetUpFacebook.Init((isLoaded) =>
            {
                FB.Mobile.SetAdvertiserTrackingEnabled(isHaveIDFA);
                FB.Mobile.SetAdvertiserIDCollectionEnabled(isHaveIDFA);
                RaiseInitEvent(EventList.Service.FACEBOOK_LOADED);
            });
#endif

            InitAdsAfterGDPR(true, isHaveIDFA);
        }

        void InitAnalyticAndSetupAbTest()
        {
            foreach (var analytic in m_ListAnalytics)
            {
                analytic.Init();
                analytic.Init(config.adjustEventConfig);
            }

            this.config.SetActiveAbByVersion();
            foreach (var abTest in config.abTestConfigs)
            {
                SetUpAbTest(abTest);
            }
        }

        public void SendATTAnalytic(string eventName)
        {
            const string serverUrl = "https://att-analytic.ieccorp.vn/";
            var endPoint = $"{serverUrl}sendAnalyticEvent.php";

            var formData = new WWWForm();
            formData.AddField("data[bundleId]", Application.identifier);
            formData.AddField("data[eventName]", eventName);
            formData.AddField("data[deviceModel]", SystemInfo.deviceModel);
            formData.AddField("data[deviceVersion]", SystemInfo.operatingSystem);
            formData.AddField("data[appVersion]", Application.version);

            StartCoroutine(ISendATTAnalytic(endPoint, formData));
        }
        IEnumerator ISendATTAnalytic(string url, WWWForm formData)
        {

            var www = UnityWebRequest.Post(url, formData);
            yield return www.SendWebRequest();
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
            m_ServiceData.Init(config.adjustEventConfig, PushEvent);
            if (string.IsNullOrEmpty(m_ServiceData.firstVersion))
            {
                m_ServiceData.firstVersion = m_ServiceData.currentVersion = Application.version.Trim();
                m_ServiceData.installTime = m_ServiceData.updateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else if (!Application.version.Trim().Equals(m_ServiceData.currentVersion))
            {
                if (!m_ServiceData.installTime.Contains("Z"))
                {
                    var isCanParse = DateTime.TryParse(m_ServiceData.installTime, out DateTime installTime);
                    if (isCanParse)
                    {
                        m_ServiceData.installTime = installTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    }
                }
                m_ServiceData.currentVersion = Application.version.Trim();
                m_ServiceData.updateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            m_AnalyticSaveData = DataSecure().GetObject(BaseAnalyticModule.ANALYTIC_DATA_KEY, new AnalyticSaveData());
            InitMilestoneAd();
            Pool.Set(m_ServiceData);
            Pool.Set(m_AnalyticSaveData);
        }

        void InitAdsAfterGDPR(bool isAcceptConsent, bool isAlowAppTracking)
        {
            bool isNoAds = config.IsNoAds || Application.isEditor;

            if (Application.isEditor)
            {
                var adsMock = new AdsMock(config.IsEditorAdsLoaded);
                m_Ads = adsMock;
                EventController.RaiseEvent(EventList.Service.INIT_AD_DONE);
            }
            else
            {
#if (PluginFacebookAudienceNetwork)
#if UNITY_IOS
                Debug.LogWarning($"AudienceNetwork SDK Version: {AudienceNetwork.SdkVersion.Build}");
                Debug.LogWarning("AudienceNetwork SetAdvertiserTrackingEnabled: " + isAlowAppTracking);
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(isAlowAppTracking);
#endif
#endif
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
                applovinAds.Init(config.ApplovinConfig, config.AmazonConfig, true, true, (result) =>
                {
                    if (result)
                    {
                        RaiseInitEvent(EventList.Service.INIT_AD_DONE);
                    }
                },
                PushEvent);
                applovinAds.onAdRevenueAction = m_ServiceData.OnAdRevenue;
                m_Ads = applovinAds;
#endif

                Debug.Log("Ads:" + m_Ads);
                m_Ads.SetOnClick(AdsEnum.BANNER_BOTTOM, () =>
                {
                    PushEvent("click_banner_ads");
                });

                RequestUserIdfa();
            }
        }

        void DoActionIfNotNull(object obj, Action action)
        {
            if (obj != null)
            {
                action();
            }
        }

        public void SaveData()
        {
            Init();
            DataSecure().SetObject(SERVICE_DATA_KEY, m_ServiceData);
            DataSecure().SetObject(BaseAnalyticModule.ANALYTIC_DATA_KEY, m_AnalyticSaveData);
            m_DataNormal.Save();
            m_DataSecure.Save();
        }

        public float GetBannerHeight(UnityEngine.UI.CanvasScaler canvasScaler)
        {
            float unityScale = canvasScaler.transform.localScale.y;
            Debug.Log("m_Ads.GetBannerHeight() : " + m_Ads.GetBannerHeight());
            Debug.Log("unityScale: " + unityScale);
            var scale = 1f;
            if (1f * Screen.height / Screen.width > 2f)
            {
                scale = 1.2f;
                // hotfix for iPhone X, iPhone X GSM, iPhone XS
                if (SystemInfo.deviceModel == "iPhone10,3"
                || SystemInfo.deviceModel == "iPhone10,6"
                || SystemInfo.deviceModel == "iPhone11,2"
                )
                {
                    scale = 1f;
                }
            }
            return (Screen.safeArea.y + m_Ads.GetBannerHeight() / unityScale) * scale;
        }

        void SetUpAbTest(ABTestConfig abTest)
        {
            SetUpAbTest(abTest, m_IsNewInstall);
        }

        public void SetUpAbTest(ABTestConfig abTest, bool isNewInstall)
        {
            if (abTest == null)
            {
                Debug.LogError("AB Test config is null");
                return;
            }
            m_ABTest.SetUpTest(abTest, isNewInstall, (s) =>
            {
                Debug.Log("AB Test event:" + s);
                var abVersion = s.Split('_')[2].Remove(0, 1);
                var isNumber = int.TryParse(abVersion, out int version);
                var eventValue = (isNumber) ? version : 1;
                if (!isNumber) Debug.LogError("Version Ab is not a number");
                Debug.LogWarning("AbTest event value:" + eventValue);
                PushEvent(abTest.KeyName, eventValue, new Dictionary<string, object>
                {
                    { "version", s }
                });
                SetUserProperty("AB_Version", s);
            });
        }

        public void PushEventIapKochava(string packName, float price)
        {
            return;
#if (PluginKochava)
            Kochava.Event myEvent = new Kochava.Event(Kochava.EventType.Purchase);
            myEvent.name = packName;
            myEvent.price = price;
            Kochava.Tracker.SendEvent(myEvent);
#endif
        }

        public void PushEvent(TrackingEvent.AnalyticEvent evt)
        {
            PushEvent(evt.Name, evt.GetValue(), evt.GetParams());
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            PushEvent(eventName, null, value, param);
        }

        public void PushEvent(string eventName, string position, float value = 1, Dictionary<string, object> param = null)
        {
            PushCachedEvents();
            if (param == null) param = new Dictionary<string, object>();
            param["count"] = m_EventCount.IncreaseCount(eventName);
            param["Dx"] = m_Retention.GetCountDate();
            param["Dy"] = m_Retention.GetCountDay();
            param["cumulative_revenue"] = m_ServiceData.revTotalAd;

            if (!string.IsNullOrEmpty(position))
            {
                param["position"] = position;
            }

            if (m_ServiceData.dictCustomParams != null && m_ServiceData.dictCustomParams.ContainsKey(eventName))
            {
                var customParam = m_ServiceData.dictCustomParams[eventName].Invoke();
                foreach (var p in customParam)
                {
                    param[p.Key] = p.Value;
                }
            }

            foreach (var analytic in m_ListAnalytics)
            {
                analytic.PushEvent(eventName, value, param);
            }

            var fullAdCount = m_EventCount.GetCount("show_full_ad");
            var rewardCount = m_EventCount.GetCount("show_reward_ad");
            if (eventName.Equals("show_full_ad"))
            {
                PushEventMilestoneAd(RevAdType.INTER, fullAdCount);
                PushEventMilestoneAd(RevAdType.INTER_N_REWARD, fullAdCount + rewardCount);
            }

            if (eventName.Equals("show_reward_ad"))
            {
                PushEventMilestoneAd(RevAdType.REWARD, rewardCount);
                PushEventMilestoneAd(RevAdType.INTER_N_REWARD, fullAdCount + rewardCount);
            }

            // if (onPushEvent == null)
            // {
            //     onPushEvent = (s =>
            //     {
            //         if (s != "show_full_ad" && s != "show_reward_ad") return;
            //         var count = m_EventCount.GetCount("show_full_ad")
            //                     + m_EventCount.GetCount("show_reward_ad");
            //         if (count % 10 == 0)
            //         {
            //             PushFBLogPurchase();
            //         }
            //     });
            // }
            onPushEvent?.Invoke(eventName);

        }

        public void PushFBLogPurchase()
        {
#if (PluginFacebook)
            if(FB.IsInitialized) 
                FB.LogPurchase(GetValueToSum(),"USD");
#endif
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

        public void ShowFullAds(string position, Action closeCallback = null)
        {
            ShowFullAds(closeCallback, null, null, position);
        }

        public void ShowFullAds(Action closeCallback = null, Action canShowCallback = null, Action cantShowCallback = null, string position = null)
        {
            if (config.IsNoAds || m_ServiceData.isNoAds)
            {
                Debug.Log("show full ads");
                SafeInvoke(closeCallback);
                return;
            }

            m_Ads.Show(AdsEnum.INTERSTITIAL, (param) =>
            {
                if (param.isSuccess)
                {
                    PushEvent("show_full_ad", position);
#if (PluginKochava)
                    m_KochavaAnalytic?.PushEventFullAd();
#endif
#if (PluginAdjust)
                    m_AdjustAnalytic?.PushEventFullAd();
#endif
                    lastFullAdsTime = UnityEngine.Time.realtimeSinceStartup;
                    var fullAdCount = m_EventCount.GetCount("show_full_ad");
                    if (fullAdCount > 0 && fullAdCount < 100)
                    {
                        EventController.RaiseEvent(EventList.Service.PUSH_SHOW_INTER,new EventParam {p0 = (long)fullAdCount});
                    }
                    
                    /*var milestone = GetMilestone(fullAdCount);
                    if (fullAdCount % milestone == 0 && fullAdCount > 0 && fullAdCount < 100)
                    {
                        PushEvent("inter_" + fullAdCount);
                    }*/
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

        public void ShowFullAdsWithTime(float timeCondition, string position, Action closeCallback = null)
        {
            ShowFullAdsWithTime(timeCondition, closeCallback, null, position);
        }

        public bool ShowFullAdsWithTime(float timeCondition, Action closeCallback = null, Action canShowCallback = null, string position = null)
        {
            if (config.IsNoAds || m_ServiceData.isNoAds)
            {
                SafeInvoke(closeCallback);
                return false;
            }

            if (lastFullAdsTime < 0 || UnityEngine.Time.realtimeSinceStartup - lastFullAdsTime >= timeCondition)
            {
                ShowFullAds(closeCallback, canShowCallback, null, position);
                return true;
            }

            SafeInvoke(closeCallback);

            return false;
        }

        public void ShowRewardAds(Action closeCallback, Action skipCallback = null, bool isResetInterCD = true)
        {
            var serviceData = Pool.Get<ServiceData>();
            if (Application.isEditor || config.IsNoAds || serviceData.isNoAdsReward)
            {
                SafeInvoke(closeCallback);
                return;
            }
            if (!IsRewardLoaded())
            {
                Pool.Get<ToastGroup>().Notify.ShowAdsNotReady();
                return;
            }
            Debug.Log("Show reward ads");
            if (m_Ads.IsLoaded(AdsEnum.REWARD))
            {
                m_Ads.Show(AdsEnum.REWARD, (param) =>
                {
                    if (param.isSuccess)
                    {
                        if (isResetInterCD)
                        {
                            lastFullAdsTime = UnityEngine.Time.realtimeSinceStartup;
                        }
                        //PushEvent("show_reward_ad");
#if (PluginKochava)
                        m_KochavaAnalytic?.PushEventRewardAd();
#endif
#if (PluginAdjust)
                        m_AdjustAnalytic?.PushEventRewardAd();
#endif
                        PushEvent("show_reward_ad", 1);

                        var rewardCount = m_EventCount.GetCount("show_reward_ad");
                        var milestone = GetMilestone(rewardCount) / 2;
                        if (rewardCount % milestone == 0 && rewardCount > 0 && rewardCount < 100)
                        {
                            PushEvent("reward_" + rewardCount);
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
            m_ServiceData.isNoAds = isActive;
            SaveData();
            SetActiveBanner(AdsEnum.BANNER_BOTTOM, !isActive);
        }

        public void SetActiveBanner(AdsEnum bannerType, bool isActive)
        {
            if (m_ServiceData.isNoAds && isActive) return;
            if (isActive)
            {
                Ads().Show(bannerType, null);
            }
            else
            {
                Ads().Hide(bannerType);
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

        void SetPushCustomEventFirebase()
        {
            if (config.remoteCustomEventFirebase != null && m_FirebaseAnalytic != null)
            {
                m_FirebaseAnalytic.IsDisable = !m_DataRemote.GetBool(config.remoteCustomEventFirebase.keyName, true);
            }
        }

        IEnumerator IE_CountOneSec()
        {
            while (true)
            {
                yield return oneSec;
                EventController.RaiseEvent(EventList.Service.COUNT_ONE_SEC, new EventParam(this));
            }
        }
        
#if PluginInAppRating
#if UNITY_ANDROID
        private ReviewManager m_Review;
        private PlayReviewInfo m_PlayReviewInfo;

        IEnumerator IE_GetPlayReviewInfo()
        {
            var requestFlowOperation = m_Review.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError("request review info error:" + requestFlowOperation.Error.ToString());
                yield break;
            }

            m_PlayReviewInfo = requestFlowOperation.GetResult();
        }
#endif
        public void RateUs()
        {
            Debug.LogWarning("rate us");
#if UNITY_IOS
            UnityEngine.iOS.Device.RequestStoreReview();
#endif
#if UNITY_ANDROID
            StartCoroutine(IE_RequestReview());
            IEnumerator IE_RequestReview()
            {
                if (m_PlayReviewInfo == null) yield break;
                var launchFlowOperation = m_Review.LaunchReviewFlow(m_PlayReviewInfo);
                yield return launchFlowOperation;
                if (launchFlowOperation.Error != ReviewErrorCode.NoError)
                {
                    Debug.LogError("launch review error:" + launchFlowOperation.Error.ToString());
                    // Application.OpenURL("market://details?id=" + Application.identifier);
                }
            }
#endif
        }
#endif
        const string USER_IDFA = "UserIDFA";
        private string m_UserIDFA;
        public string UserIDFA => m_UserIDFA;
        public bool IsHaveIDFA => !string.IsNullOrEmpty(m_UserIDFA);

        public void RequestUserIdfa()
        {
            if (string.IsNullOrWhiteSpace(m_UserIDFA))
            {
#if UNITY_EDITOR
                m_UserIDFA = "f28127e4-63c4-46f4-bb85-2e7875f6daf9";
#endif
#if UNITY_ANDROID
                try
                {
                    var up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                    var currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                    var client = new AndroidJavaClass ("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                    var adInfo = client.CallStatic<AndroidJavaObject> ("getAdvertisingIdInfo", currentActivity);
     
                    m_UserIDFA = adInfo.Call<string> ("getId").ToString();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
#else
                Application.RequestAdvertisingIdentifierAsync(
                    (string advertisingId, bool trackingEnabled, string error) =>
                    {
                        Debug.Log("advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
                        m_UserIDFA = advertisingId;
                    }
                );
#endif
            }
            else
            {
                Debug.LogWarning("advertisingId " + m_UserIDFA);
            }
        }
        
        private string m_LogFilePath => (Application.persistentDataPath + @"/log.txt");
        private string m_LogFileNonEncr => Application.persistentDataPath + @"/log_non_encr.txt";
        private string m_PlayerDataFilePath => (Application.persistentDataPath + @"/playerData.txt");
        private string m_ServiceDataFilePath => (Application.persistentDataPath + @"/serviceData.txt");
        [NaughtyAttributes.Button]
        public void SendEmail()
        {
            SaveLogToFile();
            SendMail("");
            FeedbackDiscord("test@ieccorp.vn", "Test Content");
        }

        public void FeedbackDiscord(string email, string contentFeedback)
        {
            if(!File.Exists(m_LogFileNonEncr)) return;
            var isIOS = Application.platform == RuntimePlatform.IPhonePlayer;
            string adjustId, firebasePseudoId, adjustCampaign, adjustNetwork, adjustCreative;
            adjustId = firebasePseudoId = adjustCampaign = adjustNetwork = adjustCreative = string.Empty;
#if PluginAdjust
            adjustId = Adjust.getAdid();
            var adjustCampain = Adjust.getAttribution();
            if(adjustCampain != null)
            {
                adjustCampaign = adjustCampain.campaign;
                adjustNetwork = adjustCampain.network;
                adjustCreative = adjustCampain.creative;
            }
#endif
#if PluginFirebaseAnalytic
            firebasePseudoId = m_ServiceData.firebasePseudoId;
#endif
            StartCoroutine(IE_SendFeedbackDiscord());

            IEnumerator IE_SendFeedbackDiscord()
            {
                var logByte = File.ReadAllBytes(m_LogFileNonEncr);
                var fileName = "log.txt";
                var url = config.discordWebhook;
                var idfv = "";
                #if UNITY_IOS
                idfv = UnityEngine.iOS.Device.vendorIdentifier;
                #endif
                var data = new
                {
                    content = contentFeedback,
                    embeds = new[]
                    {
                        new
                        {
                            color = "5814783",
                            fields = new[]
                            {
                                new
                                {
                                    name = "Game Id",
                                    value = Application.identifier
                                },
                                new
                                {
                                    name = "Platform",
                                    value = isIOS ? "iOS" : "Android",
                                },
                                new
                                {
                                    name = "App Version",
                                    value = Application.version
                                },
                                new
                                {
                                    name = "AB Version",
                                    value = m_ServiceData.curAbTestDate
                                }
                            }
                        },
                        new
                        {
                            color = "5814783",
                            fields = new[]
                            {
                                new
                                {
                                    name = "IDFA",
                                    value = m_UserIDFA
                                },
                                new
                                {
                                    name = "IDFV",
                                    value = idfv
                                },
                                new
                                {
                                    name = "Adjust ID",
                                    value = adjustId
                                },
                                new
                                {
                                    name = "Pseudo UID",
                                    value = firebasePseudoId
                                },
                                new
                                {
                                    name = "Ticket ID",
                                    value = m_TicketId
                                }
                            }
                        },
                        new
                        {
                            color = "5814783",
                            fields = new[]
                            {
                                new
                                {
                                    name = "Network",
                                    value = adjustNetwork
                                },
                                new
                                {
                                    name = "Campaign",
                                    value = adjustCampaign
                                },
                                new
                                {
                                    name = "Creative",
                                    value = adjustCreative
                                }
                            }
                        }
                    }
                };

                var formData = new List<IMultipartFormSection>()
                {
                    // new MultipartFormDataSection("payload_json", data.content),
                    new MultipartFormDataSection("payload_json", Json().ToJson(data), Encoding.UTF8, "application/json"),
                    new MultipartFormFileSection("file", logByte, fileName, "text/plain")
                };
                var uwr = UnityWebRequest.Post(url, formData);
                yield return uwr.SendWebRequest();

                if (uwr.error != null)
                {
                    Debug.Log("Error While Sending: " + uwr.error);
                    // SendMail(contentFeedback);
                }
                else
                {
                    Debug.Log("Received: " + uwr.downloadHandler.text);
                }
            }
        }
        
        private void SendMail(string contentFeedback)
        {
            string email = config.emailSupport;
            string title = Application.productName + 
                           $" {(Application.platform == RuntimePlatform.Android ? "Android" : "iOS")}" +
                           $" Ticket {m_TicketId}";

            var newLine = "\n";
            string message = Application.productName + " version: " + Application.version + newLine;
            message += $"UserId: {m_ServiceData.firebasePseudoId}";
            message += newLine + "Hello, " + newLine;
            message += contentFeedback;
            Debug.Log("message SendEmail: " + message);
            var mail = new UT.MailMessage()
                .AddTo(email)
                .SetSubject(title)
                .SetBody(message)
                .AddAttachment(m_LogFilePath);
            UT.Mail.Compose(mail, "Support");
        }

        public void OpenPrivacyPolicyURL()
        {
            var url = config.privacyPolicy;
            if (string.IsNullOrWhiteSpace(url)) return;
#if UNITY_IOS
            IECiosNative.OpenURL(url);
#else
            Application.OpenURL(url);
#endif
        }

        void SaveLogToFile()
        {
            try
            {
                var saveLog = Json().ToJson(SaveLog.GetLog());

                var playerDataJson = Json().ToJson(Pool.Get<PlayerSecret>());
                var serviceDataJson = Json().ToJson(Pool.Get<ServiceData>());
                var playerDataEncrypt = PlayerPrefsUtility.VALUE_STRING_PREFIX +
                                        SimpleEncryption.EncryptString(playerDataJson);
                var serviceDataEncrypt = PlayerPrefsUtility.VALUE_STRING_PREFIX +
                                         SimpleEncryption.EncryptString(serviceDataJson);
                var contentEncr = $"IDFA: {m_UserIDFA}" +
                                  $"\n\nOS Version: {SystemInfo.operatingSystem}" +
                                  $"\n\nUserId: {m_ServiceData.firebasePseudoId}" +
                                  $"\n\nTicketId: {m_TicketId}" +
                                  $"\n\nOS Version:\n{SystemInfo.operatingSystem}";
                var content = contentEncr;
                contentEncr +=
                    $"\n\nPlayerData:\n{playerDataEncrypt}" +
                    $"\n\nServiceData:\n{serviceDataEncrypt}";
                content +=
                    $"\n\nPlayerData:\n{playerDataJson}" +
                    $"\n\nServiceData:\n{serviceDataJson}";
                Debug.Log("serviceDataEncrypt:\n" + serviceDataEncrypt);
                Debug.Log("playerDataEncrypt:\n" + playerDataEncrypt);
                
                contentEncr += $"\n\nLog:\n{saveLog}";
                content += $"\n\nLog:\n{saveLog}";
                File.WriteAllText(m_LogFilePath, contentEncr);
                File.WriteAllText(m_LogFileNonEncr, content);
            }
            catch
            {
                Debug.LogWarning("Log file is in use");
            }
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public void OpenAppSetingIos()
        {
#if UNITY_IOS
            _openAppSetting();
#endif
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

#if (PluginAdjust)
        AdjustAnalyticModule m_AdjustAnalytic;
#endif
        #endregion

        int GetIntDeviceVerion(string version)
        {
            var verSplit = version.Split('.');
            var major = int.Parse(verSplit[0]);
            var minor = int.Parse(verSplit[1]);
            return (major * 1000 + minor);
        }

        private void ShowWhiteBanner()
        {
            var canvasBanner = Resources.Load<GameObject>("CanvasBanner");
            var canvasObj = Instantiate(canvasBanner);
            var scaler = canvasObj.GetComponent<UnityEngine.UI.CanvasScaler>();
            var bannerAds = canvasObj.GetComponentInChildren<BannerAds>();
            bannerAds.SetBannerHeight(GetBannerHeight(scaler));
        }

        IEnumerator IE_LoadAd()
        {
            var delaySec = new WaitForSecondsRealtime(3f);
            while (true)
            {
                Ads().Load(AdsEnum.REWARD, null);

                if (!m_ServiceData.isNoAds)
                {
                    Ads().Load(AdsEnum.INTERSTITIAL, null);
                }
                yield return delaySec;
            }
        }
        
        public void ResetTimeShowFullAds()
        {
            lastFullAdsTime = UnityEngine.Time.realtimeSinceStartup;
        }

        private int GetMilestone(int number)
        {
            var milestone = 1;
            for (int i = 1; i < 10; i++)
            {
                milestone *= 10;
                var div = number / milestone;
                if (div < 10) break;
            }
            return milestone;
        }

        private const string LAST_TOTAL_REV = "ServicesController.LAST_TOTAL_REV";
        
        public void SetRevAllAd(float value)
        {
            m_ServiceData.revTotalAd = value;
        }
        
        public float GetRevAllAd()
        {
            return m_ServiceData.revTotalAd;
        }
        
        public float GetValueToSum()
        {
            if (PlayerPrefs.HasKey(LAST_TOTAL_REV))
            {
                m_ServiceData.lastRevFBPurchase = PlayerPrefs.GetFloat(LAST_TOTAL_REV, 0f);
                PlayerPrefs.DeleteKey(LAST_TOTAL_REV);
            }

            return m_ServiceData.GetRevFBPurchaseValueToSum();
        }

        public void AddListMilestoneAd(RevAdType adType, List<int> listMilestone)
        {
            //Deprecated
            return;
            m_DictListMilestoneAd[adType] = listMilestone;
        }

        private void InitMilestoneAd()
        {
            if(config.adjustEventConfig == null) return;
            m_DictListMilestoneAd = config.adjustEventConfig.GetListMilestone();
        }

        void PushEventMilestoneAd(RevAdType adType, int count)
        {
            if(count <= 0) return;
            if(!m_DictListMilestoneAd.ContainsKey(adType)) return;
            var listMilestone = m_DictListMilestoneAd[adType];
            if(listMilestone == null) return;
            var prefix = m_ServiceData.GetPrefixEvent(adType);
            for (int i = 0; i < listMilestone.Count; i++)
            {
                var milestone = listMilestone[i];
                var eventName = prefix + milestone.ToString();
                if (count % milestone == 0)
                {
                    var valueToSum = m_ServiceData.GetValueToSumRevAd(adType, milestone);
                    Debug.Log($"{eventName} valueToSum:{valueToSum}");
                    Debug.Log($"{eventName} lastRev:{m_ServiceData.GetDictMilestoneAd(adType)[milestone]}");
                    PushEvent(eventName, valueToSum);
                    m_ServiceData.SetValueToSumRevAd(adType, milestone);
                }
            }
        }

        public int GetEventCount(string eventName)
        {
            return m_EventCount.GetCount(eventName);
        }

        public bool IsNewInstall => m_IsNewInstall;
    }
}
