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

            public const string CLAN = BASE + "/clans";
        }

        private static string s_AccessToken = "";

        private static bool s_IsLoggedIn = false;
        private static bool s_IsLoggingIn = false;
        private static string s_PlatformId, s_GameCode;
        private static Action<string> s_OnLoginDone;
        private static Action s_OnLoginFailed;
        public static string CountryCode { get; private set; }
        public static string UserId { get; private set; }

        private static Dictionary<string, string> m_Header = new Dictionary<string, string>();
        // Start is called before the first frame update

        public static void Login(string platformId = null, Action<string> onDone = null,
            Action onError = null, string gameCode = null)
        {
            if (s_IsLoggedIn || s_IsLoggingIn) return;
            if (string.IsNullOrWhiteSpace(platformId))
            {
                platformId = SystemInfo.deviceUniqueIdentifier;
            }

            if (string.IsNullOrWhiteSpace(gameCode))
            {
                gameCode = Application.identifier;
            }

            s_PlatformId = platformId;
            s_GameCode = gameCode;
            s_OnLoginDone = onDone;
            s_OnLoginFailed = onError;
            // Debug.Log(Time.realtimeSinceStartup);
            s_IsLoggingIn = true;
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
                UserId = data["userInfo"]["userId"];
                m_Header.Add("x-access-token", s_AccessToken);
                // Debug.Log(CountryCode);
                onDone?.Invoke(data);
                s_IsLoggingIn = false;
            }, ()=>
            {
                onError?.Invoke();
                s_IsLoggingIn = false;
            }, false);
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

        public static void GetGlobalLB(string key, int page, int pageSize = 10, Action<string> onDone = null,
            Action onError = null)
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

        public static void GetGlobalAroundMe(string key, int pageSize = 10, Action<string> onDone = null,
            Action onError = null)
        {
            Get(APIUrl.LB_GLOBAL_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            }, onDone, onError);
        }

        public static void GetCountryAroundMe(string key, int pageSize = 10, Action<string> onDone = null,
            Action onError = null)
        {
            Get(APIUrl.LB_COUNTRY_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            }, result => { }, onError);
        }

        #endregion

        #region Leaderboard Group

        public static void JoinLeaderBoardGroup(string key, Action<string> onDone = null, Action onError = null,
            string typeLB = "weekly")
        {
            Get(APIUrl.LB_GROUP_JOIN, new
            {
                keyName = key,
                type = typeLB
            }, onDone, onError);
        }

        #endregion

        #region Clan

        public static void CreateClan(string clanName, string description, string type = "open", int maxMembers = 50,
            int requireLevel = 0, string scoreType = "level", string badge = "", Action<string> onDone = null,
            Action onError = null)
        {
            Post(APIUrl.CLAN, new
            {
                name = clanName,
                description,
                type,
                scoreType,
                badge,
                maxMembers,
                requireLevel
            }, onDone, onError);
        }

        public static void SearchClan(string search, string type = "open", int page = 1, int pageSize = 20,
            Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.CLAN, new
            {
                search,
                type,
                page,
                pageSize,
            }, onDone, onError);
        }

        public static void GetClan(string clanId, bool includeMembers, Action<string> onDone = null,
            Action onError = null)
        {
            var url = APIUrl.CLAN + $"/{clanId}?includeMembers={includeMembers}";
            Get(url, null, onDone, onError);
        }

        public static void UpdateClanInfo(string clanId, string clanName, string description, string type,
            string scoreType, string badge, int maxMembers, int requireLevel, Action<string> onDone = null,
            Action onError = null)
        {
            var url = APIUrl.CLAN + $"/{clanId}";
            Put(url, new
            {
                name = clanName,
                description,
                type,
                scoreType,
                badge,
                maxMembers,
                requireLevel
            }, onDone, onError);
        }

        public static void LeaveClan(string clanId, Action<string> onDone = null, Action onError = null)
        {
            var url = APIUrl.CLAN + $"/{clanId}/leave";
            Put(url, null, onDone, onError);
        }

        public static void GetClanMember(string clanId, Action<string> onDone = null, Action onError = null)
        {
            var url = APIUrl.CLAN + $"/{clanId}/members";
            Get(url, null, onDone, onError);
        }

        public static void RequestJoinMember(string clanId, Action<string> onDone = null, Action onError = null)
        {
            var url = APIUrl.CLAN + $"/{clanId}/join-request";
            Post(url, null, onDone, onError);
        }
        
        public static void GetRequestJoinClan(string clanId, Action<string> onDone = null, Action onError = null)
        {
            var url = APIUrl.CLAN + $"/{clanId}/join-request";
            Post(url, null, onDone, onError);
        }

        #endregion

        static void Post(string url, object body, Action<string> onDone = null, Action onError = null,
            bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin())
            {
                Login(s_PlatformId, s =>
                {
                    s_OnLoginDone?.Invoke(s);
                    NetworkBase.Post(url, body, onDone, onError, m_Header); 
                }, s_OnLoginFailed, s_GameCode);
                return;
            }
            NetworkBase.Post(url, body, onDone, onError, m_Header);
        }

        static void Get(string url, object body, Action<string> onDone = null, Action onError = null,
            bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin())
            {
                Login(s_PlatformId, s =>
                {
                    s_OnLoginDone?.Invoke(s);
                    NetworkBase.Get(url, body, onDone, onError, m_Header);     
                }, s_OnLoginFailed, s_GameCode);
                return;
            }
            NetworkBase.Get(url, body, onDone, onError, m_Header);
        }

        static void Put(string url, object body, Action<string> onDone = null, Action onError = null,
            bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin())
            {
                Login(s_PlatformId, s =>
                {
                    s_OnLoginDone?.Invoke(s);
                    NetworkBase.Put(url, body, onDone, onError, m_Header);
                }, s_OnLoginFailed, s_GameCode);
                return;
            }
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