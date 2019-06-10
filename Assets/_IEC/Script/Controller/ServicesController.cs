using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Root;

namespace IEC
{
    public class ServicesController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventUpdate
    {
        #region properties

        const string SERVICE_FIRST_TIME = "ServiceFirstTime";

        [SerializeField]
        private ServicesConfig m_Config;

        bool isInit = false;
        bool isInitAllServices = false;

        float lastFullAdsTime = -1f;

        Action m_OnFullAdsFinish = null;
        Action m_OnFullAdsSkip = null;
        AdsState m_FullAdsState = AdsState.None;
        enum AdsState { None, Finish, Skip };

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
                case EventList.NONE:
                    Debug.Log("event none");
                    break;
            }
        }

        public void BehaviourAwake()
        {
            Init();
        }

        public void BehaviourStart()
        {
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (m_FullAdsState == AdsState.Finish)
            {
                m_FullAdsState = AdsState.None;
                if (m_OnFullAdsFinish != null)
                {
                    m_OnFullAdsFinish();
                    m_OnFullAdsFinish = null;
                }
            }
            else if (m_FullAdsState == AdsState.Skip)
            {
                m_FullAdsState = AdsState.None;
                if (m_OnFullAdsSkip != null)
                {
                    m_OnFullAdsSkip();
                    m_OnFullAdsSkip = null;
                }
            }
        }

        void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                SaveData();
            }
        }

        void OnApplicationQuit()
        {
            SaveData();
        }

        public void InitServices(bool isShowAds, bool isHaveIAP, bool isHaveLeaderBoard)
        {
            Init();

            if (isInitAllServices)
                return;
            isInitAllServices = true;

            if (isShowAds)
            {
                m_Ads = new AdsMock();
                m_Ads.Init(null);

                m_AdsReward = new AdsRewardMock();
                m_AdsReward.Init();

                m_AppodealModule = new AdsAllMock();
                //m_AppodealModule.InitAppodeal(m_Config.AppodealConfig, true);
            }

            if (isHaveIAP)
            {
                m_IAP = new InAppPurchaseMock(m_DataSecure);
                m_IAP.ConnectInit();
            }

            InitAdAfterGDPR(true);

            m_LeaderBoard = new LeaderBoardMock();

            m_Score = new ScoreModule(m_DataSecure, m_LeaderBoard);
            m_ABTest = new AnalyticABTestModule(DataNormal());

            if (m_Config.IapTestFree || m_Config.IsTestNoAnalytic)
            {
                m_Analytic = new AnalyticEventMock();
                m_FirebaseAnalytic = new AnalyticEventMock();
            }
            else
            {
                m_Analytic = new AnalyticEventMock();
                m_FirebaseAnalytic = new AnalyticEventMock();
            }
            m_Analytic.Init();
            m_FirebaseAnalytic.Init();

            DoActionIfNotNull(m_Config, () =>
            {
                m_ABTest.SetUpTest(
                    m_Config.abTestConfigs,
                    m_Config.bundleVersion,
                    () =>
                    {
                        return (!m_DataNormal.HasKey(SERVICE_FIRST_TIME));
                    }
                );
            });
            m_DataNormal.SetBool(SERVICE_FIRST_TIME, false);

            EventController.RaiseEvent(EventList.Service.INIT_DONE, new EventParam(this));
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
        }

        void InitAdAfterGDPR(bool isAcceptConsent)
        {

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
            m_DataNormal.Save();
            m_DataSecure.Save();
        }

        void SetWaitFullAdsClose(Action closeCallback, Action skipCallback)
        {
            m_FullAdsState = AdsState.None;
            m_OnFullAdsFinish = closeCallback;
            m_OnFullAdsSkip = skipCallback;
        }

        void OnFullAdsFinish()
        {
            m_FullAdsState = AdsState.Finish;
        }

        void OnFullAdsSkip()
        {
            m_FullAdsState = AdsState.Skip;
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            m_Analytic.PushEvent(eventName, value, param);
            m_FirebaseAnalytic.PushEvent(eventName, value, param);
        }

        public void ShowFullAds(Action closeCallback = null, Action canShowCallback = null, Action cantShowCallback = null)
        {
            if (m_Config.IsNoAds || Dynamics.Get<PlayerSecret>().isNoAds)
            {
                if (closeCallback != null)
                {
                    closeCallback.Invoke();
                }
                return;
            }

            Debug.Log("Show full ads");

            if (m_AppodealModule.IsFullAdsLoaded())
            {
                Analytic().PushEvent("Ads-Full");
                lastFullAdsTime = UnityEngine.Time.realtimeSinceStartup;
                SetWaitFullAdsClose(closeCallback, cantShowCallback);
                m_AppodealModule.ShowFullAds(OnFullAdsFinish);
                if (canShowCallback != null)
                {
                    canShowCallback.Invoke();
                }
            }
            else
            {
                if (closeCallback != null)
                {
                    closeCallback.Invoke();
                }
            }
        }

        public bool ShowFullAdsWithTime(float timeCondition, Action closeCallback = null, Action canShowCallback = null)
        {
            if (m_Config.IsNoAds || Dynamics.Get<PlayerSecret>().isNoAds)
            {
                if (closeCallback != null)
                {
                    closeCallback.Invoke();
                }
                return false;
            }

            if (lastFullAdsTime < 0 || UnityEngine.Time.realtimeSinceStartup - lastFullAdsTime >= timeCondition)
            {
                ShowFullAds(closeCallback, canShowCallback);
                return true;
            }
            if (closeCallback != null)
            {
                closeCallback.Invoke();
            }

            return false;
        }

        public void ShowRewardAds(Action closeCallback, Action skipCallback = null)
        {
            Debug.Log("Show reward ads");
            if (m_AppodealModule.IsRewardAdsLoaded())
            {
                SetWaitFullAdsClose(closeCallback, skipCallback);
                Analytic().PushEvent("Ads-Reward");
                m_AppodealModule.ShowRewardAds(OnFullAdsFinish, OnFullAdsSkip);
            }
            else
            {
                if (skipCallback != null)
                {
                    skipCallback();
                }
            }
        }

        public bool IsRewardLoaded()
        {
            return m_AppodealModule.IsRewardAdsLoaded();
        }

        public void ShowBanner()
        {
            m_AppodealModule.ShowBanner();
        }

        public void HideBanner()
        {
            m_AppodealModule.HideBanner();
        }

        #region services

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

        IAdsRewardService m_AdsReward;

        public IAdsRewardService AdsReward()
        {
            Init();
            return m_AdsReward;
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

        IAnalyticEvent m_Analytic;
        IAnalyticEvent m_FirebaseAnalytic;

        public IAnalyticEvent Analytic()
        {
            Init();
            return m_Analytic;
        }

        public IAnalyticEvent FirebaseAnalytic()
        {
            Init();
            return m_FirebaseAnalytic;
        }

        ILoginService m_Login;

        public ILoginService Login()
        {
            Init();
            return m_Login;
        }

        IAdsAllService m_AppodealModule;

        #endregion
    }
}