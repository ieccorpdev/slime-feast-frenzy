using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _IECModules;
using Cysharp.Threading.Tasks;
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
            public const string UPDATE_PLATFORM_ID = BASE + "/auth/account";
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
            public const string CURRENT_CLAN = BASE + "/auth/my-clan";
        }

        private static string s_AccessToken = "";

        private static bool s_IsLoggedIn = false;
        private static bool s_IsLoggingIn = false;
        private static CancellationTokenSource s_LoginCancelToken = new CancellationTokenSource();
        public static string CountryCode { get; private set; }
        public static string UserId { get; private set; }

        private static Dictionary<string, string> m_Header = new Dictionary<string, string>();
        // Start is called before the first frame update

        public static async UniTask<string> Login(string platformId = null, string gameCode = null)
        {
            s_LoginCancelToken.Cancel();
            s_LoginCancelToken.Dispose();
            s_LoginCancelToken = new CancellationTokenSource();
            return await RepeatLogin(s_LoginCancelToken.Token, platformId, gameCode);

        }

        static async UniTask<string> RepeatLogin(CancellationToken token, string platformId = null, string gameCode = null)
        {
            while (!s_IsLoggedIn || !token.IsCancellationRequested)
            {
                if (!s_IsLoggingIn)
                {
                    s_IsLoggingIn = true;
                    if (string.IsNullOrWhiteSpace(platformId))
                    {
                        platformId = SystemInfo.deviceUniqueIdentifier;
                    }

                    if (string.IsNullOrWhiteSpace(gameCode))
                    {
                        gameCode = Application.identifier;
                    }
                    // Debug.Log(Time.realtimeSinceStartup);
                    var result = await Post(APIUrl.LOGIN_CUSTOM, new
                    {
                        playerId = platformId,
                        gameCode = gameCode,
                        platform = Application.platform == RuntimePlatform.Android ? "android" : "ios",
                    }, false);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var data = SimpleJSON.JSON.Parse(result);
                        s_IsLoggedIn = true;
                        s_AccessToken = data["accessToken"];
                        Debug.Log(s_AccessToken);
                        CountryCode = data["userInfo"]["country"];
                        UserId = data["userInfo"]["userId"];
                        m_Header.Add("x-access-token", s_AccessToken);
                        // Debug.Log(CountryCode);
                        s_IsLoggingIn = false;
                        return data;
                    }
                    else
                    {
                        s_IsLoggingIn = false;
                    }
                }
                await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: token);
            }

            return string.Empty;
        }

        public static void UpdatePlatformId(string newPlatformId)
        {
            Put(APIUrl.UPDATE_PLATFORM_ID, new
            {
                playerId = newPlatformId
            }).Forget();
        }

        public static async UniTask<string> GetProfile()
        {
            return await Get(APIUrl.PROFILE, null);
        }

        public static async UniTask<string> UpdateProfile()
        {
            return await Put(APIUrl.PROFILE, null);
        }

        public static void SubmitScore(string key, int val)
        {
            Post(APIUrl.SUBMIT_SCORE, new
            {
                keyName = key,
                value = val,
                country = CountryCode
            }).Forget();
        }

        #region Leaderboard

        public static async UniTask<string> GetGlobalLB(string key, int page = 1, int pageSize = 100)
        {
            return await Get(APIUrl.LB_GLOBAL, new
            {
                keyName = key,
                country = CountryCode,
                page,
                pageSize
            });
        }

        public static async UniTask<string> GetCountryLB(string key, int page = 1, int pageSize = 100)
        {
            return await Get(APIUrl.LB_COUNTRY, new
            {
                keyName = key,
                country = CountryCode,
                page,
                pageSize
            });
        }

        public static async UniTask<string> GetGlobalAroundMe(string key, int pageSize = 10)
        {
            return await Get(APIUrl.LB_GLOBAL_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            });
        }

        public static async UniTask<string> GetCountryAroundMe(string key, int pageSize = 10)
        {
            return await Get(APIUrl.LB_COUNTRY_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            });
        }

        #endregion

        #region Leaderboard Group

        public static async UniTask<string> JoinLeaderBoardGroup(string key, string typeLB = "weekly")
        {
            return await Get(APIUrl.LB_GROUP_JOIN, new
            {
                keyName = key,
                type = typeLB
            });
        }

        #endregion

        #region Clan

        public static async UniTask<string> CreateClan(string clanName, string description, string type = "open", int maxMembers = 50,
            int requireLevel = 0, string scoreType = "level", string badge = "")
        {
            return await Post(APIUrl.CLAN, new
            {
                name = clanName,
                description,
                type,
                scoreType,
                badge,
                maxMembers,
                requireLevel
            });
        }

        public static async UniTask<string> SearchClan(string search, string type = "open", int page = 1, int pageSize = 20)
        {
            return await Get(APIUrl.CLAN, new
            {
                search,
                type,
                page,
                pageSize,
            });
        }

        public static async UniTask<string> GetClan(string clanId, bool includeMembers)
        {
            var url = APIUrl.CLAN + $"/{clanId}?includeMembers={includeMembers}";
            return await Get(url, null);
        }

        public static async UniTask<string> GetCurrentClan()
        {
            return await Get(APIUrl.CURRENT_CLAN, null);
        }

        public static async UniTask<string> UpdateClanInfo(string clanId, string clanName, string description, string type,
            string scoreType, string badge, int maxMembers, int requireLevel)
        {
            var url = APIUrl.CLAN + $"/{clanId}";
            return await Put(url, new
            {
                name = clanName,
                description,
                type,
                scoreType,
                badge,
                maxMembers,
                requireLevel
            });
        }

        public static async UniTask<string> LeaveClan(string clanId)
        {
            var url = APIUrl.CLAN + $"/{clanId}/leave";
            return await Put(url, null);
        }

        public static async UniTask<string> GetClanMember(string clanId)
        {
            var url = APIUrl.CLAN + $"/{clanId}/members";
            return await Get(url, null);
        }

        public static async UniTask<string> RequestJoinMember(string clanId)
        {
            var url = APIUrl.CLAN + $"/{clanId}/join-request";
            return await Post(url, null);
        }
        
        public static async UniTask<string> GetRequestJoinClan(string clanId)
        {
            var url = APIUrl.CLAN + $"/{clanId}/join-request";
            return await Post(url, null);
        }

        public static async UniTask<string> CreateClanChat(string clanId, string message)
        {
            var url = APIUrl.CLAN + $"/{clanId}/chats";
            return await Post(url, new
            {
                type = "string",
                content = new {
                    msg = message
                }
            });
        }

        public static async UniTask<string> CreateRequestLive(string clanId, int maxHelp = 5)
        {
            var url = APIUrl.CLAN + $"/{clanId}/chats";
            return await Post(url, new
            {
                type = "request-live",
                content = new
                {
                    msg = "",
                    current = 0,
                    max = maxHelp
                }
            });
        }

        public static async UniTask<string> UpdateRequestLive(string clanId, string chatId, int helpCount, int maxHelp = 5)
        {
            var url = APIUrl.CLAN + $"/{clanId}/chats/{chatId}";
            return await Put(url, new
            {
                type = "request-live",
                content = new
                {
                    msg = "",
                    current = helpCount,
                    max = maxHelp
                }
            });
        }

        #endregion

        static async UniTask<string> Post(string url, object body, bool isNeedLogin = true)
        {
            if (!isNeedLogin || IsLogin()) return await NetworkBase.Post(url, body, m_Header);
            Debug.LogError("Not logged in");
            return string.Empty;
        }

        static async UniTask<string> Get(string url, object body, Action<string> onDone = null, Action onError = null,
            bool isNeedLogin = true)
        {
            if (!isNeedLogin || IsLogin()) return await NetworkBase.Get(url, body, m_Header);
            Debug.LogError("Not logged in");
            return string.Empty;
        }

        static async UniTask<string> Put(string url, object body, Action<string> onDone = null, Action onError = null,
            bool isNeedLogin = true)
        {
            if (!isNeedLogin || IsLogin()) return await NetworkBase.Put(url, body, m_Header);
            Debug.LogError("Not logged in");
            return string.Empty;
        }

        static bool IsLogin()
        {
            if (s_IsLoggedIn) return true;
            Debug.Log("<color=#ff0000ff>Post Failed</color>: Not login");
            return false;
        }
    }
}