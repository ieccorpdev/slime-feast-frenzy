using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using _IECModules;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    public class LeaderboardModule
    {
        private class APIUrl
        {
            private const string BASE = NetworkBase.BASE;
            public const string LOGIN_CUSTOM = BASE + "/auth/login-custom";
            public const string LB_GLOBAL = BASE + "/leaderboards/global";
            public const string LB_COUNTRY = BASE + "/leaderboards/country";
            public const string USER_STATS = BASE + "/leaderboards/user-stats";
            public const string SUBMIT_SCORE = BASE + "/leaderboards/submit";
            public const string LB_GLOBAL_AROUND = BASE + "/leaderboards/global/around-me";
            public const string LB_COUNTRY_AROUND = BASE + "/leaderboards/country/around-me";
            public const string PROFILE = BASE + "/auth/profile";
            public const string LOCATION_INFO = BASE + "/location-info";
            public const string LB_GROUP_JOIN = BASE + "/leaderboard-groups/join";
        }
        private static string s_AccessToken = "";

        private static bool s_IsLoggedIn = false;
        public static string CountryCode { get; private set; }

        private static Dictionary<string, string> m_Header = new Dictionary<string, string>();
        // Start is called before the first frame update

        public static void Login (string gameCode, string platformId = null, Action<string> onDone = null, Action onError = null)
        {
            if (s_IsLoggedIn) return;
            if (string.IsNullOrWhiteSpace(platformId))
            {
                platformId = SystemInfo.deviceUniqueIdentifier;
            }
            // Debug.Log(Time.realtimeSinceStartup);
            Post(APIUrl.LOGIN_CUSTOM, new
            {
                playerId = platformId,
                gameCode = gameCode,
                platform = Application.platform == RuntimePlatform.Android ? "android" : "ios",
            }, result =>
            {
                // Debug.Log(Time.realtimeSinceStartup);
                // Debug.Log(result);
                var data = SimpleJSON.JSON.Parse(result);
                s_IsLoggedIn = true;
                s_AccessToken = data["accessToken"];
                Debug.Log(s_AccessToken);
                CountryCode = data["userInfo"]["country"];
                m_Header.Add("x-access-token", s_AccessToken);
                // Debug.Log(CountryCode);
                onDone?.Invoke(data);
            }, onError, false);
        }

        public static void GetProfile(Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.PROFILE, null, onDone, onError);
        }

        public static void UpdateProfile(Action<string> onDone = null, Action onError = null)
        {
            Put(APIUrl.PROFILE, null, onDone, onError);
        }

        public static void SubmitScore(string key, int val)
        {
            Post(APIUrl.SUBMIT_SCORE, new
            {
                keyName = key,
                value = val,
                country = CountryCode
            });
        }
        
#region Leaderboard

        public static void GetGlobalLB(string key, int page, int pageSize = 10, Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.LB_GLOBAL, new
            {
                keyName = key,
                country = CountryCode,
                page = page,
                pageSize = pageSize
            }, onDone, onError);
        }

        public static void GetCountryLB(string key, int page, int pageSize = 10, Action<string> onDone = null,
            Action onError = null)
        {
            Get(APIUrl.LB_COUNTRY, new
            {
                keyName = key,
                country = CountryCode,
                page = page,
                pageSize = pageSize
            }, onDone, onError);
        }
        
        public static void GetGlobalAroundMe(string key, int pageSize = 10, Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.LB_GLOBAL_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            }, onDone, onError);
        }
        
        public static void GetCountryAroundMe(string key, int pageSize = 10, Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.LB_COUNTRY_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            }, result =>
            {
                
            }, onError);
        }

#endregion

#region Leaderboard Group

        public static void JoinLeaderBoardGroup(string key, Action<string> onDone = null, Action onError = null, string typeLB = "weekly")
        {
            Get(APIUrl.LB_GROUP_JOIN, new
            {
                keyName = key,
                type = typeLB
            }, onDone, onError);
        }

#endregion
        
        static void Post(string url, object body, Action<string> onDone = null, Action onError = null, bool isNeedLogin = true)
        {
            if(isNeedLogin && !IsLogin()) return;
            NetworkBase.Post(url, body, onDone, onError, m_Header);
        }

        static void Get(string url, object body, Action<string> onDone = null, Action onError = null, bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin()) return;
            NetworkBase.Get(url, body, onDone, onError, m_Header);
        }

        static void Put(string url, object body, Action<string> onDone = null, Action onError = null, bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin()) return;
            NetworkBase.Put(url, body, onDone, onError, m_Header);
        }

        static bool IsLogin()
        {
            if (s_IsLoggedIn) return true;
            Debug.Log("<color=#ff0000ff>Post Failed</color>: Not login");
            return false;

        }
    }
}